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
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using BaSyx.API.ServiceProvider;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using BaSyx.Utils.DependencyInjection;
using Newtonsoft.Json;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Asset Administration Shell Repository Controller
    /// </summary>
    [ApiController]
    public class AssetAdministrationShellRepositoryController : Controller
    {
        private readonly IAssetAdministrationShellRepositoryServiceProvider serviceProvider;

        private static JsonSerializer _serializer = JsonSerializer.Create(new DependencyInjectionJsonSerializerSettings());

#if NETCOREAPP3_1
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Asset Administration Shell Repository Controller
        /// </summary>
        /// <param name="assetAdministrationShellRepositoryServiceProvider"></param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public AssetAdministrationShellRepositoryController(IAssetAdministrationShellRepositoryServiceProvider assetAdministrationShellRepositoryServiceProvider, IWebHostEnvironment environment)
        {
            shellServiceProvider = aasServiceProvider;
            hostingEnvironment = environment;
        }
#else
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Asset Administration Shell Repository Controller
        /// </summary>
        /// <param name="assetAdministrationShellRepositoryServiceProvider"></param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public AssetAdministrationShellRepositoryController(IAssetAdministrationShellRepositoryServiceProvider assetAdministrationShellRepositoryServiceProvider, IHostingEnvironment environment)
        {
            serviceProvider = assetAdministrationShellRepositoryServiceProvider;
            hostingEnvironment = environment;
        }
#endif

        /// <summary>
        /// Returns all Asset Administration Shells
        /// </summary>
        /// <param name="idShort">The Asset Administration Shell’s IdShort</param>
        /// <param name="assetIds">The key-value-pair of an Asset identifier</param>
        /// <returns></returns>
        /// <response code="200">Requested Asset Administration Shells</response>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS, Name = "GetAllAssetAdministrationShells")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<BaSyx.Models.AdminShell.AssetAdministrationShell>), 200)]
        public IActionResult GetAllAssetAdministrationShells([FromQuery] string idShort = null, [FromQuery] List<string> assetIds = null)
        {
            var result = serviceProvider.RetrieveAssetAdministrationShells();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a new Asset Administration Shell
        /// </summary>
        /// <param name="aas">Asset Administration Shell object</param>
        /// <returns></returns>
        /// <response code="201">Asset Administration Shell created successfully</response>
        /// <response code="400">Bad Request</response>             
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS, Name = "PostAssetAdministrationShell")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(BaSyx.Models.AdminShell.AssetAdministrationShell), 201)]
        public IActionResult PostAssetAdministrationShell([FromBody] JObject aas)
        {
            if (aas == null)
                return ResultHandling.NullResult(nameof(aas));

            var deserialized = aas.ToObject<IAssetAdministrationShell>(_serializer);
            string aasIdentifier = ResultHandling.Base64UrlEncode(deserialized.Id);

            var result = serviceProvider.CreateAssetAdministrationShell(deserialized);
            return result.CreateActionResult(CrudOperation.Create, AssetAdministrationShellRepositoryRoutes.SHELLS_AAS.Replace("{aasIdentifier}", aasIdentifier));
        }

        /// <summary>
        /// Returns a specific Asset Administration Shell
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Asset Administration Shell</response>
        /// <response code="404">No Asset Administration Shell found</response>           
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS, Name = "GetAssetAdministrationShellById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BaSyx.Models.AdminShell.AssetAdministrationShell), 200)]
        public IActionResult GetAssetAdministrationShellById(string aasIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);

            var result = serviceProvider.RetrieveAssetAdministrationShell(aasIdentifier);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates an existing Asset Administration Shell
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="aas">Asset Administration Shell object</param>
        /// <returns></returns>
        /// <response code="204">Asset Administration Shell updated successfully</response>
        /// <response code="400">Bad Request</response>             
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS, Name = "PutAssetAdministrationShellById")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(BaSyx.Models.AdminShell.AssetAdministrationShell), 201)]
        public IActionResult PutAssetAdministrationShellById(string aasIdentifier, [FromBody] JObject aas)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));
            if (aas == null)
                return ResultHandling.NullResult(nameof(aas));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);
            var deserialized = aas.ToObject<IAssetAdministrationShell>(_serializer);

            if (aasIdentifier != deserialized.Id)
            {
                Result badRequestResult = new Result(false,
                    new Message(MessageType.Error, $"Passed path parameter {aasIdentifier} does not equal the Asset Administration Shells's id {deserialized.Id}", "400"));

                return badRequestResult.CreateActionResult(CrudOperation.Create, $"shells/{aasIdentifier}");
            }

            var result = serviceProvider.CreateAssetAdministrationShell(deserialized);
            return result.CreateActionResult(CrudOperation.Create, $"shells/{aasIdentifier}");
        }


        /// <summary>
        /// Deletes an Asset Administration Shell
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Asset Administration Shell deleted successfully</response>
        [HttpDelete(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS, Name = "DeleteAssetAdministrationShellById")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        public IActionResult DeleteAssetAdministrationShellById(string aasIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);

            var result = serviceProvider.DeleteAssetAdministrationShell(aasIdentifier);
            return result.CreateActionResult(CrudOperation.Delete);            
        }

        #region Asset Adminstration Shell Interface

        /// <inheritdoc cref="AssetAdministrationShellController.GetAssetAdministrationShell(RequestContent)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS, Name = "ShellRepo_GetAssetAdministrationShell")]
        [ProducesResponseType(typeof(IAssetAdministrationShell), 200)]
        [Produces("application/json")]
        public IActionResult ShellRepo_GetAssetAdministrationShell(string aasIdentifier, [FromQuery] RequestContent content = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.GetAssetAdministrationShell(content);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.PutAssetAdministrationShell(JObject, RequestContent)"/>
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS, Name = "ShellRepo_PutAssetAdministrationShell")]
        [ProducesResponseType(204)]
        [Produces("application/json")]
        public IActionResult ShellRepo_PutAssetAdministrationShell(string aasIdentifier, [FromBody] JObject aas, [FromQuery] RequestContent content = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.PutAssetAdministrationShell(aas, content);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.GetAssetInformation"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION, Name = "ShellRepo_GetAssetInformation")]
        [ProducesResponseType(200)]
        [Produces("application/json")]
        public IActionResult ShellRepo_GetAssetInformation(string aasIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.GetAssetInformation();
        }

        /// <inheritdoc cref="AssetAdministrationShellController.PutAssetInformation(JObject)"/>
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION, Name = "ShellRepo_PutAssetInformation")]
        [ProducesResponseType(204)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult ShellRepo_PutAssetInformation(string aasIdentifier, [FromBody] JObject assetInformation)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.PutAssetInformation(assetInformation);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.GetAllSubmodelReferences"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS, Name = "ShellRepo_GetAllSubmodelReferences")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetAllSubmodelReferences(string aasIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.GetAllSubmodelReferences();
        }

        /// <inheritdoc cref="AssetAdministrationShellController.PostSubmodelReference(Reference)"/>          
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS, Name = "ShellRepo_PostSubmodelReference")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Reference), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public IActionResult ShellRepo_PostSubmodelReference(string aasIdentifier, [FromBody] Reference submodelReference)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.PostSubmodelReference(submodelReference);
        }


        /// <inheritdoc cref="AssetAdministrationShellController.DeleteSubmodelReferenceById(string)"/>          
        [HttpDelete(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, Name = "ShellRepo_DeleteSubmodelReferenceById")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        public IActionResult ShellRepo_DeleteSubmodelReferenceById(string aasIdentifier, string submodelIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.DeleteSubmodelReferenceById(submodelIdentifier);
        }

        #endregion

        #region Submodel Interface

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodel(string, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL, Name = "ShellRepo_GetSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> ShellRepo_GetSubmodel(string aasIdentifier, string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return await service.Shell_GetSubmodel(submodelIdentifier, level, content, extent);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PutSubmodel(string, JObject, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL, Name = "ShellRepo_PutSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_PutSubmodel(string aasIdentifier, string submodelIdentifier, [FromBody] JObject submodel, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PutSubmodel(submodelIdentifier, submodel, level, content, extent);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetAllSubmodelElements(string, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "ShellRepo_GetAllSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetAllSubmodelElements(string aasIdentifier, string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetAllSubmodelElements(submodelIdentifier);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PostSubmodelElement(string, JObject)"/>
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "ShellRepo_PostSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_PostSubmodelElement(string aasIdentifier, string submodelIdentifier, [FromBody] JObject submodelElement)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PostSubmodelElement(submodelIdentifier, submodelElement);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodelElementByPath(string, string, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "ShellRepo_GetSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetSubmodelElementByPath(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodelElementByPath(submodelIdentifier, idShortPath, level, content, extent);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PostSubmodelElementByPath(string, string, JObject, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "ShellRepo_PostSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_PostSubmodelElementByPath(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] JObject submodelElement, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PostSubmodelElementByPath(submodelIdentifier, idShortPath, submodelElement, level, content, extent);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PutSubmodelElementByPath(string, string, JToken, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "ShellRepo_PutSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_PutSubmodelElementByPath(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] JToken requestBody, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PutSubmodelElementByPath(submodelIdentifier, idShortPath, requestBody, level, content, extent);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_DeleteSubmodelElementByPath(string, string)"/>
        [HttpDelete(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "ShellRepo_DeleteSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_DeleteSubmodelElementByPath(string aasIdentifier, string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_DeleteSubmodelElementByPath(submodelIdentifier, idShortPath);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_UploadFileContentByIdShort(string, string, IFormFile)"/>
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_UPLOAD, Name = "ShellRepo_UploadFileContentByIdShort")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> ShellRepo_UploadFileContentByIdShort(string aasIdentifier, string submodelIdentifier, string idShortPath, IFormFile file)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return await service.Shell_UploadFileContentByIdShort(submodelIdentifier, idShortPath, file);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_InvokeOperation(string, string, JObject, bool, RequestContent)"/>
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE, Name = "ShellRepo_InvokeOperation")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_InvokeOperation(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] JObject operationRequest, [FromQuery] bool async = false, [FromQuery] RequestContent content = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_InvokeOperation(submodelIdentifier, idShortPath, operationRequest, async, content);
        }

        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetOperationAsyncResult(string, string, string)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS, Name = "ShellRepo_GetOperationAsyncResult")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetOperationAsyncResult(string aasIdentifier, string submodelIdentifier, string idShortPath, string handleId)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetOperationAsyncResult(submodelIdentifier, idShortPath, handleId);
        }
        #endregion     
    }
}
