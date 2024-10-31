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
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using BaSyx.API.ServiceProvider;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Submodel Repository Controller
    /// </summary>
    [ApiController]
    public class SubmodelRepositoryController : Controller
    {
        private readonly ISubmodelRepositoryServiceProvider serviceProvider;
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Submodel Repository Controller
        /// </summary>
        /// <param name="submodelRepositoryServiceProvider"></param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public SubmodelRepositoryController(ISubmodelRepositoryServiceProvider submodelRepositoryServiceProvider, IWebHostEnvironment environment)
        {
            serviceProvider = submodelRepositoryServiceProvider;
            hostingEnvironment = environment;
        }

        /// <summary>
        /// Returns all Submodels
        /// </summary>
        /// <param name="semanticId">The value of the semantic id reference (BASE64-URL-encoded)</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns>Requested Submodels</returns>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODELS, Name = "GetAllSubmodels")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult<List<Submodel>>), 200)]
        public IActionResult GetAllSubmodels([FromQuery] string semanticId = null, [FromQuery] int limit = 100, [FromQuery] string cursor = "")
        {
            var result = serviceProvider.RetrieveSubmodels(limit, cursor);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a new Submodel
        /// </summary>
        /// <param name="submodel">Submodel object</param>
        /// <returns></returns>
        /// <response code="201">Submodel created successfully</response>
        /// <response code="400">Bad Request</response>             
        [HttpPost(SubmodelRepositoryRoutes.SUBMODELS, Name = "PostSubmodel")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Submodel), 201)]
        public IActionResult PostSubmodel([FromBody] ISubmodel submodel)
        {
            if (submodel == null)
                return ResultHandling.NullResult(nameof(submodel));

            string submodelId = ResultHandling.Base64UrlEncode(submodel.Id);

            var result = serviceProvider.CreateSubmodel(submodel);
            return result.CreateActionResult(CrudOperation.Create, SubmodelRepositoryRoutes.SUBMODELS + "/ " + submodelId);
        }

        /// <summary>
        /// Returns a specific Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="level"></param>
        /// <param name="extent"></param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <response code="404">No Submodel found</response>        
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID, Name = "GetSubmodelById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        public IActionResult GetSubmodelById(string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));

            submodelIdentifier = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.RetrieveSubmodel(submodelIdentifier);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates an existing Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodel">Submodel object</param>
        /// <param name="level"></param>
        /// <param name="extent"></param>
        /// <returns></returns>
        /// <response code="201">Submodel updated successfully</response>
        /// <response code="400">Bad Request</response>             
        [HttpPut(SubmodelRepositoryRoutes.SUBMODEL_BYID, Name = "PutSubmodelById")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Submodel), 201)]
        public IActionResult PutSubmodelById(string submodelIdentifier, [FromBody] ISubmodel submodel, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));
            if (submodel == null)
                return ResultHandling.NullResult(nameof(submodel));

            submodelIdentifier = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.UpdateSubmodel(submodelIdentifier, submodel);
            return result.CreateActionResult(CrudOperation.Update);
        }


        /// <summary>
        /// Deletes a Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Submodel deleted successfully</response>
        [HttpDelete(SubmodelRepositoryRoutes.SUBMODEL_BYID, Name = "DeleteSubmodelById")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        public IActionResult DeleteSubmodelById(string submodelIdentifier)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));

            submodelIdentifier = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.DeleteSubmodel(submodelIdentifier);
            return result.CreateActionResult(CrudOperation.Delete);
        }

        #region Submodel Interface


        /// <inheritdoc cref="SubmodelController.GetSubmodelMetadata()"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + OutputModifier.METADATA, Name = "SubmodelRepo_GetSubmodelMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetSubmodelMetadata(string submodelIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelMetadata();
        }

        /// <inheritdoc cref="SubmodelController.GetSubmodelValueOnly(RequestLevel, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + OutputModifier.VALUE, Name = "SubmodelRepo_GetSubmodelValue")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetSubmodelValue(string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelValueOnly(level, extent);
        }

        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElements(int, string, RequestLevel, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "SubmodelRepo_GetAllSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetAllSubmodelElements(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElements(limit, cursor, level, extent);
        }

        /// <inheritdoc cref="SubmodelController.PostSubmodelElement(ISubmodelElement, RequestLevel, RequestExtent)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "SubmodelRepo_PostSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_PostSubmodelElement(string submodelIdentifier, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PostSubmodelElement(submodelElement);
        }

        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPath(string, RequestLevel, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "SubmodelRepo_GetSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPath(idShortPath, level, extent);
        }

        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPathValueOnly(string, RequestLevel, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, Name = "SubmodelRepo_GetSubmodelElementByPathValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetSubmodelElementByPathValueOnly(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPathValueOnly(idShortPath, level, extent);
        }

        /// <inheritdoc cref="SubmodelController.PostSubmodelElementByPath(string, ISubmodelElement)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "SubmodelRepo_PostSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_PostSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PostSubmodelElementByPath(idShortPath, submodelElement);
        }

        /// <inheritdoc cref="SubmodelController.PutSubmodelElementByPath(string, ISubmodelElement)"/>
        [HttpPut(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "SubmodelRepo_PutSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_PutSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement requestBody)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PutSubmodelElementByPath(idShortPath, requestBody);
        }

		/// <inheritdoc cref="SubmodelController.PatchSubmodelElementValueByPathValueOnly(string, JsonDocument)"/>
		[HttpPatch(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, Name = "SubmodelRepo_PatchSubmodelElementValueByPathValueOnly")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_PatchSubmodelElementValueByPathValueOnly(string submodelIdentifier, string idShortPath, [FromBody] JsonDocument requestBody)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodelElementValueByPathValueOnly(idShortPath, requestBody);
        }

        /// <inheritdoc cref="SubmodelController.DeleteSubmodelElementByPath(string)"/>
        [HttpDelete(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "SubmodelRepo_DeleteSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_DeleteSubmodelElementByPath(string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.DeleteSubmodelElementByPath(idShortPath);
        }

        /// <inheritdoc cref="SubmodelController.GetFileByPath(string)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "SubmodelRepo_GetFileByPath")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetFileByPath(string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetFileByPath(idShortPath);
        }

        /// <inheritdoc cref="SubmodelController.PutFileByPath(string, IFormFile)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "SubmodelRepo_UploadFileContentByIdShort")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> SubmodelRepo_UploadFileContentByIdShort(string submodelIdentifier, string idShortPath, IFormFile file)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return await service.PutFileByPath(idShortPath, file);
        }

        /// <inheritdoc cref="SubmodelController.InvokeOperationSync(string, InvocationRequest)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE, Name = "SubmodelRepo_InvokeOperationSync")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_InvokeOperationSync(string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperationSync(idShortPath, operationRequest);
        }

        /// <inheritdoc cref="SubmodelController.InvokeOperationAsync(string, InvocationRequest)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC, Name = "SubmodelRepo_InvokeOperationAsync")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_InvokeOperationAsync(string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperationAsync(idShortPath, operationRequest);
        }

        /// <inheritdoc cref="SubmodelController.GetOperationAsyncResult(string, string)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS, Name = "SubmodelRepo_GetOperationAsyncResult")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetOperationAsyncResult(string submodelIdentifier, string idShortPath, string handleId)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetOperationAsyncResult(idShortPath, handleId);
        }
        #endregion     
    }
}
