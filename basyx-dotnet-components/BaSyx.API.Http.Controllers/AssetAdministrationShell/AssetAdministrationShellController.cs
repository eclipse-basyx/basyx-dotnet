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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using BaSyx.API.ServiceProvider;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using BaSyx.Utils.DependencyInjection;
using Newtonsoft.Json;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Asset Administration Shell Controller
    /// </summary>
    [ApiController]
    public class AssetAdministrationShellController : Controller
    {
        private readonly IAssetAdministrationShellServiceProvider serviceProvider;

        private static JsonSerializer _serializer = JsonSerializer.Create(new DependencyInjectionJsonSerializerSettings());

#if NETCOREAPP3_1
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Asset Administration Shell Controller
        /// </summary>
        /// <param name="assetAdministrationShellServiceProvider">The Asset Administration Shell Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public AssetAdministrationShellController(IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider, IWebHostEnvironment environment)
        {
            shellServiceProvider = aasServiceProvider;
            hostingEnvironment = environment;
        }
#else
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Asset Administration Shell Controller
        /// </summary>
        /// <param name="aasServiceProvider">The Asset Administration Shell Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public AssetAdministrationShellController(IAssetAdministrationShellServiceProvider aasServiceProvider, IHostingEnvironment environment)
        {
            serviceProvider = aasServiceProvider;
            hostingEnvironment = environment;
        }
#endif

        #region Asset Adminstration Shell Interface
        /// <summary>
        /// Returns the Asset Administration Shell
        /// </summary>
        /// <param name="content">Determines the request or response kind of the resource</param>
        /// <returns></returns>
        /// <response code="200">Requested Asset Administration Shell</response>
        [HttpGet(AssetAdministrationShellRoutes.AAS, Name = "GetAssetAdministrationShell")]
        [ProducesResponseType(typeof(AssetAdministrationShell), 200)]
        [Produces("application/json")]
        public IActionResult GetAssetAdministrationShell([FromQuery] RequestContent content = default)
        {
            var result = serviceProvider.RetrieveAssetAdministrationShell(content);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates the Asset Administration Shell
        /// </summary>
        /// <param name="aas">Asset Administration Shell object</param>
        /// <param name="content">Determines the request or response kind of the resource</param>
        /// <returns></returns>
        /// <response code="204">Asset Administration Shell updated successfully</response>
        [HttpPut(AssetAdministrationShellRoutes.AAS, Name = "PutAssetAdministrationShell")]
        [ProducesResponseType(204)]
        [Produces("application/json")]
        public IActionResult PutAssetAdministrationShell([FromBody] JObject aas, [FromQuery] RequestContent content = default)
        {
            if (aas == null)
                return ResultHandling.NullResult(nameof(aas));

            var deserialized = aas.ToObject<IAssetAdministrationShell>(_serializer);

            var result = serviceProvider.UpdateAssetAdministrationShell(deserialized);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <summary>
        /// Returns the Asset Information
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Requested Asset Information</response>
        [HttpGet(AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION, Name = "GetAssetInformation")]
        [ProducesResponseType(typeof(AssetInformation), 200)]
        [Produces("application/json")]
        public IActionResult GetAssetInformation()
        {
            var result = serviceProvider.RetrieveAssetInformation();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates the Asset Information
        /// </summary>
        /// <param name="assetInformation">Asset Information object</param>
        /// <returns></returns>
        /// <response code="204">Asset Information updated successfully</response>
        [HttpPut(AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION, Name = "PutAssetInformation")]
        [ProducesResponseType(204)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult PutAssetInformation([FromBody] JObject assetInformation)
        {
            if (assetInformation == null)
                return ResultHandling.NullResult(nameof(assetInformation));

            var deserialized = assetInformation.ToObject<IAssetInformation>(_serializer);

            var result = serviceProvider.UpdateAssetInformation(deserialized);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <summary>
        /// Returns all submodel references
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Requested submodel references</response> 
        [HttpGet(AssetAdministrationShellRoutes.AAS_SUBMODELS, Name = "GetAllSubmodelReferences")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetAllSubmodelReferences()
        {
            var result = serviceProvider.RetrieveAllSubmodelReferences();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a submodel reference at the Asset Administration Shell
        /// </summary>
        /// <param name="submodelReference">Reference to the Submodel</param>
        /// <returns></returns>
        /// <response code="201">Submodel reference created successfully</response>
        /// <response code="400">Bad Request</response>               
        [HttpPost(AssetAdministrationShellRoutes.AAS_SUBMODELS, Name = "PostSubmodelReference")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Reference), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public IActionResult PostSubmodelReference([FromBody] Reference submodelReference)
        {
            if (submodelReference == null)
                return ResultHandling.NullResult(nameof(submodelReference));

            var result = serviceProvider.CreateSubmodelReference(submodelReference);
            return result.CreateActionResult(CrudOperation.Create, AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID.Replace("{submodelIdentifier}", submodelReference.First.Value));
        }


        /// <summary>
        /// Deletes the submodel reference from the Asset Administration Shell
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="204">Submodel deleted successfully</response>
        /// <response code="400">Bad Request</response>    
        [HttpDelete(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, Name = "DeleteSubmodelReferenceById")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult DeleteSubmodelReferenceById(string submodelIdentifier)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));

            string submodelId = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.DeleteSubmodelReference(submodelId);
            return result.CreateActionResult(CrudOperation.Delete);
        }

        #endregion

        #region Submodel Interface

        /// <inheritdoc cref="SubmodelController.GetSubmodel(RequestLevel, RequestContent, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL, Name = "Shell_GetSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> Shell_GetSubmodel(string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return await service.GetSubmodel(level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.PutSubmodel(JObject, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPut(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL, Name = "Shell_PutSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PutSubmodel(string submodelIdentifier, [FromBody] JObject submodel, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PutSubmodel(submodel, level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElements(RequestLevel, RequestContent, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "Shell_GetAllSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetAllSubmodelElements(string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElements(level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.PostSubmodelElement(JObject, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPost(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "Shell_PostSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PostSubmodelElement(string submodelIdentifier, [FromBody] JObject submodelElement)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PostSubmodelElement(submodelElement);
        }

        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPath(string, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "Shell_GetSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPath(idShortPath, level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.PostSubmodelElementByPath(string, JObject, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPost(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "Shell_PostSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PostSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromBody] JObject submodelElement, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PostSubmodelElementByPath(idShortPath, submodelElement, level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.PutSubmodelElementByPath(string, JToken, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPut(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "Shell_PutSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PutSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromBody] JToken requestBody, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PutSubmodelElementByPath(idShortPath, requestBody, level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.DeleteSubmodelElementByPath(string)"/>
        [HttpDelete(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "Shell_DeleteSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_DeleteSubmodelElementByPath(string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.DeleteSubmodelElementByPath(idShortPath);
        }

        /// <inheritdoc cref="SubmodelController.UploadFileContentByIdShort(string, IFormFile)"/>
        [HttpPost(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_UPLOAD, Name = "Shell_UploadFileContentByIdShort")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> Shell_UploadFileContentByIdShort(string submodelIdentifier, string idShortPath, IFormFile file)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return await service.UploadFileContentByIdShort(idShortPath, file);
        }

        /// <inheritdoc cref="SubmodelController.InvokeOperation(string, JObject, bool, RequestContent)"/>
        [HttpPost(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE, Name = "Shell_InvokeOperation")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_InvokeOperation(string submodelIdentifier, string idShortPath, [FromBody] JObject operationRequest, [FromQuery] bool async = false, [FromQuery] RequestContent content = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperation(idShortPath, operationRequest, async, content);
        }

        /// <inheritdoc cref="SubmodelController.GetOperationAsyncResult(string, string)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS, Name = "Shell_GetOperationAsyncResult")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetOperationAsyncResult(string submodelIdentifier, string idShortPath, string handleId)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetOperationAsyncResult(idShortPath, handleId);
        }
        #endregion        
    }
}
