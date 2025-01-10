/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
using System.Collections.Generic;
using System.Net.Http;
using BaSyx.Models.Connectivity;
using System.Linq;
using System.Text.Json;
using BaSyx.Utils.DependencyInjection;
using System.Threading;
using Microsoft.Extensions.Logging;
using BaSyx.API.Http;
using System.Threading.Tasks;
using BaSyx.Utils.Extensions;
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling.ResultTypes;
using System.Web;

namespace BaSyx.Clients.AdminShell.Http
{
    public class AssetAdministrationShellRepositoryHttpClient : SimpleHttpClient, IAssetAdministrationShellRepositoryClient
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<AssetAdministrationShellRepositoryHttpClient>();

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
            Endpoint = new Endpoint(endpointAddress.RemoveFromEnd(AssetAdministrationShellRepositoryRoutes.SHELLS), 
                InterfaceName.AssetAdministrationShellRepositoryInterface);
        }
        public AssetAdministrationShellRepositoryHttpClient(IAssetAdministrationShellRepositoryDescriptor aasRepoDescriptor, bool preferHttps = true) : this(aasRepoDescriptor, null, preferHttps)
        { }

        public AssetAdministrationShellRepositoryHttpClient(IAssetAdministrationShellRepositoryDescriptor aasRepoDescriptor, HttpMessageHandler messageHandler, bool preferHttps = true) : this(messageHandler)
        {
            aasRepoDescriptor = aasRepoDescriptor ?? throw new ArgumentNullException(nameof(aasRepoDescriptor));
            IEndpoint httpEndpoint = null;
            if (preferHttps)
                httpEndpoint = aasRepoDescriptor.Endpoints?.FirstOrDefault(p => p.ProtocolInformation?.EndpointProtocol == Uri.UriSchemeHttps);
            if (httpEndpoint == null)
                httpEndpoint = aasRepoDescriptor.Endpoints?.FirstOrDefault(p => p.ProtocolInformation?.EndpointProtocol == Uri.UriSchemeHttp);

            if (httpEndpoint == null || string.IsNullOrEmpty(httpEndpoint.ProtocolInformation?.EndpointAddress))
                throw new Exception("There is no http endpoint for instantiating a client");

            Endpoint = new Endpoint(httpEndpoint.ProtocolInformation.EndpointAddress.RemoveFromEnd(AssetAdministrationShellRepositoryRoutes.SHELLS),
                InterfaceName.AssetAdministrationShellRepositoryInterface);
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

        public IResult<PagedResult<IElementContainer<IAssetAdministrationShell>>> RetrieveAssetAdministrationShells(int limit = 100, string cursor = "", string assetIds = null, string idShort = "")
        {
            return RetrieveAssetAdministrationShellsAsync(limit, cursor, assetIds, idShort).GetAwaiter().GetResult();
        }

        public IResult<PagedResult<IEnumerable<IReference<IAssetAdministrationShell>>>> RetrieveAssetAdministrationShellsReference(int limit = 100, string cursor = "", string assetIds = null, string idShort = "")
        {
            return RetrieveAssetAdministrationShellsReferenceAsync(limit, cursor).GetAwaiter().GetResult();
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

        public async Task<IResult<PagedResult<IElementContainer<IAssetAdministrationShell>>>> RetrieveAssetAdministrationShellsAsync(int limit = 100, string cursor = "", string assetIds = "", string idShort = "")
        {
            Uri uri = GetPath(AssetAdministrationShellRepositoryRoutes.SHELLS);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["limit"] = limit.ToString();
            query["cursor"] = cursor;
            query["assetIds"] = assetIds;
            query["idShort"] = idShort;
            var uriBuilder = new UriBuilder(uri) { Query = query.ToString() };
            uri = uriBuilder.Uri;

            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IElementContainer<IAssetAdministrationShell>>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<PagedResult<IEnumerable<IReference<IAssetAdministrationShell>>>>> RetrieveAssetAdministrationShellsReferenceAsync(int limit = 100, string cursor = "")
        {
            Uri uri = GetPath(AssetAdministrationShellRepositoryRoutes.SHELLS + OutputModifier.REFERENCE);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["limit"] = limit.ToString();
            query["cursor"] = cursor;
            var uriBuilder = new UriBuilder(uri) { Query = query.ToString() };
            uri = uriBuilder.Uri;

            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IEnumerable<IReference<IAssetAdministrationShell>>>>(response, response.Entity);
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
