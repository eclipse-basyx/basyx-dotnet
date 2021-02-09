/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Communication;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
        /// Retrieves all Submodels from the Submodel repository
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns a list of found Submodels</response>       
        [HttpGet("submodels", Name = "GetAllSubmodelsFromRepo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<Submodel>), 200)]
        public IActionResult GetAllSubmodelsFromRepo()
        {
            var result = serviceProvider.RetrieveSubmodels();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }
        /// <summary>
        /// Retrieves a specific Submodel from the Submodel repository
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Submodel</response>
        /// <response code="404">No Submodel found</response>        
        [HttpGet("submodels/{submodelId}")]
        [HttpGet("submodels/{submodelId}/submodel", Name = "RetrieveSubmodelFromRepoById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        public IActionResult RetrieveSubmodelFromRepoById(string submodelId)
        {
            if (string.IsNullOrEmpty(submodelId))
                return ResultHandling.NullResult(nameof(submodelId));

            submodelId = HttpUtility.UrlDecode(submodelId);

            var result = serviceProvider.RetrieveSubmodel(submodelId);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates or updates a Submodel at the Submodel repository
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="submodel">The Submodel object</param>
        /// <returns></returns>
        /// <response code="201">Submodel created / updated successfully</response>
        /// <response code="400">Bad Request</response>             
        [HttpPut("submodels/{submodelId}", Name = "PutSubmodelToRepo")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Submodel), 201)]
        public IActionResult PutSubmodelToRepo(string submodelId, [FromBody] ISubmodel submodel)
        {
            if (string.IsNullOrEmpty(submodelId))
                return ResultHandling.NullResult(nameof(submodelId));
            if (submodel == null)
                return ResultHandling.NullResult(nameof(submodel));

            submodelId = HttpUtility.UrlDecode(submodelId);

            if (submodelId != submodel.Identification.Id)
            {
                Result badRequestResult = new Result(false,
                    new Message(MessageType.Error, $"Passed path parameter {submodelId} does not equal the Submodel's Id {submodel.Identification.Id}", "400"));

                return badRequestResult.CreateActionResult(CrudOperation.Create, "submodels/" + submodelId);
            }

            var result = serviceProvider.CreateSubmodel(submodel);
            return result.CreateActionResult(CrudOperation.Create, "submodels/"+ submodelId);
        }
        /// <summary>
        /// Deletes a specific Submodel at the Submodel repository 
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <returns></returns>
        /// <response code="200">Submodel deleted successfully</response>
        [HttpDelete("submodels/{submodelId}", Name = "DeleteSubmodelFromRepoById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 200)]
        public IActionResult DeleteSubmodelFromRepoById(string submodelId)
        {
            if (string.IsNullOrEmpty(submodelId))
                return ResultHandling.NullResult(nameof(submodelId));

            submodelId = HttpUtility.UrlDecode(submodelId);

            var result = serviceProvider.DeleteSubmodel(submodelId);
            return result.CreateActionResult(CrudOperation.Delete);
        }

        /*****************************************************************************************/
        #region Routed Submodel Services

        /// <summary>
        /// Retrieves the minimized version of a Submodel, i.e. only the values of SubmodelElements are serialized and returned
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet("submodels/{submodelId}/submodel/values", Name = "SubmodelRepo_GetSubmodelValues")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetSubmodelValues(string submodelId)
        {
            if (serviceProvider.IsNullOrNotFound(submodelId, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelValues();
        }

        /// <summary>
        /// Retrieves all Submodel-Elements from the Submodel
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <returns></returns>
        /// <response code="200">Returns a list of found Submodel-Elements</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet("submodels/{submodelId}/submodel/submodelElements", Name = "SubmodelRepo_GetSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetSubmodelElements(string submodelId)
        {
            if (serviceProvider.IsNullOrNotFound(submodelId, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElements();
        }

        /// <summary>
        /// Creates or updates a Submodel-Element at the Submodel
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <param name="submodelElement">The Submodel-Element object</param>
        /// <returns></returns>
        /// <response code="201">Submodel-Element created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Submodel not found</response>
        [HttpPut("submodels/{submodelId}/submodel/submodelElements/{seIdShortPath}", Name = "SubmodelRepo_PutSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_PutSubmodelElement(string submodelId, string seIdShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.IsNullOrNotFound(submodelId, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PutSubmodelElement(seIdShortPath, submodelElement);
        }
        /// <summary>
        /// Retrieves a specific Submodel-Element from the Submodel
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Submodel-Element</response>
        /// <response code="404">Submodel / Submodel-Element not found</response>     
        [HttpGet("submodels/{submodelId}/submodel/submodelElements/{seIdShortPath}", Name = "SubmodelRepo_GetSubmodelElementById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetSubmodelElementById(string submodelId, string seIdShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(submodelId, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByIdShort(seIdShortPath);
        }

        /// <summary>
        /// Retrieves the value of a specific Submodel-Element from the Submodel
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <returns></returns>
        /// <response code="200">Returns the value of a specific Submodel-Element</response>
        /// <response code="404">Submodel / Submodel-Element not found</response>  
        /// <response code="405">Method not allowed</response>  
        [HttpGet("submodels/{submodelId}/submodel/submodelElements/{seIdShortPath}/value", Name = "SubmodelRepo_GetSubmodelElementValueById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        public IActionResult SubmodelRepo_GetSubmodelElementValueById(string submodelId, string seIdShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(submodelId, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementValueByIdShort(seIdShortPath);
        }

        /// <summary>
        /// Updates the Submodel-Element's value
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <param name="value">The new value</param>
        /// <returns></returns>
        /// <response code="200">Submodel-Element's value changed successfully</response>
        /// <response code="404">Submodel / Submodel-Element not found</response>     
        /// <response code="405">Method not allowed</response>  
        [HttpPut("submodels/{submodelId}/submodel/submodelElements/{seIdShortPath}/value", Name = "SubmodelRepo_PutSubmodelElementValueById")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ElementValue), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_PutSubmodelElementValueById(string submodelId, string seIdShortPath, [FromBody] object value)
        {
            if (serviceProvider.IsNullOrNotFound(submodelId, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PutSubmodelElementValueByIdShort(seIdShortPath, value);
        }

        /// <summary>
        /// Deletes a specific Submodel-Element from the Submodel
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <returns></returns>
        /// <response code="204">Submodel-Element deleted successfully</response>
        /// <response code="404">Submodel / Submodel-Element not found</response>
        [HttpDelete("submodels/{submodelId}/submodel/submodelElements/{seIdShortPath}", Name = "SubmodelRepo_DeleteSubmodelElementById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 200)]
        public IActionResult SubmodelRepo_DeleteSubmodelElementById(string submodelId, string seIdShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(submodelId, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.DeleteSubmodelElementByIdShort(seIdShortPath);
        }

        /// <summary>
        /// Uploads the actual file to the File-SubmodelElement
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="idShortPathToFile">The IdShort path to the File</param>
        /// <param name="file">The actual File to upload</param>
        /// <returns></returns>
        /// <response code="200">File uploaded successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Submodel / Method handler not found</response>
        [HttpPost("submodels/{submodelId}/submodel/submodelElements/{idShortPathToFile}/upload", Name = "SubmodelRepo_UploadFileContentByIdShort")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> SubmodelRepo_UploadFileContentByIdShort(string submodelId, string idShortPathToFile, IFormFile file)
        {
            if (serviceProvider.IsNullOrNotFound(submodelId, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return await service.UploadFileContentByIdShort(idShortPathToFile, file);
        }

        /// <summary>
        /// Invokes a specific operation from the Submodel synchronously or asynchronously
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="idShortPathToOperation">The IdShort path to the Operation</param>
        /// <param name="invocationRequest">The parameterized request object for the invocation</param>
        /// <param name="async">Determines whether the execution of the operation is asynchronous (true) or not (false)</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Submodel / Method handler not found</response>
        [HttpPost("submodels/{submodelId}/submodel/submodelElements/{idShortPathToOperation}/invoke", Name = "SubmodelRepo_InvokeOperationById")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_InvokeOperationById(string submodelId, string idShortPathToOperation, [FromBody] InvocationRequest invocationRequest, [FromQuery] bool async)
        {
            if (serviceProvider.IsNullOrNotFound(submodelId, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperationByIdShort(idShortPathToOperation, invocationRequest, async);
        }

        /// <summary>
        /// Retrieves the result of an asynchronously started operation
        /// </summary>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="idShortPathToOperation">The IdShort path to the Operation</param>
        /// <param name="requestId">The request id</param>
        /// <returns></returns>
        /// <response code="200">Result found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Submodel / Operation / Request not found</response>
        [HttpGet("submodels/{submodelId}/submodel/submodelElements/{idShortPathToOperation}/invocationList/{requestId}", Name = "SubmodelRepo_GetInvocationResultById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_GetInvocationResultById(string submodelId, string idShortPathToOperation, string requestId)
        {
            if (serviceProvider.IsNullOrNotFound(submodelId, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetInvocationResultByIdShort(idShortPathToOperation, requestId);
        }
        #endregion     
    }
}
