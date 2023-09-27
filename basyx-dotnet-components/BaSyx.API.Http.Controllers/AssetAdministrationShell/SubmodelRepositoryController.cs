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

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Submodel Repository Controller
    /// </summary>
    [ApiController]
    public class SubmodelRepositoryController : Controller
    {
        private readonly ISubmodelRepositoryServiceProvider serviceProvider;

#if NETCOREAPP3_1
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Submodel Repository Controller
        /// </summary>
        /// <param name="submodelRepositoryServiceProvider"></param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public SubmodelRepositoryController(ISubmodelRepositoryServiceProvider submodelRepositoryServiceProvider, IWebHostEnvironment environment)
        {
            shellServiceProvider = aasServiceProvider;
            hostingEnvironment = environment;
        }
#else
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Submodel Repository Controller
        /// </summary>
        /// <param name="submodelRepositoryServiceProvider"></param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public SubmodelRepositoryController(ISubmodelRepositoryServiceProvider submodelRepositoryServiceProvider, IHostingEnvironment environment)
        {
            serviceProvider = submodelRepositoryServiceProvider;
            hostingEnvironment = environment;
        }
#endif

        /// <summary>
        /// Returns all Submodels
        /// </summary>
        /// <param name="semanticId">The value of the semantic id reference (BASE64-URL-encoded)</param>
        /// <param name="idShort">The Submodel's idShort</param>
        /// <returns>Requested Submodels</returns>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODELS, Name = "GetAllSubmodels")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<Submodel>), 200)]
        public IActionResult GetAllSubmodels([FromQuery] string semanticId = null, [FromQuery] string idShort = null)
        {
            var result = serviceProvider.RetrieveSubmodels();
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

            string submodelId = ResultHandling.Base64UrlEncode(submodel.Identification.Id);

            var result = serviceProvider.CreateSubmodel(submodel);
            return result.CreateActionResult(CrudOperation.Create, SubmodelRepositoryRoutes.SUBMODELS + "/ " + submodelId);
        }

        /// <summary>
        /// Returns a specific Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <response code="404">No Submodel found</response>        
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID, Name = "GetSubmodelById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        public IActionResult GetSubmodelById(string submodelIdentifier)
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
        /// <returns></returns>
        /// <response code="201">Submodel updated successfully</response>
        /// <response code="400">Bad Request</response>             
        [HttpPut(SubmodelRepositoryRoutes.SUBMODEL_BYID, Name = "PutSubmodelById")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Submodel), 201)]
        public IActionResult PutSubmodelById(string submodelIdentifier, [FromBody] ISubmodel submodel)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));
            if (submodel == null)
                return ResultHandling.NullResult(nameof(submodel));

            submodelIdentifier = ResultHandling.Base64UrlDecode(submodelIdentifier);

            if (submodelIdentifier != submodel.Identification.Id)
            {
                Result badRequestResult = new Result(false,
                    new Message(MessageType.Error, $"Passed path parameter {submodelIdentifier} does not equal the Submodel's Id {submodel.Identification.Id}", "400"));

                return badRequestResult.CreateActionResult(CrudOperation.Create, "submodels/" + submodelIdentifier);
            }

            var result = serviceProvider.CreateSubmodel(submodel);
            return result.CreateActionResult(CrudOperation.Create, "submodels/" + submodelIdentifier);
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

        /// <inheritdoc cref="SubmodelController.GetSubmodel(RequestLevel, RequestContent, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL, Name = "SubmodelRepo_GetSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetSubmodel(string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodel(level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.PutSubmodel(JObject, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPut(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL, Name = "SubmodelRepo_PutSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_PutSubmodel(string submodelIdentifier, [FromBody] JObject submodel, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PutSubmodel(submodel, level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElements(RequestLevel, RequestContent, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "SubmodelRepo_GetAllSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetAllSubmodelElements(string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElements(level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.PostSubmodelElement(JObject, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "SubmodelRepo_PostSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_PostSubmodelElement(string submodelIdentifier, [FromBody] JObject submodelElement)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PostSubmodelElement(submodelElement);
        }

        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPath(string, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "SubmodelRepo_GetSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPath(idShortPath, level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.PostSubmodelElementByPath(string, JObject, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "SubmodelRepo_PostSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_PostSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromBody] JObject submodelElement, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PostSubmodelElementByPath(idShortPath, submodelElement, level, content, extent);
        }

        /// <inheritdoc cref="SubmodelController.PutSubmodelElementByPath(string, JToken, RequestLevel, RequestContent, RequestExtent)"/>
        [HttpPut(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "SubmodelRepo_PutSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_PutSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromBody] JToken requestBody, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PutSubmodelElementByPath(idShortPath, requestBody, level, content, extent);
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

        /// <inheritdoc cref="SubmodelController.UploadFileContentByIdShort(string, IFormFile)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_UPLOAD, Name = "SubmodelRepo_UploadFileContentByIdShort")]
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
            return await service.UploadFileContentByIdShort(idShortPath, file);
        }

        /// <inheritdoc cref="SubmodelController.InvokeOperation(string, JObject, bool, RequestContent)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE, Name = "SubmodelRepo_InvokeOperation")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_InvokeOperation(string submodelIdentifier, string idShortPath, [FromBody] JObject operationRequest, [FromQuery] bool async = false, [FromQuery] RequestContent content = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperation(idShortPath, operationRequest, async, content);
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
