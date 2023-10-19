/*******************************************************************************
* Copyright (c) 2022 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.Models.Connectivity;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.Client.Http;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BaSyx.API.Http;
using BaSyx.Utils.Extensions;
using BaSyx.API.Clients;
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace BaSyx.Registry.Client.Http
{
    public class RegistryHttpClient : SimpleHttpClient, IAssetAdministrationShellRegistryClient
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<RegistryHttpClient>();
        public RegistryClientSettings Settings { get; }

        private string baseUrl = null;

        public IEndpoint Endpoint { get; private set; }
        
        private CancellationTokenSource RepeatRegistrationCancellationToken = null;

        public void LoadSettings(RegistryClientSettings settings)
        {
            LoadProxy(settings.ProxyConfig);

            if (settings.ClientConfig.RequestConfig.RequestTimeout.HasValue)
                SetDefaultTimeout(TimeSpan.FromMilliseconds(settings.ClientConfig.RequestConfig.RequestTimeout.Value));

            baseUrl = settings.RegistryConfig.RegistryUrl.TrimEnd('/');
            Endpoint = new Endpoint(baseUrl, InterfaceName.AssetAdministrationRegistryInterface);
        }

        public RegistryHttpClient() : this(null, null)
        { }
        public RegistryHttpClient(RegistryClientSettings registryClientSettings) : this(registryClientSettings, null)
        { }
        public RegistryHttpClient(RegistryClientSettings registryClientSettings, HttpMessageHandler messageHandler) : base(messageHandler)
        {
            var options = new DefaultJsonSerializerOptions();
            var services = DefaultImplementation.GetStandardServiceCollection();
            options.AddDependencyInjection(new DependencyInjectionExtension(services));
            options.AddFullSubmodelElementConverter();
            JsonSerializerOptions = options.Build();
            Settings = registryClientSettings ?? RegistryClientSettings.LoadSettings() ?? throw new NullReferenceException("Settings is null");
            LoadSettings(Settings);
        }

        public Uri GetPath(string requestPath, string aasId = null, string submodelId = null)
        {
            string path = baseUrl;

            if (string.IsNullOrEmpty(requestPath))
                return new Uri(path);

            if(!string.IsNullOrEmpty(aasId))
            {
                requestPath = requestPath.Replace("{aasIdentifier}", aasId.Base64UrlEncode());
            }

            if (!string.IsNullOrEmpty(submodelId))
            {
                requestPath = requestPath.Replace("{submodelIdentifier}", submodelId.Base64UrlEncode());
            }

            return new Uri(path + requestPath);
        }

        public void RepeatRegistration(IAssetAdministrationShellDescriptor aasDescriptor, TimeSpan interval, CancellationTokenSource cancellationToken)
        {
            RepeatRegistrationCancellationToken = cancellationToken;
            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    IResult result = UpdateAssetAdministrationShellRegistration(aasDescriptor.Id.Id, aasDescriptor);
                    logger.LogInformation("Registration-Renewal - Success: " + result.Success + " | Messages: " + result.Messages.ToString());
                    await Task.Delay(interval);
                }
            }, cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        } 
        
        public void CancelRepeatingRegistration()
        {
            RepeatRegistrationCancellationToken?.Cancel();
        }

        public IResult<IAssetAdministrationShellDescriptor> CreateAssetAdministrationShellRegistration(IAssetAdministrationShellDescriptor aasDescriptor)
        {
            return CreateAssetAdministrationShellRegistrationAsync(aasDescriptor).GetAwaiter().GetResult();
        }

        public IResult UpdateAssetAdministrationShellRegistration(string aasIdentifier, IAssetAdministrationShellDescriptor aasDescriptor)
        {
            return UpdateAssetAdministrationShellRegistrationAsync(aasIdentifier, aasDescriptor).GetAwaiter().GetResult();
        }

        public IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellRegistration(string aasIdentifier)
        {
            return RetrieveAssetAdministrationShellRegistrationAsync(aasIdentifier).GetAwaiter().GetResult();
        }

        public IResult<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>> RetrieveAllAssetAdministrationShellRegistrations(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            return RetrieveAllAssetAdministrationShellRegistrationsAsync(predicate).GetAwaiter().GetResult();
        }

        public IResult<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>> RetrieveAllAssetAdministrationShellRegistrations()
        {
            return RetrieveAllAssetAdministrationShellRegistrationsAsync().GetAwaiter().GetResult();
        }

        public IResult DeleteAssetAdministrationShellRegistration(string aasIdentifier)
        {
            return DeleteAssetAdministrationShellRegistrationAsync(aasIdentifier).GetAwaiter().GetResult();
        }

        public IResult<ISubmodelDescriptor> CreateSubmodelRegistration(string aasIdentifier, ISubmodelDescriptor submodelDescriptor)
        {
             return CreateSubmodelRegistrationAsync(aasIdentifier, submodelDescriptor).GetAwaiter().GetResult();
        }

        public IResult UpdateSubmodelRegistration(string aasIdentifier, string submodelIdentifier, ISubmodelDescriptor submodelDescriptor)
        {
            return UpdateSubmodelRegistrationAsync(aasIdentifier, submodelIdentifier, submodelDescriptor).GetAwaiter().GetResult();
        }

        public IResult<PagedResult<IEnumerable<ISubmodelDescriptor>>> RetrieveAllSubmodelRegistrations(string aasIdentifier, Predicate<ISubmodelDescriptor> predicate)
        {
            return RetrieveAllSubmodelRegistrationsAsync(aasIdentifier, predicate).GetAwaiter().GetResult();
        }

        public IResult<PagedResult<IEnumerable<ISubmodelDescriptor>>> RetrieveAllSubmodelRegistrations(string aasIdentifier)
        {
            return RetrieveAllSubmodelRegistrationsAsync(aasIdentifier).GetAwaiter().GetResult();
        }

        public IResult<ISubmodelDescriptor> RetrieveSubmodelRegistration(string aasIdentifier, string submodelIdentifier)
        {
            return RetrieveSubmodelRegistrationAsync(aasIdentifier, submodelIdentifier).GetAwaiter().GetResult();
        }

        public IResult DeleteSubmodelRegistration(string aasIdentifier, string submodelIdentifier)
        {
            return DeleteSubmodelRegistrationAsync(aasIdentifier, submodelIdentifier).GetAwaiter().GetResult();
        }

        #region Async Interface
        public async Task<IResult<IAssetAdministrationShellDescriptor>> CreateAssetAdministrationShellRegistrationAsync(IAssetAdministrationShellDescriptor aasDescriptor)
        {
            if (aasDescriptor == null)
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasDescriptor)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTORS);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, aasDescriptor);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IAssetAdministrationShellDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateAssetAdministrationShellRegistrationAsync(string aasIdentifier, IAssetAdministrationShellDescriptor aasDescriptor)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return new Result(new ArgumentNullException(nameof(aasIdentifier)));
            if (aasDescriptor == null)
                return new Result(new ArgumentNullException(nameof(aasDescriptor)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID, aasIdentifier);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, aasDescriptor);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<IAssetAdministrationShellDescriptor>> RetrieveAssetAdministrationShellRegistrationAsync(string aasIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasIdentifier)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID, aasIdentifier);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IAssetAdministrationShellDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>>> RetrieveAllAssetAdministrationShellRegistrationsAsync()
        {
            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTORS);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>>> RetrieveAllAssetAdministrationShellRegistrationsAsync(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            if (predicate == null)
                return new Result<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>>(new ArgumentNullException(nameof(predicate)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTORS);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>>(response, response.Entity);

            if (!result.Success || result.Entity == null)
            {
                response?.Entity?.Dispose();
                return new Result<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>>(result);
            }
            else
            {
                response?.Entity?.Dispose();
                var foundItems = result.Entity.Result.Where(w => predicate.Invoke(w));
                return new Result<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>>(result.Success, 
                    new PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>(foundItems), result.Messages);
            }
        }

        public async Task<IResult> DeleteAssetAdministrationShellRegistrationAsync(string aasIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return new Result(new ArgumentNullException(nameof(aasIdentifier)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID, aasIdentifier);
            var request = base.CreateRequest(uri, HttpMethod.Delete);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelDescriptor>> CreateSubmodelRegistrationAsync(string aasIdentifier, ISubmodelDescriptor submodelDescriptor)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(aasIdentifier)));
            if (submodelDescriptor == null)
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelDescriptor)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTORS, aasIdentifier);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, submodelDescriptor);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateSubmodelRegistrationAsync(string aasIdentifier, string submodelIdentifier, ISubmodelDescriptor submodelDescriptor)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return new Result(new ArgumentNullException(nameof(aasIdentifier)));
            if (string.IsNullOrEmpty(submodelIdentifier))
                return new Result(new ArgumentNullException(nameof(submodelIdentifier)));
            if (submodelDescriptor == null)
                return new Result(new ArgumentNullException(nameof(submodelDescriptor)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTOR_ID, aasIdentifier, submodelIdentifier);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, submodelDescriptor);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<PagedResult<IEnumerable<ISubmodelDescriptor>>>> RetrieveAllSubmodelRegistrationsAsync(string aasIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return new Result<PagedResult<IEnumerable<ISubmodelDescriptor>>>(new ArgumentNullException(nameof(aasIdentifier)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTORS, aasIdentifier);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IEnumerable<ISubmodelDescriptor>>>(response, response.Entity);
            response?.Entity?.Dispose();
            return new Result<PagedResult<IEnumerable<ISubmodelDescriptor>>>(result.Success, result.Entity, result.Messages);
        }

        public async Task<IResult<PagedResult<IEnumerable<ISubmodelDescriptor>>>> RetrieveAllSubmodelRegistrationsAsync(string aasIdentifier, Predicate<ISubmodelDescriptor> predicate)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return new Result<PagedResult<IEnumerable<ISubmodelDescriptor>>>(new ArgumentNullException(nameof(aasIdentifier)));
            if (predicate == null)
                return new Result<PagedResult<IEnumerable<ISubmodelDescriptor>>>(new ArgumentNullException(nameof(predicate)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTORS, aasIdentifier);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IEnumerable<ISubmodelDescriptor>>>(response, response.Entity);

            if (!result.Success || result.Entity == null)
            {
                response?.Entity?.Dispose();
                return new Result<PagedResult<IEnumerable<ISubmodelDescriptor>>>(result);
            }
            else
            {
                response?.Entity?.Dispose();
                var foundItems = result.Entity.Result.Where(w => predicate.Invoke(w));
                return new Result<PagedResult<IEnumerable<ISubmodelDescriptor>>>(result.Success, 
                    new PagedResult<IEnumerable<ISubmodelDescriptor>>(foundItems), result.Messages);
            }          
        }

        public async Task<IResult<ISubmodelDescriptor>> RetrieveSubmodelRegistrationAsync(string aasIdentifier, string submodelIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(aasIdentifier)));
            if (string.IsNullOrEmpty(submodelIdentifier))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelIdentifier)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTOR_ID, aasIdentifier, submodelIdentifier);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> DeleteSubmodelRegistrationAsync(string aasIdentifier, string submodelIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return new Result(new ArgumentNullException(nameof(aasIdentifier)));
            if (string.IsNullOrEmpty(submodelIdentifier))
                return new Result(new ArgumentNullException(nameof(submodelIdentifier)));

            Uri uri = GetPath(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTOR_ID, aasIdentifier, submodelIdentifier);
            var request = base.CreateRequest(uri, HttpMethod.Delete);
            var response = await base.SendRequestAsync(request, CancellationToken.None).ConfigureAwait(false);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion
    }
}
