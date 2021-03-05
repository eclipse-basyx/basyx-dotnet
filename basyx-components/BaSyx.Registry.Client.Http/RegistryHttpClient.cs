/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.API.Components;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.Client.Http;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.ResultHandling;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BaSyx.Registry.Client.Http
{
    public class RegistryHttpClient : SimpleHttpClient, IAssetAdministrationShellRegistry
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public RegistryClientSettings Settings { get; }

        public const string REGISTRY_BASE_PATH = "api/v1/registry";
        public const string SUBMODEL_PATH = "submodels";
        public const string PATH_SEPERATOR = "/";

        private string baseUrl = null;
        
        private CancellationTokenSource RepeatRegistrationCancellationToken = null;

        public void LoadSettings(RegistryClientSettings settings)
        {
            LoadProxy(settings.ProxyConfig);

            if (settings.ClientConfig.RequestConfig.RequestTimeout.HasValue)
                SetDefaultTimeout(TimeSpan.FromMilliseconds(settings.ClientConfig.RequestConfig.RequestTimeout.Value));

            baseUrl = settings.RegistryConfig.RegistryUrl.TrimEnd('/') + PATH_SEPERATOR + REGISTRY_BASE_PATH;
        }

        public RegistryHttpClient() : this(null, null)
        { }
        public RegistryHttpClient(RegistryClientSettings registryClientSettings) : this(registryClientSettings, null)
        { }
        public RegistryHttpClient(RegistryClientSettings registryClientSettings, HttpMessageHandler messageHandler) : base(messageHandler)
        {
            JsonSerializerSettings = new DependencyInjectionJsonSerializerSettings();
            Settings = registryClientSettings ?? RegistryClientSettings.LoadSettings() ?? throw new NullReferenceException("Settings is null");
            LoadSettings(Settings);
        }

        public Uri GetUri(params string[] pathElements)
        {
            string path = baseUrl;

            if (pathElements?.Length > 0)
                foreach (var pathElement in pathElements)
                {
                    string encodedPathElement = HttpUtility.UrlEncode(pathElement);
                    path = path.TrimEnd('/') + PATH_SEPERATOR + encodedPathElement.TrimEnd('/');
                }
            return new Uri(path);
        }

        public void RepeatRegistration(IAssetAdministrationShellDescriptor aasDescriptor, TimeSpan interval, CancellationTokenSource cancellationToken)
        {
            RepeatRegistrationCancellationToken = cancellationToken;
            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    IResult<IAssetAdministrationShellDescriptor> result = CreateOrUpdateAssetAdministrationShellRegistration(aasDescriptor.Identification.Id, aasDescriptor);
                    logger.Info("Registration-Renewal - Success: " + result.Success + " | Messages: " + result.Messages.ToString());
                    await Task.Delay(interval);
                }
            }, cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        } 
        
        public void CancelRepeatingRegistration()
        {
            RepeatRegistrationCancellationToken?.Cancel();
        }

        public IResult<IAssetAdministrationShellDescriptor> CreateOrUpdateAssetAdministrationShellRegistration(string aasId, IAssetAdministrationShellDescriptor aasDescriptor)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasId)));
            if (aasDescriptor == null)
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasDescriptor)));

            var request = base.CreateJsonContentRequest(GetUri(aasId), HttpMethod.Put, aasDescriptor);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<IAssetAdministrationShellDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellRegistration(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasId)));

            var request = base.CreateRequest(GetUri(aasId), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<IAssetAdministrationShellDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            if (predicate == null)
                return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(new ArgumentNullException(nameof(predicate)));

            var request = base.CreateRequest(GetUri(), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<IEnumerable<IAssetAdministrationShellDescriptor>>(response, response.Entity);

            if (!result.Success || result.Entity == null)
            {
                response?.Entity?.Dispose();
                return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(result);
            }
            else
            {
                response?.Entity?.Dispose();
                var foundItems = result.Entity.Where(w => predicate.Invoke(w));
                return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(result.Success, foundItems?.AsQueryableElementContainer(), result.Messages);
            }
        }

        public IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations()
        {
            var request = base.CreateRequest(GetUri(), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<IEnumerable<IAssetAdministrationShellDescriptor>>(response, response.Entity);
            response?.Entity?.Dispose();
            return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(result.Success, result.Entity?.AsQueryableElementContainer(), result.Messages);
        }

        public IResult DeleteAssetAdministrationShellRegistration(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result(new ArgumentNullException(nameof(aasId)));

            var request = base.CreateRequest(GetUri(aasId), HttpMethod.Delete);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<ISubmodelDescriptor> CreateOrUpdateSubmodelRegistration(string aasId, string submodelId, ISubmodelDescriptor submodelDescriptor)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelId)));
            if (submodelDescriptor == null)
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelDescriptor)));

            var request = base.CreateJsonContentRequest(GetUri(aasId, SUBMODEL_PATH, submodelId), HttpMethod.Put, submodelDescriptor);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ISubmodelDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId, Predicate<ISubmodelDescriptor> predicate)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(new ArgumentNullException(nameof(aasId)));
            if (predicate == null)
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(new ArgumentNullException(nameof(predicate)));

            var request = base.CreateRequest(GetUri(aasId, SUBMODEL_PATH), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<IEnumerable<ISubmodelDescriptor>>(response, response.Entity);

            if (!result.Success || result.Entity == null)
            {
                response?.Entity?.Dispose();
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(result);
            }
            else
            {
                response?.Entity?.Dispose();
                var foundItems = result.Entity.Where(w => predicate.Invoke(w));
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(result.Success, foundItems?.AsQueryableElementContainer(), result.Messages);
            }
        }

        public IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(new ArgumentNullException(nameof(aasId)));

            var request = base.CreateRequest(GetUri(aasId, SUBMODEL_PATH), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<IEnumerable<ISubmodelDescriptor>>(response, response.Entity);
            response?.Entity?.Dispose();
            return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(result.Success, result.Entity?.AsQueryableElementContainer(), result.Messages);
        }

        public IResult<ISubmodelDescriptor> RetrieveSubmodelRegistration(string aasId, string submodelId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelId)));

            var request = base.CreateRequest(GetUri(aasId, SUBMODEL_PATH, submodelId), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ISubmodelDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult DeleteSubmodelRegistration(string aasId, string submodelId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result(new ArgumentNullException(nameof(submodelId)));

            var request = base.CreateRequest(GetUri(aasId, SUBMODEL_PATH, submodelId), HttpMethod.Delete);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }   
    }
}
