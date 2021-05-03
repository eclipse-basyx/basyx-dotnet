/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.API.Clients;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.Client.Http;
using BaSyx.Utils.ResultHandling;
using System;
using System.Net.Http;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Communication;
using BaSyx.Utils.PathHandling;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Connectivity;
using System.Linq;
using BaSyx.Utils.DependencyInjection;
using NLog;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using System.Threading;

namespace BaSyx.Submodel.Client.Http
{
    public class SubmodelHttpClient : SimpleHttpClient, ISubmodelClient
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public static bool USE_HTTPS = true;

        private const string SEPARATOR = "/";
        private const string SUBMODEL = "submodel";
        private const string SUBMODEL_ELEMENTS = "submodelElements";
        private const string VALUE = "value";
        private const string INVOKE = "invoke";
        private const string SYNCHRONOUS = "?async=false";
        private const string ASYNCHRONOUS = "?async=true";
        private const string INVOCATION_LIST = "invocationList";

        public Uri Endpoint { get; }

        private SubmodelHttpClient(HttpMessageHandler messageHandler) : base(messageHandler)
        {
            JsonSerializerSettings = new DependencyInjectionJsonSerializerSettings();
        }

        public SubmodelHttpClient(Uri endpoint) : this(endpoint, null)
        { }
        public SubmodelHttpClient(Uri endpoint, HttpMessageHandler messageHandler) : this(messageHandler)
        {
            endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            if (!endpoint.AbsolutePath.EndsWith(SUBMODEL))
                Endpoint = new Uri(endpoint, SUBMODEL);
            else
                Endpoint = endpoint;
        }
        public SubmodelHttpClient(ISubmodelDescriptor submodelDescriptor) : this(submodelDescriptor, null)
        { }

        public SubmodelHttpClient(ISubmodelDescriptor submodelDescriptor, HttpMessageHandler messageHandler) : this(messageHandler)
        {
            submodelDescriptor = submodelDescriptor ?? throw new ArgumentNullException(nameof(submodelDescriptor));
            IEnumerable<HttpEndpoint> httpEndpoints = submodelDescriptor.Endpoints?.OfType<HttpEndpoint>();
            HttpEndpoint httpEndpoint = null;
            if (USE_HTTPS)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.Type == Uri.UriSchemeHttps);
            if (httpEndpoint == null)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.Type == Uri.UriSchemeHttp);

            if (httpEndpoint == null || string.IsNullOrEmpty(httpEndpoint.Address))
                throw new Exception("There is no http endpoint for instantiating a client");
            else
            {
                if (!httpEndpoint.Address.EndsWith(SEPARATOR + SUBMODEL) && !httpEndpoint.Address.EndsWith(SEPARATOR + SUBMODEL + SEPARATOR))
                    Endpoint = new Uri(httpEndpoint.Address + SEPARATOR + SUBMODEL);
                else
                    Endpoint = new Uri(httpEndpoint.Address);
            }
        }

        public Uri GetUri(params string[] pathElements)
        {
            if (pathElements == null)
                return Endpoint;
            return Endpoint.Append(pathElements);
        }

        public IResult<ISubmodel> RetrieveSubmodel()
        {
            var request = base.CreateRequest(GetUri(), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ISubmodel>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }       
   
        public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string rootSubmodelElementIdShort, ISubmodelElement submodelElement)
        {
            var request = base.CreateJsonContentRequest(GetUri(SUBMODEL_ELEMENTS, rootSubmodelElementIdShort), HttpMethod.Put, submodelElement);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
        {
            var request = base.CreateRequest(GetUri(SUBMODEL_ELEMENTS), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<IElementContainer<ISubmodelElement>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string submodelElementId)
        {
            var request = base.CreateRequest(GetUri(SUBMODEL_ELEMENTS, submodelElementId), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<IValue> RetrieveSubmodelElementValue(string submodelElementId)
        {
            var request = base.CreateRequest(GetUri(SUBMODEL_ELEMENTS, submodelElementId, VALUE), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            IResult result = base.EvaluateResponse(response, response.Entity);
            if (result.Success && result.Entity != null)
            {
                string sValue = Encoding.UTF8.GetString((byte[])result.Entity);
                object deserializedValue = JsonConvert.DeserializeObject(sValue);
                response?.Entity?.Dispose();
                return new Result<IValue>(result.Success, new ElementValue(deserializedValue, deserializedValue.GetType()), result.Messages);
            }
            else
            {
                response?.Entity?.Dispose();
                return new Result<IValue>(result);
            }
        }      

        public IResult DeleteSubmodelElement(string submodelElementId)
        {
            var request = base.CreateRequest(GetUri(SUBMODEL_ELEMENTS, submodelElementId), HttpMethod.Delete);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<InvocationResponse> InvokeOperation(string operationId, InvocationRequest invocationRequest)
        {
            var request = base.CreateJsonContentRequest(GetUri(SUBMODEL_ELEMENTS, operationId, INVOKE + SYNCHRONOUS), HttpMethod.Post, invocationRequest);

            TimeSpan timeout = request.GetTimeout() ?? GetDefaultTimeout();
            if (invocationRequest.Timeout.HasValue && invocationRequest.Timeout.Value > timeout.TotalMilliseconds)
                request.SetTimeout(TimeSpan.FromMilliseconds(invocationRequest.Timeout.Value));

            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<CallbackResponse> InvokeOperationAsync(string operationId, InvocationRequest invocationRequest)
        {
            var request = base.CreateJsonContentRequest(GetUri(SUBMODEL_ELEMENTS, operationId, INVOKE + ASYNCHRONOUS), HttpMethod.Post, invocationRequest);

            TimeSpan timeout = request.GetTimeout() ?? GetDefaultTimeout();
            if (invocationRequest.Timeout.HasValue && invocationRequest.Timeout.Value > timeout.TotalMilliseconds)
                request.SetTimeout(TimeSpan.FromMilliseconds(invocationRequest.Timeout.Value));

            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<CallbackResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<InvocationResponse> GetInvocationResult(string operationId, string requestId)
        {
            var request = base.CreateRequest(GetUri(SUBMODEL_ELEMENTS, operationId, INVOCATION_LIST, requestId), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult UpdateSubmodelElementValue(string submodelElementId, IValue value)
        {
            var request = base.CreateJsonContentRequest(GetUri(SUBMODEL_ELEMENTS, submodelElementId, VALUE), HttpMethod.Put, value.Value);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }
    }
}
