/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.API.Clients;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.Client.Http;
using BaSyx.Utils.ResultHandling;
using System;
using System.Net.Http;
using BaSyx.Models.Connectivity;
using System.Linq;
using BaSyx.Utils.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using BaSyx.API.Http;
using System.Threading.Tasks;
using BaSyx.Utils.Extensions;
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace BaSyx.Clients.AdminShell.Http
{
    public class AssetAdministrationShellRepositoryHttpClient : SimpleHttpClient, IAssetAdministrationShellRepositoryClient
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<AssetAdministrationShellRepositoryHttpClient>();

        public static bool USE_HTTPS = true;

        public IEndpoint Endpoint { get; }

        private AssetAdministrationShellRepositoryHttpClient(HttpMessageHandler messageHandler) : base(messageHandler)
        {
            var options = new DefaultJsonSerializerOptions();
            var services = DefaultImplementation.GetStandardServiceCollection();
            options.AddDependencyInjection(new DependencyInjectionExtension(services));
            options.AddFullSubmodelElementConverter();
            JsonSerializerOptions = options.Build();
        }

        public AssetAdministrationShellRepositoryHttpClient(Uri endpoint) : this(endpoint, null)
        { }
        public AssetAdministrationShellRepositoryHttpClient(Uri endpoint, HttpMessageHandler messageHandler) : this(messageHandler)
        {
            endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            string endpointAddress = endpoint.ToString();
            Endpoint = new Endpoint(endpointAddress.RemoveFromEnd(AssetAdministrationShellRepositoryRoutes.SHELLS), InterfaceName.AssetAdministrationShellInterface);
        }
        public AssetAdministrationShellRepositoryHttpClient(IAssetAdministrationShellRepositoryDescriptor aasRepoDescriptor) : this(aasRepoDescriptor, null)
        { }

        public AssetAdministrationShellRepositoryHttpClient(IAssetAdministrationShellRepositoryDescriptor aasRepoDescriptor, HttpMessageHandler messageHandler) : this(messageHandler)
        {
            aasRepoDescriptor = aasRepoDescriptor ?? throw new ArgumentNullException(nameof(aasRepoDescriptor));
            IEnumerable<HttpProtocol> httpEndpoints = aasRepoDescriptor.Endpoints?.OfType<HttpProtocol>();
            HttpProtocol httpEndpoint = null;
            if (USE_HTTPS)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.EndpointProtocol == Uri.UriSchemeHttps);
            if (httpEndpoint == null)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.EndpointProtocol == Uri.UriSchemeHttp);

            if (httpEndpoint == null || string.IsNullOrEmpty(httpEndpoint.EndpointAddress))
                throw new Exception("There is no http endpoint for instantiating a client");
            
            Endpoint = new Endpoint(httpEndpoint.EndpointAddress.RemoveFromEnd(AssetAdministrationShellRepositoryRoutes.SHELLS), InterfaceName.AssetAdministrationShellRepositoryInterface);
        }

        public Uri GetPath(string requestPath, string aasIdentifier = null, string submodelIdentifier = null, string idShortPath = null)
        {
            string path = Endpoint.ProtocolInformation.EndpointAddress.Trim('/');

            if (string.IsNullOrEmpty(requestPath))
                return new Uri(path);

            if (!string.IsNullOrEmpty(aasIdentifier))
            {
                requestPath = requestPath.Replace("{aasIdentifier}", aasIdentifier.Base64UrlEncode());
            }

            if (!string.IsNullOrEmpty(submodelIdentifier))
            {
                requestPath = requestPath.Replace("{submodelIdentifier}", submodelIdentifier.Base64UrlEncode());
            }

            if (!string.IsNullOrEmpty(idShortPath))
            {
                requestPath = requestPath.Replace("{idShortPath}", idShortPath);
            }

            return new Uri(path + requestPath);
        }

        #region Asset Administration Shell Repository Interface

        public IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            return CreateAssetAdministrationShellAsync(aas).GetAwaiter().GetResult();
        }

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(Identifier id)
        {
            return RetrieveAssetAdministrationShellAsync(id).GetAwaiter().GetResult();
        }

        public IResult<PagedResult<IElementContainer<IAssetAdministrationShell>>> RetrieveAssetAdministrationShells()
        {
            return RetrieveAssetAdministrationShellsAsync().GetAwaiter().GetResult();
        }

        public IResult UpdateAssetAdministrationShell(Identifier id, IAssetAdministrationShell aas)
        {
            return UpdateAssetAdministrationShellAsync(id, aas).GetAwaiter().GetResult();
        }

        public IResult DeleteAssetAdministrationShell(Identifier id)
        {
            return DeleteAssetAdministrationShellAsync(id).GetAwaiter().GetResult();
        }

        #endregion

        #region Asset Administration Shell Repository Client Interface

        public async Task<IResult<IAssetAdministrationShell>> CreateAssetAdministrationShellAsync(IAssetAdministrationShell aas)
        {
            Uri uri = GetPath(AssetAdministrationShellRepositoryRoutes.SHELLS);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, aas);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IAssetAdministrationShell>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<IAssetAdministrationShell>> RetrieveAssetAdministrationShellAsync(Identifier id)
        {
            Uri uri = GetPath(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS, id);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IAssetAdministrationShell>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<PagedResult<IElementContainer<IAssetAdministrationShell>>>> RetrieveAssetAdministrationShellsAsync()
        {
            Uri uri = GetPath(AssetAdministrationShellRepositoryRoutes.SHELLS);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IElementContainer<IAssetAdministrationShell>>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateAssetAdministrationShellAsync(Identifier id, IAssetAdministrationShell aas)
        {
            Uri uri = GetPath(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS, id);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, aas);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> DeleteAssetAdministrationShellAsync(Identifier id)
        {
            Uri uri = GetPath(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS, id);
            var request = base.CreateRequest(uri, HttpMethod.Delete);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion
    }
}
