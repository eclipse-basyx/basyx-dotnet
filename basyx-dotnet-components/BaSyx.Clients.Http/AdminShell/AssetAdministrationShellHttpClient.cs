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
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BaSyx.Utils.Extensions;
using BaSyx.API.Http;
using BaSyx.Clients.AdminShell.Http;

namespace BaSyx.Clients.AdminShell.Http
{
    public class AssetAdministrationShellHttpClient : SimpleHttpClient, 
        IAssetAdministrationShellClient
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<AssetAdministrationShellHttpClient>();
        
        public static bool USE_HTTPS = true;

        public IEndpoint Endpoint { get; }

        private AssetAdministrationShellHttpClient(HttpMessageHandler messageHandler) : base(messageHandler)
        {
            JsonSerializerSettings = new DependencyInjectionJsonSerializerSettings();
        }

        public AssetAdministrationShellHttpClient(Uri endpoint) : this(endpoint, null)
        { }
        public AssetAdministrationShellHttpClient(Uri endpoint, HttpMessageHandler messageHandler) : this(messageHandler)
        {
            endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            string endpointAddress = endpoint.ToString();
            Endpoint = new Endpoint(endpointAddress.RemoveFromEnd(AssetAdministrationShellRoutes.AAS), InterfaceName.AssetAdministrationShellInterface);
        }
        public AssetAdministrationShellHttpClient(IAssetAdministrationShellDescriptor aasDescriptor) : this(aasDescriptor, null)
        { }

        public AssetAdministrationShellHttpClient(IAssetAdministrationShellDescriptor aasDescriptor, HttpMessageHandler messageHandler) : this(messageHandler)
        {
            aasDescriptor = aasDescriptor ?? throw new ArgumentNullException(nameof(aasDescriptor));
            IEnumerable<HttpProtocol> httpEndpoints = aasDescriptor.Endpoints?.OfType<HttpProtocol>();
            HttpProtocol httpEndpoint = null;
            if (USE_HTTPS)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.EndpointProtocol == Uri.UriSchemeHttps);
            if (httpEndpoint == null)
                httpEndpoint = httpEndpoints?.FirstOrDefault(p => p.EndpointProtocol == Uri.UriSchemeHttp);

            if (httpEndpoint == null || string.IsNullOrEmpty(httpEndpoint.EndpointAddress))
                throw new Exception("There is no http endpoint for instantiating a client");

            Endpoint = new Endpoint(httpEndpoint.EndpointAddress.RemoveFromEnd(AssetAdministrationShellRoutes.AAS), InterfaceName.AssetAdministrationShellInterface);
        }

        public Uri GetPath(string requestPath, string submodelIdentifier = null, string idShortPath = null, RequestContent content = default)
        {
            string path = Endpoint.ProtocolInformation.EndpointAddress.Trim('/');

            if (string.IsNullOrEmpty(requestPath))
                return new Uri(path);

            if (!string.IsNullOrEmpty(submodelIdentifier))
            {
                requestPath = requestPath.Replace("{submodelIdentifier}", submodelIdentifier.Base64UrlEncode());
            }

            if (!string.IsNullOrEmpty(idShortPath))
            {
                if (content == RequestContent.Value)
                    requestPath = requestPath.Replace("{idShortPath}", idShortPath) + "?content=value";
                else
                    requestPath = requestPath.Replace("{idShortPath}", idShortPath);
            }

            return new Uri(path + requestPath);
        }


        #region Descriptor Interface

        public IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellDescriptor()
        {
            return RetrieveAssetAdministrationShellDescriptorAsync().GetAwaiter().GetResult();
        }

        public async Task<IResult<IAssetAdministrationShellDescriptor>> RetrieveAssetAdministrationShellDescriptorAsync()
        {
            Uri uri = GetPath(DescriptorRoutes.DESCRIPTOR);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IAssetAdministrationShellDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion

        #region Asset Administration Shell Interface

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(RequestContent content)
        {
            return RetrieveAssetAdministrationShellAsync(content).GetAwaiter().GetResult();
        }

        public IResult UpdateAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            return UpdateAssetAdministrationShellAsync(aas).GetAwaiter().GetResult();
        }

        public IResult<IAssetInformation> RetrieveAssetInformation()
        {
            return RetrieveAssetInformationAsync().GetAwaiter().GetResult();
        }

        public IResult UpdateAssetInformation(IAssetInformation assetInformation)
        {
            return UpdateAssetInformationAsync(assetInformation).GetAwaiter().GetResult();
        }

        public IResult<IEnumerable<IReference<ISubmodel>>> RetrieveAllSubmodelReferences()
        {
            return RetrieveAllSubmodelReferencesAsync().GetAwaiter().GetResult();
        }

        public IResult<IReference> CreateSubmodelReference(IReference submodelRef)
        {
            return CreateSubmodelReferenceAsync(submodelRef).GetAwaiter().GetResult();
        }

        public IResult DeleteSubmodelReference(string submodelIdentifier)
        {
            return DeleteSubmodelReferenceAsync(submodelIdentifier).GetAwaiter().GetResult();
        }

        #endregion

        #region Asset Administration Shell Client Interface

        public async Task<IResult<IAssetAdministrationShell>> RetrieveAssetAdministrationShellAsync(RequestContent content)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IAssetAdministrationShell>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateAssetAdministrationShellAsync(IAssetAdministrationShell aas)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, aas);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<IAssetInformation>> RetrieveAssetInformationAsync()
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IAssetInformation>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateAssetInformationAsync(IAssetInformation assetInformation)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, assetInformation);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<IEnumerable<IReference<ISubmodel>>>> RetrieveAllSubmodelReferencesAsync()
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IEnumerable<IReference<ISubmodel>>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<IReference>> CreateSubmodelReferenceAsync(IReference submodelRef)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, submodelRef);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IReference>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> DeleteSubmodelReferenceAsync(string submodelIdentifier)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, submodelIdentifier);
            var request = base.CreateRequest(uri, HttpMethod.Delete);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion

        #region Asset Administration Shell Submodel Client

        public ISubmodelClient CreateSubmodelClient(string submodelIdentifier)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL, submodelIdentifier);
            SubmodelHttpClient submodelClient = new SubmodelHttpClient(uri);
            return submodelClient;
        }

        public IResult<ISubmodel> RetrieveSubmodel(string submodelIdentifier, RequestLevel level = default, RequestContent content = default, RequestExtent extent = default)
        {
            return RetrieveSubmodelAsync(submodelIdentifier, level, content, extent).GetAwaiter().GetResult();
        }

        public IResult UpdateSubmodel(string submodelIdentifier, ISubmodel submodel)
        {
            return UpdateSubmodelAsync(submodelIdentifier, submodel).GetAwaiter().GetResult();
        }

        public IResult<ISubmodelElement> CreateSubmodelElement(string submodelIdentifier, string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return CreateSubmodelElementAsync(submodelIdentifier, rootIdShortPath, submodelElement).GetAwaiter().GetResult();
        }

        public IResult<ISubmodelElement> UpdateSubmodelElement(string submodelIdentifier, string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return UpdateSubmodelElementAsync(submodelIdentifier, rootIdShortPath, submodelElement).GetAwaiter().GetResult();
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements(string submodelIdentifier)
        {
            return RetrieveSubmodelElementsAsync(submodelIdentifier).GetAwaiter().GetResult();
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string submodelIdentifier, string idShortPath)
        {
            return RetrieveSubmodelElementAsync(submodelIdentifier, idShortPath).GetAwaiter().GetResult();
        }

        public IResult<IValue> RetrieveSubmodelElementValue(string submodelIdentifier, string idShortPath)
        {
            return RetrieveSubmodelElementValueAsync(submodelIdentifier, idShortPath).GetAwaiter().GetResult();
        }

        public IResult DeleteSubmodelElement(string submodelIdentifier, string idShortPath)
        {
            return DeleteSubmodelElementAsync(submodelIdentifier, idShortPath).GetAwaiter().GetResult();
        }

        public IResult<InvocationResponse> InvokeOperation(string submodelIdentifier, string idShortPath, InvocationRequest invocationRequest, bool async)
        {
            return InvokeOperationAsync(submodelIdentifier, idShortPath, invocationRequest, async).GetAwaiter().GetResult();
        }

        public IResult<InvocationResponse> GetInvocationResult(string submodelIdentifier, string idShortPath, string requestId)
        {
            return GetInvocationResultAsync(submodelIdentifier, idShortPath, requestId).GetAwaiter().GetResult();
        }

        public IResult UpdateSubmodelElementValue(string submodelIdentifier, string idShortPath, IValue value)
        {
            return UpdateSubmodelElementValueAsync(submodelIdentifier, idShortPath, value).GetAwaiter().GetResult();
        }

        public async Task<IResult<ISubmodel>> RetrieveSubmodelAsync(string submodelIdentifier, RequestLevel level = RequestLevel.Deep, RequestContent content = RequestContent.Normal, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL, submodelIdentifier);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodel>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateSubmodelAsync(string submodelIdentifier, ISubmodel submodel)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL, submodelIdentifier);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, submodel);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodel>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(string submodelIdentifier, ISubmodelElement submodelElement)
            => await CreateSubmodelElementAsync(".", submodelElement).ConfigureAwait(false);

        public async Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(string submodelIdentifier, string rootIdShortPath, ISubmodelElement submodelElement)
        {
            Uri uri;
            if (rootIdShortPath == ".")
                uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, submodelIdentifier);
            else
                uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, submodelIdentifier, rootIdShortPath);

            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, submodelElement);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelElement>> UpdateSubmodelElementAsync(string submodelIdentifier, string rootIdShortPath, ISubmodelElement submodelElement)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, submodelIdentifier, rootIdShortPath);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, submodelElement);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElementsAsync(string submodelIdentifier)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, submodelIdentifier);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IElementContainer<ISubmodelElement>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelElement>> RetrieveSubmodelElementAsync(string submodelIdentifier, string idShortPath)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, submodelIdentifier, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<IValue>> RetrieveSubmodelElementValueAsync(string submodelIdentifier, string idShortPath)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, submodelIdentifier, idShortPath, RequestContent.Value);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            IResult result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            if (result.Success && result.Entity != null)
            {
                string sValue = Encoding.UTF8.GetString((byte[])result.Entity);
                if (!string.IsNullOrEmpty(sValue))
                {
                    object deserializedValue = JsonConvert.DeserializeObject(sValue);
                    return new Result<IValue>(result.Success, new ElementValue(deserializedValue, deserializedValue?.GetType()), result.Messages);
                }
            }
            return new Result<IValue>(result);
        }

        public async Task<IResult> UpdateSubmodelElementValueAsync(string submodelIdentifier, string idShortPath, IValue value)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, submodelIdentifier, idShortPath, RequestContent.Value);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, value.Value);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> DeleteSubmodelElementAsync(string submodelIdentifier, string idShortPath)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, submodelIdentifier, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Delete);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<InvocationResponse>> InvokeOperationAsync(string submodelIdentifier, string idShortPath, InvocationRequest invocationRequest, bool async = false)
        {
            string path = AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE;
            if (async)
                path = AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE + "?async=true";

            Uri uri = GetPath(path, submodelIdentifier, idShortPath);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, invocationRequest);

            TimeSpan timeout = request.GetTimeout() ?? GetDefaultTimeout();
            if (invocationRequest.Timeout.HasValue && invocationRequest.Timeout.Value > timeout.TotalMilliseconds)
                request.SetTimeout(TimeSpan.FromMilliseconds(invocationRequest.Timeout.Value));

            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<InvocationResponse>> GetInvocationResultAsync(string submodelIdentifier, string idShortPath, string requestId)
        {
            string path = AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS.Replace("{handleId}", requestId);
            Uri uri = GetPath(path, submodelIdentifier, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion
    }
}
