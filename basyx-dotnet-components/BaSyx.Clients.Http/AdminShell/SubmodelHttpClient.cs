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
    public class SubmodelHttpClient : SimpleHttpClient, ISubmodelClient
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<SubmodelHttpClient>();

        public IEndpoint Endpoint { get; }

        private bool _standalone;

        private SubmodelHttpClient(HttpMessageHandler messageHandler, bool standalone = true) : base(messageHandler)
        {
            _standalone = standalone;
            var options = new DefaultJsonSerializerOptions();
            var services = DefaultImplementation.GetStandardServiceCollection();
            options.AddDependencyInjection(new DependencyInjectionExtension(services));
            options.AddFullSubmodelElementConverter();
            JsonSerializerOptions = options.Build();         
        }

        public SubmodelHttpClient(Uri endpoint, bool standalone = true) : this(endpoint, null, standalone)
        { }
        public SubmodelHttpClient(Uri endpoint, HttpMessageHandler messageHandler, bool standalone = true) : this(messageHandler, standalone)
        {
            endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            string endpointAddress = endpoint.ToString();
            Endpoint = new Endpoint(endpointAddress.RemoveFromEnd(SubmodelRoutes.SUBMODEL), InterfaceName.SubmodelInterface);
        }
        public SubmodelHttpClient(ISubmodelDescriptor submodelDescriptor, bool standalone = true, bool preferHttps = true) : this(submodelDescriptor, null, standalone, preferHttps)
        { }

        public SubmodelHttpClient(ISubmodelDescriptor submodelDescriptor, HttpMessageHandler messageHandler, bool standalone = true, bool preferHttps = true) : this(messageHandler, standalone)
        {
            submodelDescriptor = submodelDescriptor ?? throw new ArgumentNullException(nameof(submodelDescriptor));
            IEndpoint httpEndpoint = null;
            if (preferHttps)
                httpEndpoint = submodelDescriptor.Endpoints?.FirstOrDefault(p => p.ProtocolInformation?.EndpointProtocol == Uri.UriSchemeHttps);
            if (httpEndpoint == null)
                httpEndpoint = submodelDescriptor.Endpoints?.FirstOrDefault(p => p.ProtocolInformation?.EndpointProtocol == Uri.UriSchemeHttp);

            if (httpEndpoint == null || string.IsNullOrEmpty(httpEndpoint.ProtocolInformation?.EndpointAddress))
                throw new Exception("There is no http endpoint for instantiating a client");

            Endpoint = new Endpoint(httpEndpoint.ProtocolInformation.EndpointAddress.RemoveFromEnd(SubmodelRoutes.SUBMODEL),
                InterfaceName.SubmodelInterface);           
        }

        public Uri GetPath(string requestPath = null, string idShortPath = null)
        {
            string path = Endpoint.ProtocolInformation.EndpointAddress.Trim('/');

            if (_standalone)
                path += SubmodelRoutes.SUBMODEL;

            if (string.IsNullOrEmpty(requestPath))
                return new Uri(path);

            if (!string.IsNullOrEmpty(idShortPath))
            {
                requestPath = requestPath.Replace("{idShortPath}", idShortPath);
            }

            return new Uri(path + requestPath);
        }

        public IResult<ISubmodel> RetrieveSubmodel(RequestLevel level = default, RequestExtent extent = default)
        {
            return RetrieveSubmodelAsync(level, extent).GetAwaiter().GetResult();
        }

        public IResult UpdateSubmodel(ISubmodel submodel)
        {
            return UpdateSubmodelAsync(submodel).GetAwaiter().GetResult();
        }

        public IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return CreateSubmodelElementAsync(rootIdShortPath, submodelElement).GetAwaiter().GetResult();
        }

        public IResult UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return UpdateSubmodelElementAsync(rootIdShortPath, submodelElement).GetAwaiter().GetResult();
        }

        public IResult<PagedResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElements(int limit = 100, string cursor = "", RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            return RetrieveSubmodelElementsAsync(limit, cursor, level, extent).GetAwaiter().GetResult();
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string idShortPath)
        {
            return RetrieveSubmodelElementAsync(idShortPath).GetAwaiter().GetResult();
        }

        public IResult<ValueScope> RetrieveSubmodelElementValue(string idShortPath)
        {
            return RetrieveSubmodelElementValueAsync(idShortPath).GetAwaiter().GetResult();
        }      

        public IResult DeleteSubmodelElement(string idShortPath)
        {
            return DeleteSubmodelElementAsync(idShortPath).GetAwaiter().GetResult();
        }

        public IResult<InvocationResponse> InvokeOperation(string idShortPath, InvocationRequest invocationRequest, bool async)
        {
            return InvokeOperationAsync(idShortPath, invocationRequest, async).GetAwaiter().GetResult();
        }

        public IResult<InvocationResponse> GetInvocationResult(string idShortPath, string requestId)
        {
            return GetInvocationResultAsync(idShortPath, requestId).GetAwaiter().GetResult();
        }

        public IResult UpdateSubmodelElementValue(string idShortPath, ValueScope value)
        {
            return UpdateSubmodelElementValueAsync(idShortPath, value).GetAwaiter().GetResult();
        }

        #region Asynchronous implementation 

        public async Task<IResult<ISubmodel>> RetrieveSubmodelAsync(RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            Uri uri = GetPath();
            var request = CreateRequest(uri, HttpMethod.Get);
            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync<ISubmodel>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateSubmodelAsync(ISubmodel submodel)
        {
            Uri uri = GetPath();
            var request = CreateJsonContentRequest(uri, HttpMethod.Put, submodel);
            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(ISubmodelElement submodelElement)
            => await CreateSubmodelElementAsync(".", submodelElement).ConfigureAwait(false);

        public async Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            Uri uri;
            if (rootIdShortPath == ".")
                uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS);
            else
                uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS, rootIdShortPath);

            var request = CreateJsonContentRequest(uri, HttpMethod.Post, submodelElement);
            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, rootIdShortPath);
            var request = CreateJsonContentRequest(uri, HttpMethod.Put, submodelElement);
            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<PagedResult<IElementContainer<ISubmodelElement>>>> RetrieveSubmodelElementsAsync(int limit = 100, string cursor = "", RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS);
            var request = CreateRequest(uri, HttpMethod.Get);
            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync<PagedResult<IElementContainer<ISubmodelElement>>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelElement>> RetrieveSubmodelElementAsync(string idShortPath)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, idShortPath);
            var request = CreateRequest(uri, HttpMethod.Get);
            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ValueScope>> RetrieveSubmodelElementValueAsync(string idShortPath)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, idShortPath);
            var request = CreateRequest(uri, HttpMethod.Get);
            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync<ValueScope>(response, response.Entity);
            response?.Entity?.Dispose();  
            return result;
        }

        public async Task<IResult> UpdateSubmodelElementValueAsync(string idShortPath, ValueScope value)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, idShortPath);
            var request = CreateJsonContentRequest(uri, new HttpMethod("PATCH"), value);
            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> DeleteSubmodelElementAsync(string idShortPath)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, idShortPath);
            var request = CreateRequest(uri, HttpMethod.Delete);
            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public virtual async Task<IResult<InvocationResponse>> InvokeOperationAsync(string idShortPath, InvocationRequest invocationRequest, bool async = false)
        {
            string path = SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE;
            if (async)
                path = SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC;

            Uri uri = GetPath(path, idShortPath);
            var request = CreateJsonContentRequest(uri, HttpMethod.Post, invocationRequest);

            TimeSpan timeout = request.GetTimeout() ?? GetDefaultTimeout();
            if (invocationRequest.Timeout.HasValue && invocationRequest.Timeout.Value > timeout.TotalMilliseconds)
                request.SetTimeout(TimeSpan.FromMilliseconds(invocationRequest.Timeout.Value));

            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<InvocationResponse>> GetInvocationResultAsync(string idShortPath, string requestId)
        {
            string path = SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS.Replace("{handleId}", requestId);
            Uri uri = GetPath(path, idShortPath);
            var request = CreateRequest(uri, HttpMethod.Get);
            var response = await SendRequestAsync(request, CancellationToken.None);
            var result = await EvaluateResponseAsync<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion
    }
}
