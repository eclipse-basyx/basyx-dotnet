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
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BaSyx.Utils.Extensions;
using BaSyx.API.Http;
using BaSyx.Models.Extensions;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace BaSyx.Clients.AdminShell.Http
{
    public class AssetAdministrationShellHttpClient : SimpleHttpClient, 
        IAssetAdministrationShellClient
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<AssetAdministrationShellHttpClient>();
        
        public static bool USE_HTTPS = true;

        private bool _standalone;

        public IEndpoint Endpoint { get; }

        private AssetAdministrationShellHttpClient(HttpMessageHandler messageHandler, bool standalone = true) : base(messageHandler)
        {
            _standalone = standalone;
            var options = new DefaultJsonSerializerOptions();
            var services = DefaultImplementation.GetStandardServiceCollection();
            options.AddDependencyInjection(new DependencyInjectionExtension(services));
            options.AddFullSubmodelElementConverter();
            JsonSerializerOptions = options.Build();
        }

        public AssetAdministrationShellHttpClient(Uri endpoint, bool standalone = true) : this(endpoint, null, standalone)
        { }
        public AssetAdministrationShellHttpClient(Uri endpoint, HttpMessageHandler messageHandler, bool standalone = true) : this(messageHandler, standalone)
        {
            endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            string endpointAddress = endpoint.ToString();
            Endpoint = new Endpoint(endpointAddress.RemoveFromEnd(AssetAdministrationShellRoutes.AAS), InterfaceName.AssetAdministrationShellInterface);
        }
        public AssetAdministrationShellHttpClient(IAssetAdministrationShellDescriptor aasDescriptor, bool standalone = true) : this(aasDescriptor, null, standalone)
        { }

        public AssetAdministrationShellHttpClient(IAssetAdministrationShellDescriptor aasDescriptor, HttpMessageHandler messageHandler, bool standalone = true) : this(messageHandler, standalone)
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

        public Uri GetPath(string requestPath = null, Identifier id = null, string idShortPath = null)
        {
            string path = Endpoint.ProtocolInformation.EndpointAddress.Trim('/');

            if (_standalone)
                path += AssetAdministrationShellRoutes.AAS;

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


        #region Descriptor Interface

        public IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellDescriptor()
        {
            return RetrieveAssetAdministrationShellDescriptorAsync().GetAwaiter().GetResult();
        }

        public async Task<IResult<IAssetAdministrationShellDescriptor>> RetrieveAssetAdministrationShellDescriptorAsync()
        {
            string path = Endpoint.ProtocolInformation.EndpointAddress.Trim('/') + DescriptionRoutes.DESCRIPTOR;
            Uri uri = new Uri(path);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IAssetAdministrationShellDescriptor>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion

        #region Asset Administration Shell Interface

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell()
        {
            return RetrieveAssetAdministrationShellAsync().GetAwaiter().GetResult();
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

        public IResult<PagedResult<IEnumerable<IReference<ISubmodel>>>> RetrieveAllSubmodelReferences()
        {
            return RetrieveAllSubmodelReferencesAsync().GetAwaiter().GetResult();
        }

        public IResult<IReference> CreateSubmodelReference(IReference submodelRef)
        {
            return CreateSubmodelReferenceAsync(submodelRef).GetAwaiter().GetResult();
        }

        public IResult DeleteSubmodelReference(Identifier id)
        {
            return DeleteSubmodelReferenceAsync(id).GetAwaiter().GetResult();
        }

        #endregion

        #region Asset Administration Shell Client Interface

        public async Task<IResult<IAssetAdministrationShell>> RetrieveAssetAdministrationShellAsync()
        {
            Uri uri = GetPath();
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IAssetAdministrationShell>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateAssetAdministrationShellAsync(IAssetAdministrationShell aas)
        {
            Uri uri = GetPath();
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

        public async Task<IResult<PagedResult<IEnumerable<IReference<ISubmodel>>>>> RetrieveAllSubmodelReferencesAsync()
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODEL_REFS);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IEnumerable<IReference<ISubmodel>>>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<IReference>> CreateSubmodelReferenceAsync(IReference submodelRef)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODEL_REFS);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, submodelRef);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<IReference>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> DeleteSubmodelReferenceAsync(Identifier id)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODEL_REFS_BYID, id);
            var request = base.CreateRequest(uri, HttpMethod.Delete);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion

        #region Asset Administration Shell Submodel Client

        public ISubmodelClient CreateSubmodelClient(Identifier id)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, id);
            SubmodelHttpClient submodelClient = new SubmodelHttpClient(uri);
            return submodelClient;
        }

        public IResult<ISubmodel> RetrieveSubmodel(Identifier id, RequestLevel level = default, RequestExtent extent = default)
        {
            return RetrieveSubmodelAsync(id, level, extent).GetAwaiter().GetResult();
        }

        public IResult UpdateSubmodel(Identifier id, ISubmodel submodel)
        {
            return UpdateSubmodelAsync(id, submodel).GetAwaiter().GetResult();
        }

        public IResult<ISubmodelElement> CreateSubmodelElement(Identifier id, string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return CreateSubmodelElementAsync(id, rootIdShortPath, submodelElement).GetAwaiter().GetResult();
        }

        public IResult<ISubmodelElement> UpdateSubmodelElement(Identifier id, string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return UpdateSubmodelElementAsync(id, rootIdShortPath, submodelElement).GetAwaiter().GetResult();
        }

        public IResult<PagedResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElements(Identifier id)
        {
            return RetrieveSubmodelElementsAsync(id).GetAwaiter().GetResult();
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(Identifier id, string idShortPath)
        {
            return RetrieveSubmodelElementAsync(id, idShortPath).GetAwaiter().GetResult();
        }

        public IResult<ValueScope> RetrieveSubmodelElementValue(Identifier id, string idShortPath)
        {
            return RetrieveSubmodelElementValueAsync(id, idShortPath).GetAwaiter().GetResult();
        }

        public IResult DeleteSubmodelElement(Identifier id, string idShortPath)
        {
            return DeleteSubmodelElementAsync(id, idShortPath).GetAwaiter().GetResult();
        }

        public IResult<InvocationResponse> InvokeOperation(Identifier id, string idShortPath, InvocationRequest invocationRequest, bool async)
        {
            return InvokeOperationAsync(id, idShortPath, invocationRequest, async).GetAwaiter().GetResult();
        }

        public IResult<InvocationResponse> GetInvocationResult(Identifier id, string idShortPath, string requestId)
        {
            return GetInvocationResultAsync(id, idShortPath, requestId).GetAwaiter().GetResult();
        }

        public IResult UpdateSubmodelElementValue(Identifier id, string idShortPath, ValueScope value)
        {
            return UpdateSubmodelElementValueAsync(id, idShortPath, value).GetAwaiter().GetResult();
        }

        public async Task<IResult<ISubmodel>> RetrieveSubmodelAsync(Identifier id, RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, id);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodel>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateSubmodelAsync(Identifier id, ISubmodel submodel)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, id);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, submodel);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(Identifier id, ISubmodelElement submodelElement)
            => await CreateSubmodelElementAsync(".", submodelElement).ConfigureAwait(false);

        public async Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(Identifier id, string rootIdShortPath, ISubmodelElement submodelElement)
        {
            Uri uri;
            if (rootIdShortPath == ".")
                uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, id);
            else
                uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, id, rootIdShortPath);

            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, submodelElement);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelElement>> UpdateSubmodelElementAsync(Identifier id, string rootIdShortPath, ISubmodelElement submodelElement)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, id, rootIdShortPath);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Put, submodelElement);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<PagedResult<IElementContainer<ISubmodelElement>>>> RetrieveSubmodelElementsAsync(Identifier id)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, id);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<PagedResult<IElementContainer<ISubmodelElement>>>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ISubmodelElement>> RetrieveSubmodelElementAsync(Identifier id, string idShortPath)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, id, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ISubmodelElement>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<ValueScope>> RetrieveSubmodelElementValueAsync(Identifier id, string idShortPath)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, id, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<ValueScope>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> UpdateSubmodelElementValueAsync(Identifier id, string idShortPath, ValueScope value)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, id, idShortPath);
            var request = base.CreateJsonContentRequest(uri, new HttpMethod("PATCH"), value);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult> DeleteSubmodelElementAsync(Identifier id, string idShortPath)
        {
            Uri uri = GetPath(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, id, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Delete);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<InvocationResponse>> InvokeOperationAsync(Identifier id, string idShortPath, InvocationRequest invocationRequest, bool async = false)
        {
            string path = AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE;
            if (async)
                path = AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC;

            Uri uri = GetPath(path, id, idShortPath);
            var request = base.CreateJsonContentRequest(uri, HttpMethod.Post, invocationRequest);

            TimeSpan timeout = request.GetTimeout() ?? GetDefaultTimeout();
            if (invocationRequest.Timeout.HasValue && invocationRequest.Timeout.Value > timeout.TotalMilliseconds)
                request.SetTimeout(TimeSpan.FromMilliseconds(invocationRequest.Timeout.Value));

            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        public async Task<IResult<InvocationResponse>> GetInvocationResultAsync(Identifier id, string idShortPath, string requestId)
        {
            string path = AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS.Replace("{handleId}", requestId);
            Uri uri = GetPath(path, id, idShortPath);
            var request = base.CreateRequest(uri, HttpMethod.Get);
            var response = await base.SendRequestAsync(request, CancellationToken.None);
            var result = await base.EvaluateResponseAsync<InvocationResponse>(response, response.Entity);
            response?.Entity?.Dispose();
            return result;
        }

        #endregion
    }
}
