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

        private int RepeatRegistrationInterval = -1;
        private string baseUrl = null;
        
        private CancellationTokenSource RepeatRegistrationCancellationToken = null;
        public int RequestTimeout = DEFAULT_REQUEST_TIMEOUT;

        public void LoadSettings(RegistryClientSettings settings)
        {
            LoadProxy(settings.ProxyConfig);

            if (settings.RegistryConfig.RepeatRegistration.HasValue)
                RepeatRegistrationInterval = settings.RegistryConfig.RepeatRegistration.Value;

            if (settings.ClientConfig.RequestConfig.RequestTimeout.HasValue)
                RequestTimeout = settings.ClientConfig.RequestConfig.RequestTimeout.Value;
            else
                RequestTimeout = DEFAULT_REQUEST_TIMEOUT;

            baseUrl = settings.RegistryConfig.RegistryUrl.TrimEnd('/') + PATH_SEPERATOR + REGISTRY_BASE_PATH;
        }

        private RegistryHttpClient(HttpClientHandler clientHandler, RegistryClientSettings registryClientSettings) : base(clientHandler) 
        {
            JsonSerializerSettings = new DependencyInjectionJsonSerializerSettings();

            Settings = registryClientSettings ?? RegistryClientSettings.LoadSettings();
            Settings = Settings ?? throw new NullReferenceException("Settings is null");

            LoadSettings(Settings);
        }

        public RegistryHttpClient() : this (DEFAULT_HTTP_CLIENT_HANDLER, null)
        { }
        public RegistryHttpClient(RegistryClientSettings registryClientSettings) : this(DEFAULT_HTTP_CLIENT_HANDLER, registryClientSettings)
        { }
        public RegistryHttpClient(RegistryClientSettings registryClientSettings, HttpClientHandler clientHandler) : this(clientHandler, registryClientSettings)
        { }

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

        public void RepeatRegistration(IAssetAdministrationShellDescriptor aasDescriptor, CancellationTokenSource cancellationToken)
        {
            RepeatRegistrationCancellationToken = cancellationToken;
            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    IResult<IAssetAdministrationShellDescriptor> result = CreateOrUpdateAssetAdministrationShellRegistration(aasDescriptor.Identification.Id, aasDescriptor);
                    logger.Info("Registration-Renewal - Success: " + result.Success + " | Messages: " + result.Messages.ToString());
                    await Task.Delay(RepeatRegistrationInterval);
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
            var response = base.SendRequest(request, RequestTimeout);
            return base.EvaluateResponse<IAssetAdministrationShellDescriptor>(response, response.Entity);
        }

        public IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellRegistration(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasId)));

            var request = base.CreateRequest(GetUri(aasId), HttpMethod.Get);
            var response = base.SendRequest(request, RequestTimeout);
            return base.EvaluateResponse<IAssetAdministrationShellDescriptor>(response, response.Entity);
        }

        public IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            if (predicate == null)
                return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(new ArgumentNullException(nameof(predicate)));

            var request = base.CreateRequest(GetUri(), HttpMethod.Get);
            var response = base.SendRequest(request, RequestTimeout);
            var result = base.EvaluateResponse<IEnumerable<IAssetAdministrationShellDescriptor>>(response, response.Entity);

            if (!result.Success || result.Entity == null)
                return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(result);
            else
            {
                var foundItems = result.Entity.Where(w => predicate.Invoke(w));
                return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(result.Success, foundItems?.AsQueryableElementContainer(), result.Messages);
            }
        }

        public IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations()
        {
            var request = base.CreateRequest(GetUri(), HttpMethod.Get);
            var response = base.SendRequest(request, RequestTimeout);
            var result = base.EvaluateResponse<IEnumerable<IAssetAdministrationShellDescriptor>>(response, response.Entity);
            return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(result.Success, result.Entity?.AsQueryableElementContainer(), result.Messages);
        }

        public IResult DeleteAssetAdministrationShellRegistration(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result(new ArgumentNullException(nameof(aasId)));

            if (RepeatRegistrationInterval > 0)
                RepeatRegistrationCancellationToken?.Cancel();

            var request = base.CreateRequest(GetUri(aasId), HttpMethod.Delete);
            var response = base.SendRequest(request, RequestTimeout);
            return base.EvaluateResponse(response, response.Entity);
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
            var response = base.SendRequest(request, RequestTimeout);
            return base.EvaluateResponse<ISubmodelDescriptor>(response, response.Entity);
        }

        public IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId, Predicate<ISubmodelDescriptor> predicate)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(new ArgumentNullException(nameof(aasId)));
            if (predicate == null)
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(new ArgumentNullException(nameof(predicate)));

            var request = base.CreateRequest(GetUri(aasId, SUBMODEL_PATH), HttpMethod.Get);
            var response = base.SendRequest(request, RequestTimeout);
            var result = base.EvaluateResponse<IEnumerable<ISubmodelDescriptor>>(response, response.Entity);

            if (!result.Success || result.Entity == null)
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(result);
            else
            {
                var foundItems = result.Entity.Where(w => predicate.Invoke(w));
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(result.Success, foundItems?.AsQueryableElementContainer(), result.Messages);
            }
        }

        public IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(new ArgumentNullException(nameof(aasId)));

            var request = base.CreateRequest(GetUri(aasId, SUBMODEL_PATH), HttpMethod.Get);
            var response = base.SendRequest(request, RequestTimeout);
            var result = base.EvaluateResponse<IEnumerable<ISubmodelDescriptor>>(response, response.Entity);

            return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(result.Success, result.Entity?.AsQueryableElementContainer(), result.Messages);
        }

        public IResult<ISubmodelDescriptor> RetrieveSubmodelRegistration(string aasId, string submodelId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelId)));

            var request = base.CreateRequest(GetUri(aasId, SUBMODEL_PATH, submodelId), HttpMethod.Get);
            var response = base.SendRequest(request, RequestTimeout);
            return base.EvaluateResponse<ISubmodelDescriptor>(response, response.Entity);
        }

        public IResult DeleteSubmodelRegistration(string aasId, string submodelId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result(new ArgumentNullException(nameof(submodelId)));

            var request = base.CreateRequest(GetUri(aasId, SUBMODEL_PATH, submodelId), HttpMethod.Delete);
            var response = base.SendRequest(request, RequestTimeout);
            return base.EvaluateResponse(response, response.Entity);
        }   
    }
}
