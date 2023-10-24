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
    public class SubmodelRepositoryHttpClient : SimpleHttpClient, ISubmodelRepositoryClient
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<SubmodelRepositoryHttpClient>();

        public static bool USE_HTTPS = true;

        public IEndpoint Endpoint { get; }

        private SubmodelRepositoryHttpClient(HttpMessageHandler messageHandler) : base(messageHandler)
        {
            var options = new DefaultJsonSerializerOptions();
            var services = DefaultImplementation.GetStandardServiceCollection();
            options.AddDependencyInjection(new DependencyInjectionExtension(services));
            options.AddFullSubmodelElementConverter();
            JsonSerializerOptions = options.Build();
        }

        public SubmodelRepositoryHttpClient(Uri endpoint) : this(endpoint, null)
        { }
        public SubmodelRepositoryHttpClient(Uri endpoint, HttpMessageHandler messageHandler) : this(messageHandler)
        {
            endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            string endpointAddress = endpoint.ToString();
            Endpoint = new Endpoint(endpointAddress.RemoveFromEnd(SubmodelRepositoryRoutes.SUBMODELS), InterfaceName.SubmodelRepositoryInterface);
        }
        public SubmodelRepositoryHttpClient(ISubmodelRepositoryDescriptor submodelRepoDescriptor) : this(submodelRepoDescriptor, null)
        { }

        public SubmodelRepositoryHttpClient(ISubmodelRepositoryDescriptor submodelRepoDescriptor, HttpMessageHandler messageHandler) : this(messageHandler)
        {
            submodelRepoDescriptor = submodelRepoDescriptor ?? throw new ArgumentNullException(nameof(submodelRepoDescriptor));
            IEnumerable<HttpProtocol> httpEndpoints = submodelRepoDescriptor.Endpoints?.OfType<HttpProtocol>();
            HttpProtocol httpEndpoint = null;
            if (USE_HTTPS)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.EndpointProtocol == Uri.UriSchemeHttps);
            if (httpEndpoint == null)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.EndpointProtocol == Uri.UriSchemeHttp);

            if (httpEndpoint == null || string.IsNullOrEmpty(httpEndpoint.EndpointAddress))
                throw new Exception("There is no http endpoint for instantiating a client");
            
            Endpoint = new Endpoint(httpEndpoint.EndpointAddress.RemoveFromEnd(SubmodelRepositoryRoutes.SUBMODELS), InterfaceName.SubmodelRepositoryInterface);
        }

        public Uri GetPath(string requestPath, Identifier id = null, string idShortPath = null)
        {
            string path = Endpoint.ProtocolInformation.EndpointAddress.Trim('/');

            if (string.IsNullOrEmpty(requestPath))
                return new Uri(path);

            if (!string.IsNullOrEmpty(id))
            {
                requestPath = requestPath.Replace("{submodelIdentifier}", id.Id.Base64UrlEncode());
            }

            if (!string.IsNullOrEmpty(idShortPath))
            {
                requestPath = requestPath.Replace("{idShortPath}", idShortPath);
            }

            return new Uri(path + requestPath);
        }

        #region Submodel Repository Interface

        public IResult<ISubmodel> CreateSubmodel(ISubmodel submodel)
        {
            return CreateSubmodelAsync(submodel).GetAwaiter().GetResult();
        }

        public IResult UpdateSubmodel(Identifier id, ISubmodel submodel)
        {
            return UpdateSubmodelAsync(id, submodel).GetAwaiter().GetResult();
        }

        public IResult<PagedResult<IElementContainer<ISubmodel>>> RetrieveSubmodels()
        {
            return RetrieveSubmodelsAsync().GetAwaiter().GetResult();
        }

        public IResult<ISubmodel> RetrieveSubmodel(Identifier id)
        {
            return RetrieveSubmodelAsync(id).GetAwaiter().GetResult();
        }

        public IResult DeleteSubmodel(Identifier id)
        {
            return DeleteSubmodelAsync(id).GetAwaiter().GetResult();
        }

        #endregion

        #region Submodel Repository Client Interface

        public async Task<IResult<ISubmodel>> CreateSubmodelAsync(ISubmodel submodel)
        {
            Uri uri = GetPath(SubmodelRepositoryRoutes.SUBMODELS);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, submodel);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodel>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodel>> RetrieveSubmodelAsync(Identifier id)
        {
            Uri uri = GetPath(SubmodelRepositoryRoutes.SUBMODEL_BYID, id);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodel>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<PagedResult<IElementContainer<ISubmodel>>>> RetrieveSubmodelsAsync()
        {
            Uri uri = GetPath(SubmodelRepositoryRoutes.SUBMODELS);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IElementContainer<ISubmodel>>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateSubmodelAsync(Identifier id, ISubmodel submodel)
        {
            Uri uri = GetPath(SubmodelRepositoryRoutes.SUBMODEL_BYID, id);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, submodel);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> DeleteSubmodelAsync(Identifier id)
        {
            Uri uri = GetPath(SubmodelRepositoryRoutes.SUBMODEL_BYID, id);
            var request = base.CreateRequest(uri, HttpMethod.Delete);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion
    }
}
