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
using BaSyx.Utils.PathHandling;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Connectivity.Descriptors;
using System.Linq;
using BaSyx.Models.Connectivity;
using NLog;
using BaSyx.Models.Communication;
using BaSyx.Utils.DependencyInjection;
using System.Collections.Generic;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using Newtonsoft.Json;
using System.Text;
using System.Threading;

namespace BaSyx.AAS.Client.Http
{
    public class AssetAdministrationShellHttpClient : SimpleHttpClient, 
        IAssetAdministrationShellClient, 
        IAssetAdministrationShellSubmodelClient, 
        ISubmodelRepositoryClient
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        
        public static bool USE_HTTPS = true;

        private const string SEPARATOR = "/";
        private const string AAS = "aas";
        private const string SUBMODELS = "submodels";
        private const string SUBMODEL = "submodel";
        private const string SUBMODEL_ELEMENTS = "submodelElements";
        private const string VALUE = "value";
        private const string INVOKE = "invoke";
        private const string SYNCHRONOUS = "?async=false";
        private const string ASYNCHRONOUS = "?async=true";
        private const string INVOCATION_LIST = "invocationList";

        public Uri Endpoint { get; }

        private AssetAdministrationShellHttpClient(HttpMessageHandler messageHandler) : base(messageHandler)
        {
            JsonSerializerSettings = new DependencyInjectionJsonSerializerSettings();
        }

        public AssetAdministrationShellHttpClient(Uri endpoint) : this(endpoint, null)
        { }
        public AssetAdministrationShellHttpClient(Uri endpoint, HttpMessageHandler messageHandler) : this(messageHandler)
        {           
            endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            if (!endpoint.AbsolutePath.EndsWith(AAS))
                Endpoint = new Uri(endpoint, AAS);
            else
                Endpoint = endpoint;
        }
        public AssetAdministrationShellHttpClient(IAssetAdministrationShellDescriptor aasDescriptor) : this(aasDescriptor, null)
        { }

        public AssetAdministrationShellHttpClient(IAssetAdministrationShellDescriptor aasDescriptor, HttpMessageHandler messageHandler) : this(messageHandler)
        {
            aasDescriptor = aasDescriptor ?? throw new ArgumentNullException(nameof(aasDescriptor));
            IEnumerable<HttpEndpoint> httpEndpoints = aasDescriptor.Endpoints?.OfType<HttpEndpoint>();
            HttpEndpoint httpEndpoint = null;
            if (USE_HTTPS)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.Type == Uri.UriSchemeHttps);
            if (httpEndpoint == null)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.Type == Uri.UriSchemeHttp);

            if (httpEndpoint == null || string.IsNullOrEmpty(httpEndpoint.Address))
                throw new Exception("There is no http endpoint for instantiating a client");
            else
            {
                if (!httpEndpoint.Address.EndsWith(SEPARATOR + AAS) && !httpEndpoint.Address.EndsWith(SEPARATOR + AAS + SEPARATOR))
                    Endpoint = new Uri(httpEndpoint.Address + SEPARATOR + AAS);
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

        public IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellDescriptor()
        {
            var request = base.CreateRequest(GetUri(), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<IAssetAdministrationShellDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell()
        {
            var request = base.CreateRequest(GetUri(), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<IAssetAdministrationShell>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<ISubmodel> CreateOrUpdateSubmodel(ISubmodel submodel)
        {
            var request = base.CreateJsonContentRequest(GetUri(SUBMODELS, submodel.IdShort), HttpMethod.Put, submodel);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ISubmodel>(response, response.Entity); 
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<IElementContainer<ISubmodel>> RetrieveSubmodels()
        {
            var request = base.CreateRequest(GetUri(SUBMODELS), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ElementContainer<ISubmodel>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<ISubmodel> RetrieveSubmodel(string submodelId)
        {
            var request = base.CreateRequest(GetUri(SUBMODELS, submodelId), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ISubmodel>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult DeleteSubmodel(string submodelId)
        {
            var request = base.CreateRequest(GetUri(SUBMODELS, submodelId), HttpMethod.Delete);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }
      
        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements(string submodelId)
        {
            var request = base.CreateRequest(GetUri(SUBMODELS, submodelId, SUBMODEL, SUBMODEL_ELEMENTS), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ElementContainer<ISubmodelElement>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string submodelId, string seIdShortPath)
        {
            var request = base.CreateRequest(GetUri(SUBMODELS, submodelId, SUBMODEL, SUBMODEL_ELEMENTS, seIdShortPath), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string submodelId, string rootSeIdShortPath, ISubmodelElement submodelElement)
        {
            var request = base.CreateJsonContentRequest(GetUri(SUBMODELS, submodelId, SUBMODEL, SUBMODEL_ELEMENTS, rootSeIdShortPath), HttpMethod.Put, submodelElement);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult UpdateSubmodelElementValue(string submodelId, string seIdShortPath, IValue value)
        {
            var request = base.CreateJsonContentRequest(GetUri(SUBMODELS, submodelId, SUBMODEL, SUBMODEL_ELEMENTS, seIdShortPath, VALUE), HttpMethod.Put, value.Value);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<IValue> RetrieveSubmodelElementValue(string submodelId, string seIdShortPath)
        {
            var request = base.CreateRequest(GetUri(SUBMODELS, submodelId, SUBMODEL, SUBMODEL_ELEMENTS, seIdShortPath, VALUE), HttpMethod.Get);
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

        public IResult DeleteSubmodelElement(string submodelId, string seIdShortPath)
        {
            var request = base.CreateRequest(GetUri(SUBMODELS, submodelId, SUBMODEL, SUBMODEL_ELEMENTS, seIdShortPath), HttpMethod.Delete);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<InvocationResponse> InvokeOperation(string submodelId, string operationIdShortPath, InvocationRequest invocationRequest)
        {
            var request = base.CreateJsonContentRequest(GetUri(SUBMODELS, submodelId, SUBMODEL, SUBMODEL_ELEMENTS, operationIdShortPath, INVOKE + SYNCHRONOUS), HttpMethod.Post, invocationRequest);

            TimeSpan timeout = request.GetTimeout() ?? GetDefaultTimeout();
            if (invocationRequest.Timeout.HasValue && invocationRequest.Timeout.Value > timeout.TotalMilliseconds)
                request.SetTimeout(TimeSpan.FromMilliseconds(invocationRequest.Timeout.Value));

            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }
        public IResult<CallbackResponse> InvokeOperationAsync(string submodelId, string operationIdShortPath, InvocationRequest invocationRequest)
        {
            var request = base.CreateJsonContentRequest(GetUri(SUBMODELS, submodelId, SUBMODEL, SUBMODEL_ELEMENTS, operationIdShortPath, INVOKE + ASYNCHRONOUS), HttpMethod.Post, invocationRequest);

            TimeSpan timeout = request.GetTimeout() ?? GetDefaultTimeout();
            if (invocationRequest.Timeout.HasValue && invocationRequest.Timeout.Value > timeout.TotalMilliseconds)
                request.SetTimeout(TimeSpan.FromMilliseconds(invocationRequest.Timeout.Value));

            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<CallbackResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public IResult<InvocationResponse> GetInvocationResult(string submodelId, string operationIdShortPath, string requestId)
        {
            var request = base.CreateRequest(GetUri(SUBMODELS, submodelId, SUBMODEL, SUBMODEL_ELEMENTS, operationIdShortPath, INVOCATION_LIST, requestId), HttpMethod.Get);
            var response = base.SendRequest(request, CancellationToken.None);
            var result = base.EvaluateResponse<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }       
    }
}
