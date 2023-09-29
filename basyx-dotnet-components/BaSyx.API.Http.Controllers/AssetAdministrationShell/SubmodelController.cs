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
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using BaSyx.API.ServiceProvider;
using System;
using Newtonsoft.Json.Linq;
using BaSyx.Models.Extensions;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.Json;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Submodel Controller
    /// </summary>
    [ApiController]
    public class SubmodelController : Controller
    {
        private readonly ISubmodelServiceProvider serviceProvider;

        private static JsonSerializer _serializer = JsonSerializer.Create(new DependencyInjectionJsonSerializerSettings());

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
        /// Returns the Submodel
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="content">Determines the request or response kind of the resource</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet(SubmodelRoutes.SUBMODEL, Name = "GetSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> GetSubmodel([FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            var result = serviceProvider.RetrieveSubmodel(level, content, extent);

            if (result != null && result.Entity != null && content == RequestContent.Value)
            {
                JObject minimizedSubmodel = await result.Entity.MinimizeSubmodelAsync();
                return new JsonResult(minimizedSubmodel);
            }

            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates the Submodel
        /// </summary>
        /// <returns></returns>
        /// <param name="submodel">Submodel object</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="content">Determines the request or response kind of the resource</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <response code="204">Submodel updated successfully</response>
        /// <response code="404">Submodel not found</response>       
        [HttpPut(SubmodelRoutes.SUBMODEL, Name = "PutSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult PutSubmodel([FromBody] JObject submodel, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (submodel == null)
                return ResultHandling.NullResult(nameof(submodel));

            var deserialized = submodel.ToObject<ISubmodel>(_serializer);

            var result = serviceProvider.UpdateSubmodel(deserialized);
            return result.CreateActionResult(CrudOperation.Update);
        }


        /// <summary>
        /// Returns a customizable table version of a Submodel
        /// </summary>
        /// <param name="columns">A comma-separated list of field names to structure the payload beeing returned</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <response code="404">Submodel not found</response>   
        [HttpGet(SubmodelRoutes.SUBMODEL_TABLE, Name = "GetSubmodelAsTable")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetSubmodelAsTable([FromQuery] string columns)
        {
            if (string.IsNullOrEmpty(columns))
                return ResultHandling.NullResult(nameof(columns));

            var result = serviceProvider.RetrieveSubmodel(RequestLevel.Deep, RequestContent.Normal, RequestExtent.WithoutBlobValue);
            if (result != null && result.Entity != null)
            {
                string[] columnNames = columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                JToken customizedSubmodel = result.Entity.CustomizeSubmodel(columnNames);
                return new JsonResult(customizedSubmodel);
            }

            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="content">Determines the request or response kind of the resource</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet(SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "GetAllSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetAllSubmodelElements([FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            var result = serviceProvider.RetrieveSubmodelElements();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a new submodel element
        /// </summary>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="content">Determines the request or response kind of the resource</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="201">Submodel element created successfully</response>
        /// <response code="400">Bad Request</response>
        [HttpPost(SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "PostSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult PostSubmodelElement([FromBody] JObject submodelElement, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (submodelElement == null)
                return ResultHandling.NullResult(nameof(submodelElement));

            var deserialized = submodelElement.ToObject<ISubmodelElement>(_serializer);

            var result = serviceProvider.CreateSubmodelElement(".", deserialized);
            return result.CreateActionResult(CrudOperation.Create, SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH.Replace("{idShortPath}", deserialized.IdShort));
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="content">Determines the request or response kind of the resource</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element</response>
        /// <response code="404">Submodel Element not found</response>     
        [HttpGet(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "GetSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetSubmodelElementByPath(string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));

            idShortPath = HttpUtility.UrlDecode(idShortPath);

            if (content == RequestContent.Value)
            {
                var result = serviceProvider.RetrieveSubmodelElementValue(idShortPath);
                if (result.Success && result.Entity != null)
                    return new OkObjectResult(result.Entity.Value);
                else
                    return result.CreateActionResult(CrudOperation.Retrieve);
            }
            else
            {
                var result = serviceProvider.RetrieveSubmodelElement(idShortPath);
                return result.CreateActionResult(CrudOperation.Retrieve);
            }           
        }

        /// <summary>
        /// Creates a new submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="content">Determines the request or response kind of the resource</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="201">Submodel element created successfully</response>
        /// <response code="400">Bad Request</response>
        [HttpPost(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "PostSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult PostSubmodelElementByPath(string idShortPath, [FromBody] JObject submodelElement, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (submodelElement == null)
                return ResultHandling.NullResult(nameof(submodelElement));

            var deserialized = submodelElement.ToObject<ISubmodelElement>(_serializer);

            var result = serviceProvider.CreateSubmodelElement(idShortPath, deserialized);
            return result.CreateActionResult(CrudOperation.Create, SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH.Replace("{idShortPath}", string.Join(".", idShortPath, deserialized.IdShort)));
        }

        /// <summary>
        /// Updates an existing submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="content">Determines the request or response kind of the resource</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        /// <response code="400">Bad Request</response>
        [HttpPut(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "PutSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult PutSubmodelElementByPath(string idShortPath, [FromBody] JToken requestBody, [FromQuery] RequestLevel level = default, [FromQuery] RequestContent content = default, [FromQuery] RequestExtent extent = default)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (requestBody == null)
                return ResultHandling.NullResult(nameof(requestBody));

            //Todo: Check dataType conformity, e.g. define long Property and send string as value update
            if(content == RequestContent.Value)
            {
                JValue jValue = (JValue)requestBody;
                ElementValue elementValue = new ElementValue(jValue.Value, jValue.Value.GetType());
                var result = serviceProvider.UpdateSubmodelElementValue(idShortPath, elementValue);
                return result.CreateActionResult(CrudOperation.Update);
            }
            else
            {
                ISubmodelElement submodelElement = requestBody.ToObject<ISubmodelElement>(_serializer);
                var result = serviceProvider.UpdateSubmodelElement(idShortPath, submodelElement);
                return result.CreateActionResult(CrudOperation.Update);
            }
        }

        /// <summary>
        /// Deletes a submodel element at a specified path within the submodel elements hierarchy
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        /// <response code="204">Submodel element deleted successfully</response>
        /// <response code="404">Submodel element not found</response>
        [HttpDelete(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "DeleteSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult DeleteSubmodelElementByPath(string idShortPath)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));

            idShortPath = HttpUtility.UrlDecode(idShortPath);

            var result = serviceProvider.DeleteSubmodelElement(idShortPath);
            return result.CreateActionResult(CrudOperation.Delete);
        }

        /// <summary>
        /// Uploads the content to the file submodel element
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <param name="file">Content to upload</param>
        /// <returns></returns>
        /// <response code="200">Content uploaded successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">File not found</response>
        [HttpPost(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_UPLOAD, Name = "UploadFileContentByIdShort")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> UploadFileContentByIdShort(string idShortPath, IFormFile file)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (file == null)
                return ResultHandling.NullResult(nameof(file));

            var fileElementRetrieved = serviceProvider.RetrieveSubmodelElement(idShortPath);
            if(!fileElementRetrieved.Success || fileElementRetrieved.Entity == null)
                return fileElementRetrieved.CreateActionResult(CrudOperation.Retrieve);

            IFileElement fileElement = fileElementRetrieved.Entity.Cast<IFileElement>();
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
        /// Synchronously or asynchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <param name="async">Determines whether an operation invocation is performed asynchronously or synchronously</param>
        /// <param name="content">Determines the request or response kind of the resource</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Operation / Method handler not found</response>
        [HttpPost(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE, Name = "InvokeOperation")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult InvokeOperation(string idShortPath, [FromBody] JObject operationRequest, [FromQuery] bool async = false, [FromQuery] RequestContent content = default)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (operationRequest == null)
                return ResultHandling.NullResult(nameof(operationRequest));

            var opRequest = operationRequest.ToObject<InvocationRequest>(_serializer);
            _serializer.Context = new System.Runtime.Serialization.StreamingContext().AddData("serializationMode", "full");

            IResult<InvocationResponse> result = serviceProvider.InvokeOperation(idShortPath, opRequest, async);
            return result.CreateActionResult(CrudOperation.Invoke);
        }

        /// <summary>
        /// Returns the Operation result of an asynchronous invoked Operation
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation’s asynchronous invocation used to request the current state of the operation’s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Operation / Request not found</response>
        [HttpGet(SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS, Name = "GetOperationAsyncResult")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetOperationAsyncResult(string idShortPath, string handleId)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (string.IsNullOrEmpty(handleId))
                return ResultHandling.NullResult(nameof(handleId));

            IResult<InvocationResponse> result = serviceProvider.GetInvocationResult(idShortPath, handleId);
            return result.CreateActionResult(CrudOperation.Invoke);
        }
      }
}

