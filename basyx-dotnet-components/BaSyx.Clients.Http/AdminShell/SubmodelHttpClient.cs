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

        public static bool USE_HTTPS = true;

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
        public SubmodelHttpClient(ISubmodelDescriptor submodelDescriptor, bool standalone = true) : this(submodelDescriptor, null, standalone)
        { }

        public SubmodelHttpClient(ISubmodelDescriptor submodelDescriptor, HttpMessageHandler messageHandler, bool standalone = true) : this(messageHandler, standalone)
        {
            submodelDescriptor = submodelDescriptor ?? throw new ArgumentNullException(nameof(submodelDescriptor));
            IEnumerable<HttpProtocol> httpEndpoints = submodelDescriptor.Endpoints?.OfType<HttpProtocol>();
            HttpProtocol httpEndpoint = null;
            if (USE_HTTPS)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.EndpointProtocol == Uri.UriSchemeHttps);
            if (httpEndpoint == null)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.EndpointProtocol == Uri.UriSchemeHttp);

            if (httpEndpoint == null || string.IsNullOrEmpty(httpEndpoint.EndpointAddress))
                throw new Exception("There is no http endpoint for instantiating a client");
            
            Endpoint = new Endpoint(httpEndpoint.EndpointAddress.RemoveFromEnd(SubmodelRoutes.SUBMODEL), InterfaceName.SubmodelInterface);
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

        public IResult<ISubmodelElement> UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return UpdateSubmodelElementAsync(rootIdShortPath, submodelElement).GetAwaiter().GetResult();
        }

        public IResult<PagedResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElements()
        {
            return RetrieveSubmodelElementsAsync().GetAwaiter().GetResult();
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
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodel>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateSubmodelAsync(ISubmodel submodel)
        {
            Uri uri = GetPath();
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, submodel);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
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

            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, submodelElement);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelElement>> UpdateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, rootIdShortPath);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, submodelElement);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<PagedResult<IElementContainer<ISubmodelElement>>>> RetrieveSubmodelElementsAsync()
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IElementContainer<ISubmodelElement>>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelElement>> RetrieveSubmodelElementAsync(string idShortPath)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ValueScope>> RetrieveSubmodelElementValueAsync(string idShortPath)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ValueScope>(response, response.Entity);
            response?.Entity?.Dispose();  
            return result;
        }

        public async Task<IResult> UpdateSubmodelElementValueAsync(string idShortPath, ValueScope value)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, idShortPath);
            var request = base.CreateJsonContentRequest(uri, new HttpMethod("PATCH"), value);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> DeleteSubmodelElementAsync(string idShortPath)
        {
            Uri uri = GetPath(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Delete);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<InvocationResponse>> InvokeOperationAsync(string idShortPath, InvocationRequest invocationRequest, bool async = false)
        {
            string path = SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE;
            if (async)
                path = SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC;

            Uri uri = GetPath(path, idShortPath);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, invocationRequest);

            TimeSpan timeout = request.GetTimeout() ?? GetDefaultTimeout();
            if (invocationRequest.Timeout.HasValue && invocationRequest.Timeout.Value > timeout.TotalMilliseconds)
                request.SetTimeout(TimeSpan.FromMilliseconds(invocationRequest.Timeout.Value));

            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<InvocationResponse>> GetInvocationResultAsync(string idShortPath, string requestId)
        {
            string path = SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS.Replace("{handleId}", requestId);
            Uri uri = GetPath(path, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion
    }
}
