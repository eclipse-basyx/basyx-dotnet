/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
using BaSyx.Models.Extensions;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.ResultHandling.ResultTypes;
using BaSyx.Utils.FileSystem;
using Microsoft.Extensions.FileProviders;
using Range = BaSyx.Models.AdminShell.Range;
using System.Text.Json.Serialization;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Submodel Controller
    /// </summary>
    [ApiController]
    public class SubmodelController : Controller
    {
        private readonly ISubmodelServiceProvider serviceProvider;
        private readonly IWebHostEnvironment hostingEnvironment;

        private static JsonSerializerOptions _defaultSerializerOptions;
        private static JsonSerializerOptions _metadataSerializerOptions;
        private static JsonSerializerOptions _fullSerializerOptions;
        static SubmodelController()
        {
            var services = DefaultImplementation.GetStandardServiceCollection();

            DefaultJsonSerializerOptions defaultOptions = new DefaultJsonSerializerOptions();            
            defaultOptions.AddDependencyInjection(new DependencyInjectionExtension(services));
            _defaultSerializerOptions = defaultOptions.Build();

            DefaultJsonSerializerOptions options = new DefaultJsonSerializerOptions();
            options.AddDependencyInjection(new DependencyInjectionExtension(services));
            options.AddMetadataSubmodelElementConverter();
            _metadataSerializerOptions = options.Build();           

            DefaultJsonSerializerOptions options3 = new DefaultJsonSerializerOptions();
            options3.AddDependencyInjection(new DependencyInjectionExtension(services));
            options3.AddFullSubmodelElementConverter();
            _fullSerializerOptions = options3.Build();
        }

        /// <summary>
        /// The constructor for the Submodel Controller
        /// </summary>
        /// <param name="submodelServiceProvider">The Submodel Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public SubmodelController(ISubmodelServiceProvider submodelServiceProvider, IWebHostEnvironment environment)
        {
            serviceProvider = submodelServiceProvider;
            hostingEnvironment = environment;
        }

        /// <summary>
        /// Returns the Submodel
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>   
        [HttpGet(SubmodelRoutes.SUBMODEL, Name = "GetSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]        
        public IActionResult GetSubmodel([FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            var result = serviceProvider.RetrieveSubmodel(level, extent);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates the Submodel
        /// </summary>
        /// <returns></returns>
        /// <param name="submodel">Submodel object</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <response code="204">Submodel updated successfully</response>     
        [HttpPut(SubmodelRoutes.SUBMODEL, Name = "PutSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult PutSubmodel([FromBody] ISubmodel submodel, [FromQuery] RequestLevel level = RequestLevel.Deep, [FromQuery] RequestExtent extent = default)
        {
            if (submodel == null)
                return ResultHandling.NullResult(nameof(submodel));

            var result = serviceProvider.UpdateSubmodel(submodel);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <summary>
        /// Updates the Submodel
        /// </summary>
        /// <returns></returns>
        /// <param name="submodel">Submodel object</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <response code="204">Submodel updated successfully</response>     
        [HttpPatch(SubmodelRoutes.SUBMODEL, Name = "PatchSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult PatchSubmodel([FromBody] ISubmodel submodel, [FromQuery] RequestLevel level = RequestLevel.Core, [FromQuery] RequestExtent extent = default)
        {
            if (submodel == null)
                return ResultHandling.NullResult(nameof(submodel));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the metadata attributes of a specific Submodel
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet(SubmodelRoutes.SUBMODEL + OutputModifier.METADATA, Name = "GetSubmodelMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelMetadata([FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            var result = serviceProvider.RetrieveSubmodel(level, extent);
            if(!result.Success || result.Entity == null)
                return result.CreateActionResult(CrudOperation.Retrieve);

            ISubmodel metadataSubmodel = result.Entity.GetMetadata();
            string json = JsonSerializer.Serialize(metadataSubmodel, _metadataSerializerOptions);
            return Content(json, "application/json");                    
        }

        /// <summary>
        /// Updates the metadata attributes of the Submodel
        /// </summary>
        /// <param name="submodel">The metadata attributes of the Submodel object</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>     
        [HttpPatch(SubmodelRoutes.SUBMODEL + OutputModifier.METADATA, Name = "PatchSubmodelMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult PatchSubmodelMetadata([FromBody] ISubmodel submodel, [FromQuery] RequestLevel level = RequestLevel.Core, [FromQuery] RequestExtent extent = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the Submodel in the ValueOnly representation
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">ValueOnly representation of the Submodel</response>     
        [HttpGet(SubmodelRoutes.SUBMODEL + OutputModifier.VALUE, Name = "GetSubmodelValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelValueOnly([FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            var result = serviceProvider.RetrieveSubmodelElements();
            if (!result.Success || result.Entity == null || result.Entity.Result == null)
                return result.CreateActionResult(CrudOperation.Retrieve);

            JsonObject smValue = new JsonObject();
            var node = JsonSerializer.SerializeToNode(result.Entity.Result, _fullSerializerOptions);
            smValue.Add("submodelElements", node);

            string json = smValue.ToJsonString(); 
            return Content(json, "application/json");
        }

        /// <summary>
        /// Updates the values of the Submodel
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="204">Submodel object in its ValueOnly representation</response>     
        [HttpPatch(SubmodelRoutes.SUBMODEL + OutputModifier.VALUE, Name = "PatchSubmodelValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult PatchSubmodelValueOnly([FromQuery] RequestLevel level = RequestLevel.Core, [FromQuery] RequestExtent extent = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the Reference of the Submodel
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">ValueOnly representation of the Submodel</response>     
        [HttpGet(SubmodelRoutes.SUBMODEL + OutputModifier.REFERENCE, Name = "GetSubmodelReference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelReference([FromQuery] RequestLevel level = RequestLevel.Core)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the Submodel in the Path notation
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Submodel in Path notation</response>     
        [HttpGet(SubmodelRoutes.SUBMODEL + OutputModifier.PATH, Name = "GetSubmodelPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelPath([FromQuery] RequestLevel level = default)
        {
            throw new NotImplementedException();
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
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelAsTable([FromQuery] string columns)
        {
            if (string.IsNullOrEmpty(columns))
                return ResultHandling.NullResult(nameof(columns));

            //var result = serviceProvider.RetrieveSubmodel(RequestLevel.Deep, RequestContent.Normal, RequestExtent.WithoutBlobValue);
            //if (result != null && result.Entity != null)
            //{
            //    string[] columnNames = columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //    JToken customizedSubmodel = result.Entity.CustomizeSubmodel(columnNames);
            //    return new JsonResult(customizedSubmodel);
            //}

            //return result.CreateActionResult(CrudOperation.Retrieve);
            return BadRequest();
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "GetAllSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetAllSubmodelElements([FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            var result = serviceProvider.RetrieveSubmodelElements();
            Result<PagedResult> newPagedResult = new Result<PagedResult>(result.Success, result.Entity, result.Messages);
            return newPagedResult.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a new submodel element
        /// </summary>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="201">Submodel element created successfully</response>
        [HttpPost(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "PostSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 409)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult PostSubmodelElement([FromBody] ISubmodelElement submodelElement, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (submodelElement == null)
                return ResultHandling.NullResult(nameof(submodelElement));

            var result = serviceProvider.CreateSubmodelElement(".", submodelElement);
            return result.CreateActionResult(CrudOperation.Create, SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH.Replace("{idShortPath}", submodelElement.IdShort));
        }

        /// <summary>
        /// Returns the metadata attributes of all submodel elements including their hierarchy
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.METADATA, Name = "GetAllSubmodelElementsMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetAllSubmodelElementsMetadata([FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            var result = serviceProvider.RetrieveSubmodelElements();
            if (!result.Success || result.Entity == null)
                return result.CreateActionResult(CrudOperation.Retrieve);

            string json = JsonSerializer.Serialize(result.Entity, _metadataSerializerOptions);
            return Content(json, "application/json");
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy in the ValueOnly representation
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>  
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.VALUE, Name = "GetAllSubmodelElementsValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetAllSubmodelElementsValueOnly([FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            var result = serviceProvider.RetrieveSubmodelElements();
            if (!result.Success || result.Entity == null || result.Entity.Result == null)
                return result.CreateActionResult(CrudOperation.Retrieve);

            string json = JsonSerializer.Serialize(result.Entity, new JsonSerializerOptions()
            {
                Converters = { new SubmodelElementContainerValueOnlyConverter(_defaultSerializerOptions) }
            });
            return Content(json, "application/json");
        }

        /// <summary>
        /// Returns the References of all submodel elements
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>  
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.REFERENCE, Name = "GetAllSubmodelElementsReference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetAllSubmodelElementsReference([FromQuery] RequestLevel level = RequestLevel.Core, [FromQuery] RequestExtent extent = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy in the Path notation
        /// </summary>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements in the Path notation</response>  
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.PATH, Name = "GetAllSubmodelElementsPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetAllSubmodelElementsPath([FromQuery] RequestLevel level = RequestLevel.Core, [FromQuery] RequestExtent extent = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element</response>  
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "GetSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelElementByPath(string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));

            idShortPath = HttpUtility.UrlDecode(idShortPath);

            var result = serviceProvider.RetrieveSubmodelElement(idShortPath);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a new submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="201">Submodel element created successfully</response>
        [HttpPost(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "PostSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 409)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult PostSubmodelElementByPath(string idShortPath, [FromBody] ISubmodelElement submodelElement, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (submodelElement == null)
                return ResultHandling.NullResult(nameof(submodelElement));

            var result = serviceProvider.CreateSubmodelElement(idShortPath, submodelElement);
            return result.CreateActionResult(CrudOperation.Create, SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH.Replace("{idShortPath}", string.Join(".", idShortPath, submodelElement.IdShort)));
        }

        /// <summary>
        /// Updates an existing submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        [HttpPut(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "PutSubmodelElementByPath")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult PutSubmodelElementByPath(string idShortPath, [FromBody] ISubmodelElement submodelElement, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (submodelElement == null)
                return ResultHandling.NullResult(nameof(submodelElement));

            var result = serviceProvider.UpdateSubmodelElement(idShortPath, submodelElement);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <summary>
        /// Updates an existing SubmodelElement
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        [HttpPatch(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "PatchSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult PatchSubmodelElementByPath(string idShortPath, [FromBody] ISubmodelElement submodelElement, [FromQuery] RequestLevel level = RequestLevel.Core, [FromQuery] RequestExtent extent = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a submodel element at a specified path within the submodel elements hierarchy
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        /// <response code="204">Submodel element deleted successfully</response>
        [HttpDelete(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "DeleteSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult DeleteSubmodelElementByPath(string idShortPath)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));

            idShortPath = HttpUtility.UrlDecode(idShortPath);

            var result = serviceProvider.DeleteSubmodelElement(idShortPath);
            return result.CreateActionResult(CrudOperation.Delete);
        }


        /// <summary>
        /// Returns the matadata attributes of a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        /// <response code="404">Submodel Element not found</response>     
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.METADATA, Name = "GetSubmodelElementByPathMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelElementByPathMetadata(string idShortPath, [FromQuery] RequestLevel level = default)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));

            idShortPath = HttpUtility.UrlDecode(idShortPath);

            var result = serviceProvider.RetrieveSubmodelElement(idShortPath);
            if (result.Success && result.Entity != null)
            {
                string json = JsonSerializer.Serialize(result.Entity, _metadataSerializerOptions);
                return Content(json, "application/json");
            }
            else
                return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates the metadata attributes an existing SubmodelElement
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>  
        [HttpPatch(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.METADATA, Name = "PatchSubmodelElementByPathMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult PatchSubmodelElementByPathMetadata(string idShortPath, [FromQuery] RequestLevel level = RequestLevel.Core)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path in the ValueOnly representation
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, Name = "GetSubmodelElementByPathValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ValueScope), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelElementByPathValueOnly(string idShortPath, [FromQuery] RequestLevel level = RequestLevel.Deep)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));

            idShortPath = HttpUtility.UrlDecode(idShortPath);

            var result = serviceProvider.RetrieveSubmodelElementValue(idShortPath);
            if (result.Success && result.Entity != null)
            {
                string value = JsonSerializer.Serialize<ValueScope>(result.Entity, new JsonSerializerOptions()
                {
                    Converters =
                    {
                        new ValueScopeConverter(options: new ValueScopeConverterOptions()
                        {
                            SerializationOption = SerializationOption.ValueOnly,
                            ValueAsString = false
                        }, jsonOptions: _fullSerializerOptions, level)
                    }
                });
				return Content(value, "application/json");
			}
            else
                return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates the value of an existing submodel element value at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        /// <response code="400">Bad Request</response>
        [HttpPatch(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, Name = "PatchSubmodelElementValueByPathValueOnly")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult PatchSubmodelElementValueByPathValueOnly(string idShortPath, [FromBody] JsonDocument requestBody, [FromQuery] RequestLevel level = RequestLevel.Core)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (requestBody.Equals(default(JsonElement)))
                return ResultHandling.NullResult(nameof(requestBody));

            var sme_retrieved = serviceProvider.RetrieveSubmodelElement(idShortPath);
            if (!sme_retrieved.Success || sme_retrieved.Entity == null)
                return sme_retrieved.CreateActionResult(CrudOperation.Retrieve);

            SubmodelElement sme = sme_retrieved.Entity as SubmodelElement;
            ValueScope valueScope;

            if(sme.ModelType == ModelType.Property)
            {
				Property property = sme as Property;
				valueScope = requestBody.Deserialize<PropertyValue>(new JsonSerializerOptions()
				{
					Converters = { new ValueScopeConverter<PropertyValue>(dataType: property.ValueType) }
				});
			}
            else if (sme.ModelType == ModelType.SubmodelElementCollection)
            {
                valueScope = requestBody.Deserialize<SubmodelElementCollectionValue>(new JsonSerializerOptions()
                {
                    Converters = { new ValueScopeConverter<SubmodelElementCollectionValue>(
                        sme: sme,
                        options: new ValueScopeConverterOptions() { SerializationOption = SerializationOption.ValueOnly },
                        jsonOptions: _fullSerializerOptions) }
                });
            }
            else if (sme.ModelType == ModelType.SubmodelElementList)
            {
                valueScope = requestBody.Deserialize<SubmodelElementListValue>(new JsonSerializerOptions()
                {
                    Converters = { new ValueScopeConverter<SubmodelElementListValue>(
                        sme: sme,
                        options: new ValueScopeConverterOptions() { SerializationOption = SerializationOption.ValueOnly },
                        jsonOptions: _fullSerializerOptions) }
                });
            }
            else if (sme.ModelType == ModelType.Range)
            {
                Range range = sme as Range;
                valueScope = requestBody.Deserialize<RangeValue>(new JsonSerializerOptions() 
                { 
                    Converters = { new ValueScopeConverter<RangeValue>(dataType: range.ValueType) }
                });
            }
            else if (sme.ModelType == ModelType.MultiLanguageProperty)
            {
				valueScope = requestBody.Deserialize<MultiLanguagePropertyValue>(new JsonSerializerOptions()
				{
					Converters = { new ValueScopeConverter<MultiLanguagePropertyValue>() }
				});
			}
			else if (sme.ModelType == ModelType.ReferenceElement)
			{
				valueScope = requestBody.Deserialize<ReferenceElementValue>(new JsonSerializerOptions()
				{
					Converters = { new ValueScopeConverter<ReferenceElementValue>(jsonOptions: _fullSerializerOptions) }
				});
			}
            else if (sme.ModelType == ModelType.BasicEventElement)
            {
                valueScope = requestBody.Deserialize<BasicEventElementValue>(new JsonSerializerOptions()
                {
                    Converters = { new ValueScopeConverter<BasicEventElementValue>(jsonOptions: _fullSerializerOptions) }
                });
            }
            else if (sme.ModelType == ModelType.RelationshipElement)
            {
                valueScope = requestBody.Deserialize<RelationshipElementValue>(new JsonSerializerOptions()
                {
                    Converters = { new ValueScopeConverter<RelationshipElementValue>(jsonOptions: _fullSerializerOptions) }
                });
            }
            else if (sme.ModelType == ModelType.AnnotatedRelationshipElement)
            {
                valueScope = requestBody.Deserialize<AnnotatedRelationshipElementValue>(new JsonSerializerOptions()
                {
                    Converters = { new ValueScopeConverter<AnnotatedRelationshipElementValue>(
                        sme: sme, 
                        options: new ValueScopeConverterOptions() { SerializationOption = SerializationOption.ValueOnly }, 
                        jsonOptions: _fullSerializerOptions) }
                });
            }
            else if (sme.ModelType == ModelType.Entity)
            {
                valueScope = requestBody.Deserialize<EntityValue>(new JsonSerializerOptions()
                {
                    Converters = { new ValueScopeConverter<EntityValue>(
                        sme: sme,
                        options: new ValueScopeConverterOptions() { SerializationOption = SerializationOption.ValueOnly },
                        jsonOptions: _fullSerializerOptions) }
                });
            }
            else if (sme.ModelType == ModelType.File)
            {
                valueScope = requestBody.Deserialize<FileElementValue>(new JsonSerializerOptions()
                {
                    Converters = { new ValueScopeConverter<FileElementValue>() }
                });
            }
            else if (sme.ModelType == ModelType.Blob)
            {
                valueScope = requestBody.Deserialize<BlobValue>(new JsonSerializerOptions()
                {
                    Converters = { new ValueScopeConverter<BlobValue>() }
                });
            }
            else
            {
				return new Result(false, new ErrorMessage("SubmodelElement is unknown or not implemented")).CreateActionResult(CrudOperation.Update);
			}

            var result = serviceProvider.UpdateSubmodelElementValue(idShortPath, valueScope);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <summary>
        /// Returns the Reference of a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.REFERENCE, Name = "GetSubmodelElementByPathReference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelElementByPathReference(string idShortPath, [FromQuery] RequestLevel level = RequestLevel.Core)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path in the Path notation
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.PATH, Name = "GetSubmodelElementByPathPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelElementByPathPath(string idShortPath, [FromQuery] RequestLevel level = RequestLevel.Core)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Downloads file content from a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <returns></returns>
        /// <response code="200">Requested file</response>
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "GetFileByPath")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetFileByPath(string idShortPath)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));

            var fileElementRetrieved = serviceProvider.RetrieveSubmodelElement(idShortPath);
            if (!fileElementRetrieved.Success || fileElementRetrieved.Entity == null)
                return fileElementRetrieved.CreateActionResult(CrudOperation.Retrieve);

            IFileElement fileElement = fileElementRetrieved.Entity.Cast<IFileElement>();
            string fileName = fileElement.Value.Value.TrimStart('/');

            IFileProvider fileProvider = hostingEnvironment.ContentRootFileProvider;
            var file = fileProvider.GetFileInfo(fileName);
            if (file.Exists)
            {
                if (MimeTypes.TryGetContentType(file.PhysicalPath, out string contentType))
                {
                    string fileNameOnly = Path.GetFileName(file.PhysicalPath);
                    return File(file.CreateReadStream(), contentType, fileNameOnly);
                }                   
            }
            return NotFound();
        }


        /// <summary>
        /// Uploads file content to an existing submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <param name="file">Content to upload</param>
        /// <returns></returns>
        /// <response code="200">Content uploaded successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">File not found</response>
        [HttpPut(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "PutFileByPath")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> PutFileByPath(string idShortPath, IFormFile file)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (file == null)
                return ResultHandling.NullResult(nameof(file));

            var fileElementRetrieved = serviceProvider.RetrieveSubmodelElement(idShortPath);
            if(!fileElementRetrieved.Success || fileElementRetrieved.Entity == null)
                return fileElementRetrieved.CreateActionResult(CrudOperation.Retrieve);

            IFileElement fileElement = fileElementRetrieved.Entity.Cast<IFileElement>();
            string fileName = fileElement.Value.Value.TrimStart('/');
            string filePath = Path.Combine(hostingEnvironment.ContentRootPath, fileName);
            
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return Ok();
        }

        /// <summary>
        /// Deletes file content of an existing submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <returns></returns>
        /// <response code="200">File deleted successfully</response>
        [HttpDelete(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "DeleteFileByPath")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult DeleteFileByPath(string idShortPath)
        {
           throw new NotImplementedException();
        }

        /// <summary>
        /// Synchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        [HttpPost(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE, Name = "InvokeOperation")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult InvokeOperationSync(string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (operationRequest == null)
                return ResultHandling.NullResult(nameof(operationRequest));

            IResult<InvocationResponse> result = serviceProvider.InvokeOperation(idShortPath, operationRequest, false);
            return result.CreateActionResult(CrudOperation.Invoke);
        }

        /// <summary>
        /// Asynchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        [HttpPost(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC, Name = "InvokeOperationAsync")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(202)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult InvokeOperationAsync(string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (operationRequest == null)
                return ResultHandling.NullResult(nameof(operationRequest));

            IResult<InvocationResponse> result = serviceProvider.InvokeOperation(idShortPath, operationRequest, true);
            return result.CreateActionResult(CrudOperation.Invoke);
        }

        /// <summary>
        /// Synchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        [HttpPost(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE + OutputModifier.VALUE, Name = "InvokeOperationSyncValueOnly")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult InvokeOperationSyncValueOnly(string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (operationRequest == null)
                return ResultHandling.NullResult(nameof(operationRequest));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        [HttpPost(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC + OutputModifier.VALUE, Name = "InvokeOperationAsyncValueOnly")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult InvokeOperationAsyncValueOnly(string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (operationRequest == null)
                return ResultHandling.NullResult(nameof(operationRequest));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the Operation status of an asynchronous invoked Operation
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation’s asynchronous invocation used to request the current state of the operation’s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_STATUS, Name = "GetOperationAsyncStatus")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 302)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetOperationAsyncStatus(string idShortPath, string handleId)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (string.IsNullOrEmpty(handleId))
                return ResultHandling.NullResult(nameof(handleId));

           throw new NotImplementedException();
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
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS, Name = "GetOperationAsyncResult")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetOperationAsyncResult(string idShortPath, string handleId)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (string.IsNullOrEmpty(handleId))
                return ResultHandling.NullResult(nameof(handleId));

            IResult<InvocationResponse> result = serviceProvider.GetInvocationResult(idShortPath, handleId);
            return result.CreateActionResult(CrudOperation.Invoke);
        }

        /// <summary>
        /// Returns the Operation result of an asynchronous invoked Operation
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation’s asynchronous invocation used to request the current state of the operation’s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS + OutputModifier.VALUE, Name = "GetOperationAsyncResultValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetOperationAsyncResultValueOnly(string idShortPath, string handleId)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return ResultHandling.NullResult(nameof(idShortPath));
            if (string.IsNullOrEmpty(handleId))
                return ResultHandling.NullResult(nameof(handleId));

            throw new NotImplementedException();
        }
    }
}

