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
using Microsoft.AspNetCore.Mvc;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using BaSyx.API.Components;
using System;
using Newtonsoft.Json.Linq;
using BaSyx.Models.Extensions;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Communication;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.IO;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Submodel Controller
    /// </summary>
    [ApiController]
    public class SubmodelController : Controller
    {
        private readonly ISubmodelServiceProvider serviceProvider;

#if NETCOREAPP3_1
        private readonly IWebHostEnvironment hostingEnvironment;

         /// <summary>
        /// The constructor for the Submodel Controller
        /// </summary>
        /// <param name="submodelServiceProvider">The Submodel Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public SubmodelController(ISubmodelServiceProvider submodelServiceProvider, IWebHostEnvironment environment)
        {
            shellServiceProvider = aasServiceProvider;
            hostingEnvironment = environment;
        }
#else
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Submodel Controller
        /// </summary>
        /// <param name="submodelServiceProvider">The Submodel Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public SubmodelController(ISubmodelServiceProvider submodelServiceProvider, IHostingEnvironment environment)
        {
            serviceProvider = submodelServiceProvider;
            hostingEnvironment = environment;
        }
#endif

        /// <summary>
        /// Retrieves the entire Submodel
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet("submodel", Name = "GetSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetSubmodel()
        {
            var result = serviceProvider.RetrieveSubmodel();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Retrieves the minimized version of a Submodel, i.e. only the values of SubmodelElements are serialized and returned
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet("submodel/values", Name = "GetSubmodelValues")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetSubmodelValues()
        {
            var result = serviceProvider.RetrieveSubmodel();

            if (result != null && result.Entity != null)
            {
                JObject minimizedSubmodel = result.Entity.MinimizeSubmodel();
                return new JsonResult(minimizedSubmodel);
            }

            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Retrieves a customizable table version of a Submodel
        /// </summary>
        /// <param name="columns">A comma-separated list of field names to structure the payload beeing returned</param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="404">Submodel not found</response>   
        [HttpGet("submodel/table", Name = "GetSubmodelAsTable")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetSubmodelAsTable([FromQuery] string columns)
        {
            if (string.IsNullOrEmpty(columns))
                return ResultHandling.NullResult(nameof(columns));

            var result = serviceProvider.RetrieveSubmodel();
            if (result != null && result.Entity != null)
            {
                string[] columnNames = columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                JToken customizedSubmodel = result.Entity.CustomizeSubmodel(columnNames);
                return new JsonResult(customizedSubmodel);
            }

            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Retrieves all Submodel-Elements from the Submodel
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns a list of found Submodel-Elements</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet("submodel/submodelElements", Name = "GetSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetSubmodelElements()
        {
            var result = serviceProvider.RetrieveSubmodelElements();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates or updates a Submodel-Element at the Submodel
        /// </summary>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <param name="submodelElement">The Submodel-Element object</param>
        /// <returns></returns>
        /// <response code="201">Submodel-Element created successfully</response>
        /// <response code="400">Bad Request</response>
        [HttpPut("submodel/submodelElements/{seIdShortPath}", Name = "PutSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult PutSubmodelElement(string seIdShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (string.IsNullOrEmpty(seIdShortPath))
                return ResultHandling.NullResult(nameof(seIdShortPath));
            if (submodelElement == null)
                return ResultHandling.NullResult(nameof(submodelElement));

            seIdShortPath = HttpUtility.UrlDecode(seIdShortPath);

            var result = serviceProvider.CreateOrUpdateSubmodelElement(seIdShortPath, submodelElement);
            return result.CreateActionResult(CrudOperation.Create, "submodel/submodelElements/" + seIdShortPath);
        }


        /// <summary>
        /// Retrieves a specific Submodel-Element from the Submodel
        /// </summary>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Submodel-Element</response>
        /// <response code="404">Submodel Element not found</response>     
        [HttpGet("submodel/submodelElements/{seIdShortPath}", Name = "GetSubmodelElementByIdShort")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetSubmodelElementByIdShort(string seIdShortPath)
        {
            if (string.IsNullOrEmpty(seIdShortPath))
                return ResultHandling.NullResult(nameof(seIdShortPath));

            seIdShortPath = HttpUtility.UrlDecode(seIdShortPath);

            var result = serviceProvider.RetrieveSubmodelElement(seIdShortPath);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Retrieves the value of a specific Submodel-Element from the Submodel
        /// </summary>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <returns></returns>
        /// <response code="200">Returns the value of a specific Submodel-Element</response>
        /// <response code="404">Submodel / Submodel-Element not found</response>  
        /// <response code="405">Method not allowed</response>  
        [HttpGet("submodel/submodelElements/{seIdShortPath}/value", Name = "GetSubmodelElementValueByIdShort")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        public IActionResult GetSubmodelElementValueByIdShort(string seIdShortPath)
        {
            if (string.IsNullOrEmpty(seIdShortPath))
                return ResultHandling.NullResult(nameof(seIdShortPath));

            seIdShortPath = HttpUtility.UrlDecode(seIdShortPath);

            var result = serviceProvider.RetrieveSubmodelElementValue(seIdShortPath);
            if (result.Success && result.Entity != null)
                return new OkObjectResult(result.Entity.Value);
            else
                return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates the Submodel-Element's value
        /// </summary>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <param name="value">The new value</param>
        /// <returns></returns>
        /// <response code="200">Submodel-Element's value changed successfully</response>
        /// <response code="404">Submodel-Element not found</response>     
        /// <response code="405">Method not allowed</response>  
        [HttpPut("submodel/submodelElements/{seIdShortPath}/value", Name = "PutSubmodelElementValueByIdShort")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ElementValue), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult PutSubmodelElementValueByIdShort(string seIdShortPath, [FromBody] object value)
        {
            if (string.IsNullOrEmpty(seIdShortPath))
                return ResultHandling.NullResult(nameof(seIdShortPath));
            if (value == null)
                return ResultHandling.NullResult(nameof(value));

            seIdShortPath = HttpUtility.UrlDecode(seIdShortPath);

            ElementValue elementValue = new ElementValue(value, value.GetType());
            var result = serviceProvider.UpdateSubmodelElementValue(seIdShortPath, elementValue);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <summary>
        /// Deletes a specific Submodel-Element from the Submodel
        /// </summary>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <returns></returns>
        /// <response code="204">Submodel-Element deleted successfully</response>
        /// <response code="404">Submodel-Element not found</response>
        [HttpDelete("submodel/submodelElements/{seIdShortPath}", Name = "DeleteSubmodelElementByIdShort")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 200)]
        public IActionResult DeleteSubmodelElementByIdShort(string seIdShortPath)
        {
            if (string.IsNullOrEmpty(seIdShortPath))
                return ResultHandling.NullResult(nameof(seIdShortPath));

            seIdShortPath = HttpUtility.UrlDecode(seIdShortPath);

            var result = serviceProvider.DeleteSubmodelElement(seIdShortPath);
            return result.CreateActionResult(CrudOperation.Delete);
        }

        /// <summary>
        /// Invokes a specific operation from the Submodel synchronously or asynchronously
        /// </summary>
        /// <param name="idShortPathToFile">The IdShort path to the File</param>
        /// <param name="file">The actual File to upload</param>
        /// <returns></returns>
        /// <response code="200">File uploaded successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Method handler not found</response>
        [HttpPost("submodel/submodelElements/{idShortPathToFile}/upload", Name = "UploadFileContentByIdShort")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> UploadFileContentByIdShort(string idShortPathToFile, IFormFile file)
        {
            if (string.IsNullOrEmpty(idShortPathToFile))
                return ResultHandling.NullResult(nameof(idShortPathToFile));
            if (file == null)
                return ResultHandling.NullResult(nameof(file));

            idShortPathToFile = HttpUtility.UrlDecode(idShortPathToFile);

            var fileElementRetrieved = serviceProvider.RetrieveSubmodelElement(idShortPathToFile);
            if(!fileElementRetrieved.Success || fileElementRetrieved.Entity == null)
                return fileElementRetrieved.CreateActionResult(CrudOperation.Retrieve);

            IFile fileElement = fileElementRetrieved.Entity.Cast<IFile>();
            string fileName = fileElement.Value.TrimStart('/');
            string filePath = Path.Combine(hostingEnvironment.ContentRootPath, fileName);
            
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return Ok();
        }

        /// <summary>
        /// Invokes a specific operation from the Submodel synchronously or asynchronously
        /// </summary>
        /// <param name="idShortPathToOperation">The IdShort path to the Operation</param>
        /// <param name="invocationRequest">The parameterized request object for the invocation</param>
        /// <param name="async">Determines whether the execution of the operation is asynchronous (true) or not (false)</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Method handler not found</response>
        [HttpPost("submodel/submodelElements/{idShortPathToOperation}/invoke", Name = "InvokeOperationByIdShortAsync")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult InvokeOperationByIdShort(string idShortPathToOperation, [FromBody] InvocationRequest invocationRequest, [FromQuery] bool async)
        {
            if (string.IsNullOrEmpty(idShortPathToOperation))
                return ResultHandling.NullResult(nameof(idShortPathToOperation));
            if (invocationRequest == null)
                return ResultHandling.NullResult(nameof(invocationRequest));

            idShortPathToOperation = HttpUtility.UrlDecode(idShortPathToOperation);

            if (async)
            {
                IResult<CallbackResponse> result = serviceProvider.InvokeOperationAsync(idShortPathToOperation, invocationRequest);
                return result.CreateActionResult(CrudOperation.Invoke);
            }
            else
            {
                IResult<InvocationResponse> result = serviceProvider.InvokeOperation(idShortPathToOperation, invocationRequest);
                return result.CreateActionResult(CrudOperation.Invoke);
            }
        }

        /// <summary>
        /// Retrieves the result of an asynchronously started operation
        /// </summary>
        /// <param name="idShortPathToOperation">The IdShort path to the Operation</param>
        /// <param name="requestId">The request id</param>
        /// <returns></returns>
        /// <response code="200">Result found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Operation / Request not found</response>
        [HttpGet("submodel/submodelElements/{idShortPathToOperation}/invocationList/{requestId}", Name = "GetInvocationResultByIdShort")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetInvocationResultByIdShort(string idShortPathToOperation, string requestId)
        {
            if (string.IsNullOrEmpty(idShortPathToOperation))
                return ResultHandling.NullResult(nameof(idShortPathToOperation));
            if (string.IsNullOrEmpty(requestId))
                return ResultHandling.NullResult(nameof(requestId));

            idShortPathToOperation = HttpUtility.UrlDecode(idShortPathToOperation);

            IResult<InvocationResponse> result = serviceProvider.GetInvocationResult(idShortPathToOperation, requestId);
            return result.CreateActionResult(CrudOperation.Invoke);
        }
      }
}

