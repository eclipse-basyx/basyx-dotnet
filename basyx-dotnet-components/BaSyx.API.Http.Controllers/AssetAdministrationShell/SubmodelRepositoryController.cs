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
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using BaSyx.API.ServiceProvider;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BaSyx.Utils.ResultHandling.ResultTypes;
using BaSyx.Models.Extensions;
using BaSyx.Utils.DependencyInjection;
using System.Text.Json.Nodes;
using BaSyx.Models.Extensions.JsonConverters;

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

        private static JsonSerializerOptions _defaultSerializerOptions;
        private static JsonSerializerOptions _metadataSerializerOptions;
        private static JsonSerializerOptions _fullSerializerOptions;

        /// <summary>
        /// The constructor for the Submodel Repository Controller
        /// </summary>
        /// <param name="submodelRepositoryServiceProvider"></param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public SubmodelRepositoryController(ISubmodelRepositoryServiceProvider submodelRepositoryServiceProvider, IWebHostEnvironment environment)
        {
            serviceProvider = submodelRepositoryServiceProvider;
            hostingEnvironment = environment;

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
        /// Returns all Submodels
        /// </summary>
        /// <param name="semanticId">The value of the semantic id reference (BASE64-URL-encoded)</param>
        /// <param name="idShort">The Asset Administration Shell’s IdShort</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns>Requested Submodels</returns>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODELS, Name = "GetAllSubmodels")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult<List<Submodel>>), 200)]
        public IActionResult GetAllSubmodels([FromQuery] string semanticId = "", [FromQuery] string idShort = "", [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            cursor = ResultHandling.Base64UrlDecode(cursor);
            semanticId = ResultHandling.Base64UrlDecode(semanticId);

            var result = serviceProvider.RetrieveSubmodels(limit, cursor, semanticId, idShort);

            var jsonOptions = new GlobalJsonSerializerOptions().Build();
            jsonOptions.Converters.Add(new ElementContainerConverter(new ConverterOptions()
            {
                ValueSerialization = true,
                RequestLevel = level,
                RequestExtent = extent
            }));

            string json = JsonSerializer.Serialize(result.Entity, jsonOptions);
            return Content(json, "application/json");
        }

        /// <summary>
        /// Returns the metadata attributes of all Submodels
        /// </summary>
        /// <param name="semanticId">The value of the semantic id reference (BASE64-URL-encoded)</param>
        /// <param name="idShort">The Asset Administration Shell’s IdShort</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue (BASE64-URL-encoded)</param>
        /// <returns>Requested Submodels</returns>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODELS + OutputModifier.METADATA, Name = "GetAllSubmodels-Metadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult<List<Submodel>>), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetAllSubmodelsMetadata([FromQuery] string semanticId = "", [FromQuery] string idShort = "", [FromQuery] int limit = 100, [FromQuery] string cursor = "")
        {
            cursor = ResultHandling.Base64UrlDecode(cursor);
            semanticId = ResultHandling.Base64UrlDecode(semanticId);

            var result = serviceProvider.RetrieveSubmodelsMetadata(limit, cursor, semanticId, idShort);
            if (!result.Success || result.Entity == null)
                return result.CreateActionResult(CrudOperation.Retrieve);

            var jsonOptions = _metadataSerializerOptions;
            string json = JsonSerializer.Serialize(result.Entity, jsonOptions);
            return Content(json, "application/json");
        }

        /// <summary>
        /// Returns all Submodels in their ValueOnly representation
        /// </summary>
        /// <param name="semanticId">The value of the semantic id reference (BASE64-URL-encoded)</param>
        /// <param name="idShort">The Asset Administration Shell’s IdShort</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodels in their ValueOnly representation</response>  
        [HttpGet(SubmodelRepositoryRoutes.SUBMODELS + OutputModifier.VALUE, Name = "GetAllSubmodels-ValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetAllSubmodelsValueOnly([FromQuery] string semanticId = "", [FromQuery] string idShort = "", [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            cursor = ResultHandling.Base64UrlDecode(cursor);
            semanticId = ResultHandling.Base64UrlDecode(semanticId);

            var result = serviceProvider.RetrieveSubmodels(limit, cursor, semanticId, idShort);
            if (!result.Success || result.Entity == null || result.Entity.Result == null)
                return result.CreateActionResult(CrudOperation.Retrieve);

            JsonArray allSmValues = new JsonArray();
            var jsonOptions = new GlobalJsonSerializerOptions().Build();
            jsonOptions.Converters.Add(new SubmodelElementContainerValueOnlyConverter(_defaultSerializerOptions, new SubmodelElementContainerValueOnlyConverterOptions()
            {
                RequestLevel = level,
                RequestExtent = extent
            }));

            foreach (var submodel in result.Entity.Result)
            {
                var smValue = new JsonObject();
                var node = JsonSerializer.SerializeToNode(submodel.SubmodelElements, jsonOptions);
                string smIdShort = submodel.IdShort;
                smValue.Add(smIdShort, node);
                allSmValues.Add(smValue);
            }

            var pagedSmValues = new PagedResult<JsonArray>(allSmValues, result.Entity.PagingMetadata);
            var valueResult = new Result<PagedResult<JsonArray>>(true, pagedSmValues, new EmptyMessage());
            return valueResult.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Returns the References for all Submodels
        /// </summary>
        /// <param name="semanticId">The value of the semantic id reference (BASE64-URL-encoded)</param>
        /// <param name="idShort">The Asset Administration Shell’s IdShort</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">References of the requested Submodels</response>     
        [HttpGet(SubmodelRepositoryRoutes.SUBMODELS + OutputModifier.REFERENCE, Name = "GetAllSubmodels-Reference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetAllSubmodelsReference([FromQuery] string semanticId = "", [FromQuery] string idShort = "", [FromQuery] int limit = 100, [FromQuery] string cursor = "")
        {
            cursor = ResultHandling.Base64UrlDecode(cursor);
            semanticId = ResultHandling.Base64UrlDecode(semanticId);

            var result = serviceProvider.RetrieveSubmodelsReference(limit, cursor, semanticId, idShort); 
            return result.CreateActionResult(CrudOperation.Retrieve);
        }


        /// <summary>
        /// Returns the Paths for all Submodels
        /// </summary>
        /// <param name="semanticId">The value of the semantic id reference (BASE64-URL-encoded)</param>
        /// <param name="idShort">The Asset Administration Shell’s IdShort</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">References of the requested Submodels</response>     
        [HttpGet(SubmodelRepositoryRoutes.SUBMODELS + OutputModifier.PATH, Name = "GetAllSubmodelsPath-Path")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetAllSubmodelsPath([FromQuery] string semanticId = "", [FromQuery] string idShort = "", [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default)
        {
            cursor = ResultHandling.Base64UrlDecode(cursor);
            semanticId = ResultHandling.Base64UrlDecode(semanticId);

            var result = serviceProvider.RetrieveSubmodels(limit, cursor, semanticId, idShort);
            if (!result.Success || result.Entity == null)
                return result.CreateActionResult(CrudOperation.Retrieve);

            JsonArray allSmPath = new JsonArray();
            var jsonOptions = new GlobalJsonSerializerOptions().Build();
            jsonOptions.Converters.Add(new SubmodelElementContainerPathConverter(new PathConverterOptions()
            {
                RequestLevel = level,
                EncloseInBrackets = false
            }));

            foreach (var submodel in result.Entity.Result)
            {
                var smValue = new JsonObject();
                var node = JsonSerializer.SerializeToNode(submodel.SubmodelElements, jsonOptions);
                string smIdShort = submodel.IdShort;
                smValue.Add(smIdShort, node);
                allSmPath.Add(smValue);
            }

            var pagedSmValues = new PagedResult<JsonArray>(allSmPath, result.Entity.PagingMetadata);
            var valueResult = new Result<PagedResult<JsonArray>>(true, pagedSmValues, new EmptyMessage());
            return valueResult.CreateActionResult(CrudOperation.Retrieve);
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
        /// <inheritdoc cref="SubmodelController.GetSubmodel"/> 
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID, Name = "GetSubmodelById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetSubmodelById(string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodel(level, extent);
        }

        /// <summary>
        /// Replace Replace an existing Submodel
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

            var result = serviceProvider.UpdateSubmodel(submodelIdentifier, submodel);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <summary>
        /// Updates an existing Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodel">Submodel object</param>
        /// <returns></returns>
        /// <response code="204">Submodel updated successfully</response>
        /// <inheritdoc cref="SubmodelController.PatchSubmodel(ISubmodel)"/>
        [HttpPatch(SubmodelRepositoryRoutes.SUBMODEL_BYID, Name = "PatchSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult PatchSubmodel(string submodelIdentifier, [FromBody] ISubmodel submodel)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodel(submodel);
        }


        /// <summary>
        /// Deletes an existing Submodel
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

        /// <summary>
        /// Returns the metadata attributes of a specific Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel in the metadata representation</response>
        /// <inheritdoc cref="SubmodelController.GetSubmodelMetadata()"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + OutputModifier.METADATA, Name = "GetSubmodelById-Metadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetSubmodelMetadata(string submodelIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelMetadata();
        }

        /// <summary>
        /// Updates the metadata attributes of the Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodel">The metadata attributes of the Submodel object</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <inheritdoc cref="SubmodelController.PatchSubmodelMetadata(ISubmodel)"/> 
        [HttpPatch(SubmodelRepositoryRoutes.SUBMODEL_BYID + OutputModifier.METADATA, Name = "SubmodelRepo_PatchSubmodelMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_PatchSubmodelMetadata(string submodelIdentifier, [FromBody] ISubmodel submodel)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodelMetadata(submodel);
        }
        
        /// <summary>
        /// Returns a specific Submodel in the ValueOnly representation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">ValueOnly representation of the requested Submodel</response>    
        /// <inheritdoc cref="SubmodelController.GetSubmodelValueOnly(RequestLevel, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + OutputModifier.VALUE, Name = "GetSubmodelById-ValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetSubmodelValue(string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelValueOnly(level, extent);
        }

        /// <summary>
        /// Updates the values of the Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel object in its ValueOnly representation</response>     
        /// <inheritdoc cref="SubmodelController.PatchSubmodelValueOnly(JsonDocument)"/>
        [HttpPatch(SubmodelRepositoryRoutes.SUBMODEL_BYID + OutputModifier.VALUE, Name = "SubmodelRepo_PatchSubmodelValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_PatchSubmodelValueOnly(string submodelIdentifier, [FromBody] JsonDocument requestBody)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodelValueOnly(requestBody);
        }

        /// <summary>
        /// Returns the Reference of a specific Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>    
        /// <inheritdoc cref="SubmodelController.GetSubmodelReference()"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + OutputModifier.REFERENCE, Name = "GetSubmodelById-Reference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetSubmodelReference(string submodelIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelReference();
        }

        /// <summary>
        /// Returns a specific Submodel in the Path notation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <inheritdoc cref="SubmodelController.GetSubmodelPath(RequestLevel)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + OutputModifier.PATH, Name = "GetSubmodelById-Path")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetSubmodelPath(string submodelIdentifier, [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelPath(level);
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>
        /// <response code="404">Submodel not found</response>
        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElements(int, string, RequestLevel, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "GetAllSubmodelElements_SubmodelRepository")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetAllSubmodelElements(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElements(limit, cursor, level, extent);
        }

        /// <summary>
        /// Returns the metadata attributes of all submodel elements including their hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements in the metadata representation</response>
        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElementsMetadata(int, string, RequestLevel)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.METADATA, Name = "GetAllSubmodelElements-Metadata_SubmodelRepository")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetAllSubmodelElementsMetadata(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElementsMetadata(limit, cursor, level);
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy in the Path notation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements in the Path notation</response>
        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElementsPath(int, string, RequestLevel)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.PATH, Name = "GetAllSubmodelElements-Path_SubmodelRepo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetAllSubmodelElementsPath(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElementsPath(limit, cursor, level);
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy in the ValueOnly representation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements in the ValueOnly representation</response>
        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElementsValueOnly(int, string, RequestLevel, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.VALUE, Name = "GetAllSubmodelElements-ValueOnly_SubmodelRepo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetAllSubmodelElementsValueOnly(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElementsValueOnly(limit, cursor, level, extent);
        }

        /// <summary>
        /// Returns the References of all submodel elements
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>
        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElementsReference(int, string)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.REFERENCE, Name = "GetAllSubmodelElements-Reference_SubmodelRepo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetAllSubmodelElementsReference(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "")
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElementsReference(limit, cursor);

        }

        /// <summary>
        /// Creates a new submodel element
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="201">Submodel element created successfully</response>
        /// <inheritdoc cref="SubmodelController.PostSubmodelElement(ISubmodelElement)"/>
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

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element</response>  
        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPath(string, RequestLevel, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "GetSubmodelElementByPath_SubmodelRepo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPath(idShortPath, level, extent);
        }

        /// <summary>
        /// Returns the metadata attributes of a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Metadata attributes of the requested submodel element</response>
        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPathMetadata(string, RequestLevel)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.METADATA, Name = "GetSubmodelElementByPath-Metadata_SubmodelRepo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetSubmodelElementByPathMetadata(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPathMetadata(idShortPath, level);
        }

        /// <summary>
        /// Updates the metadata attributes an existing SubmodelElement
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Metadata attributes of the SubmodelElement</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response> 
        /// <inheritdoc cref="SubmodelController.PatchSubmodelElementByPathMetadata(string, ISubmodelElement)"/> 
        [HttpPatch(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.METADATA,
            Name = "SubmodelRepo_PatchSubmodelElementByPathMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_PatchSubmodelElementByPathMetadata(string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodelElementByPathMetadata(idShortPath, submodelElement);
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path in the ValueOnly representation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPathValueOnly(string, RequestLevel, RequestExtent)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, Name = "GetSubmodelElementByPath-ValueOnly_SubmodelRepo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetSubmodelElementByPathValueOnly(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPathValueOnly(idShortPath, level, extent);
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path in the Path notation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in the Path notation</response>
        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPathPath(string, RequestLevel)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.PATH, Name = "GetSubmodelElementByPath-Path_SubmodelRepo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetSubmodelElementByPathPath(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPathPath(idShortPath, level);
        }

        /// <summary>
        /// Returns the Reference of a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (UTF8-BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        /// <response code="200">A Reference of the requested submodel element</response>
        /// /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPathReference(string)"/>
        [HttpGet(SubmodelRoutes.SUBMODEL + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.REFERENCE, Name = "GetSubmodelElementByPath-Reference_SubmodelRepo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetSubmodelElementByPathReference(string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPathReference(idShortPath);
        }

        /// <summary>
        /// Creates a new submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="201">Submodel element created successfully</response>
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

        /// <summary>
        /// Replaces an existing submodel element at a specified path within the submodel element hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
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

        /// <summary>
        /// Updates an existing SubmodelElement
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        /// <inheritdoc cref="SubmodelController.PatchSubmodelElementByPath(string, ISubmodelElement)"/>
        [HttpPatch(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH,
            Name = "SubmodelRepo_PatchSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_PatchSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodelElementByPath(idShortPath, submodelElement);
        }

        /// <summary>
        /// Deletes a submodel element at a specified path within the submodel elements hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        /// <response code="204">Submodel element deleted successfully</response>
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

        /// <summary>
        /// Updates the value of an existing submodel element value at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        /// <response code="400">Bad Request</response>
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

        /// <summary>
        /// Downloads file content from a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <returns></returns>
        /// <response code="200">Requested file</response>
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

        /// <summary>
        /// Uploads file content to an existing submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <param name="file">Content to upload</param>
        /// <returns></returns>
        /// <response code="200">Content uploaded successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">File not found</response>
        /// <inheritdoc cref="SubmodelController.PutFileByPath(string, IFormFile)"/>
        [HttpPut(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "SubmodelRepo_PutFileByPath")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> SubmodelRepo_PutFileByPath(string submodelIdentifier, string idShortPath, IFormFile file)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return await service.PutFileByPath(idShortPath, file);
        }

        /// <summary>
        /// Deletes file content of an existing submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <returns></returns>
        /// <response code="200">File deleted successfully</response>
        /// <inheritdoc cref="SubmodelController.DeleteFileByPath(string)"/>
        [HttpDelete(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT,
            Name = "SubmodelRepo_DeleteFileByPath")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult SubmodelRepo_DeleteFileByPath(string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.DeleteFileByPath(idShortPath);
        }

        /// <summary>
        /// Synchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
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

        /// <summary>
        /// Asynchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
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

        /// <summary>
        /// Synchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <inheritdoc cref="SubmodelController.InvokeOperationSyncValueOnly(string, InvocationRequest)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE + OutputModifier.VALUE, Name = "SubmodelRepo_InvokeOperationSyncValueOnly")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_InvokeOperationSyncValueOnly(string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperationSyncValueOnly(idShortPath, operationRequest);
        }

        /// <summary>
        /// Asynchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <inheritdoc cref="SubmodelController.InvokeOperationAsyncValueOnly(string, InvocationRequest)"/>
        [HttpPost(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC + OutputModifier.VALUE, Name = "SubmodelRepo_InvokeOperationAsyncValueOnly")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_InvokeOperationAsyncValueOnly(string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperationAsyncValueOnly(idShortPath, operationRequest);
        }

        /// <summary>
        /// Returns the Operation status of an asynchronous invoked Operation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation’s asynchronous invocation used to request the current state of the operation’s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
        /// <inheritdoc cref="SubmodelController.GetOperationAsyncStatus(string, string)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_STATUS, Name = "SubmodelRepo_GetOperationAsyncStatus")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 302)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetOperationAsyncStatus(string submodelIdentifier, string idShortPath, string handleId)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetOperationAsyncStatus(idShortPath, handleId); ;
        }

        /// <summary>
        /// Returns the Operation result of an asynchronous invoked Operation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation’s asynchronous invocation used to request the current state of the operation’s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
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

        /// <summary>
        /// Returns the Operation result of an asynchronous invoked Operation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation’s asynchronous invocation used to request the current state of the operation’s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
        /// <inheritdoc cref="SubmodelController.GetOperationAsyncResultValueOnly(string, string)"/>
        [HttpGet(SubmodelRepositoryRoutes.SUBMODEL_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS + OutputModifier.VALUE, Name = "SubmodelRepo_GetOperationAsyncResultValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult SubmodelRepo_GetOperationAsyncResultValueOnly(string submodelIdentifier, string idShortPath, string handleId)
        {
            if (serviceProvider.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetOperationAsyncResultValueOnly(idShortPath, handleId);
        }
        #endregion     
    }
}
