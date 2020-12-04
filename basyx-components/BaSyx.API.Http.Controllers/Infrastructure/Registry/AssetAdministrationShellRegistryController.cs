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
using BaSyx.API.Components;
using BaSyx.Utils.ResultHandling;
using System.Web;
using BaSyx.Models.Connectivity.Descriptors;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Asset Administration Shell Registry Controller
    /// </summary>
    [ApiController]
    public class AssetAdministrationShellRegistryController : Controller
    {
        private readonly IAssetAdministrationShellRegistry serviceProvider;

        /// <summary>
        /// The constructor for the Asset Administration Shell Registry Controller
        /// </summary>
        /// <param name="aasRegistry">The backend implementation for the IAssetAdministrationShellRegistry interface. Usually provided by the Depedency Injection mechanism.</param>
        public AssetAdministrationShellRegistryController(IAssetAdministrationShellRegistry aasRegistry)
        {
            serviceProvider = aasRegistry;
        }

        /// <summary>
        /// Retrieves all registered Asset Administration Shells within system (e.g. Station, Line, Plant, Area, etc.) defined by the Registry
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns a list of found Asset Administration Shell Descriptors</response>        
        [HttpGet("api/v1/registry", Name = "GetAllAssetAdministrationShellDescriptors")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<AssetAdministrationShellDescriptor>), 200)]
        public IActionResult GetAllAssetAdministrationShellDescriptors()
        {
            var result = serviceProvider.RetrieveAllAssetAdministrationShellRegistrations();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }
        /// <summary>
        /// Retrieves a specific Asset Administration Shell registration
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Asset Administration Shell</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="404">No Asset Administration Shell with passed id found</response>     
        [HttpGet("api/v1/registry/{aasId}", Name = "GetAssetAdministrationShellDescriptor")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AssetAdministrationShellDescriptor), 200)]
        public IActionResult GetAssetAdministrationShellDescriptor(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return ResultHandling.NullResult(nameof(aasId));

            aasId = HttpUtility.UrlDecode(aasId);
            var result = serviceProvider.RetrieveAssetAdministrationShellRegistration(aasId);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a new or updates an existing Asset Administration Shell registration at the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="aasDescriptor">The Asset Administration Shell Descriptor</param>
        /// <returns></returns>
        /// <response code="200">The Asset Administration Shell's registration was successfully renewed</response>
        /// <response code="400">The syntax of the passed Asset Administration Shell is not valid or malformed request</response>    
        /// <response code="404">No Asset Administration Shell with passed id found</response>      
        [HttpPut("api/v1/registry/{aasId}", Name = "RegisterAssetAdministrationShell")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 204)]
        public IActionResult RegisterAssetAdministrationShell(string aasId, [FromBody] IAssetAdministrationShellDescriptor aasDescriptor)
        {
            if (string.IsNullOrEmpty(aasId))
                return ResultHandling.NullResult(nameof(aasId));
            if(aasDescriptor.Identification == null || string.IsNullOrEmpty(aasDescriptor.Identification.Id))
                return ResultHandling.NullResult("The identification property of the Asset Administration Shell Descriptor is null or empty");

            aasId = HttpUtility.UrlDecode(aasId);

            if (aasId != aasDescriptor.Identification.Id)
                return ResultHandling.BadRequestResult($"Path parameter {aasId} does not equal Asset Administration Shell Descriptor identification property {aasDescriptor.Identification.Id}");
            
            var result = serviceProvider.CreateOrUpdateAssetAdministrationShellRegistration(aasId, aasDescriptor);
            return result.CreateActionResult(CrudOperation.Create, "api/v1/registry/" + HttpUtility.UrlEncode(aasId));
        }

        /// <summary>
        /// Deletes the Asset Administration Shell registration from the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <returns></returns>
        /// <response code="200">The Asset Administration Shell was deleted successfully</response>
        /// <response code="400">Bad Request</response>  
        /// <response code="404">No Asset Administration Shell with passed id found</response>     
        [HttpDelete("api/v1/registry/{aasId}", Name = "UnregisterAssetAdministrationShell")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 204)]
        public IActionResult UnregisterAssetAdministrationShell(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return ResultHandling.NullResult(nameof(aasId));

            aasId = HttpUtility.UrlDecode(aasId);
            var result = serviceProvider.DeleteAssetAdministrationShellRegistration(aasId);
            return result.CreateActionResult(CrudOperation.Delete);
        }


        /// <summary>
        /// Creates a new or updates an existing Submodel registration at a specific Asset Administration Shell registered at the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="submodelDescriptor">The Submodel Descriptor</param>
        /// <returns></returns>
        /// <response code="201">The Submodel was created successfully</response>
        /// <response code="400">The syntax of the passed Submodel is not valid or malformed request</response>      
        /// <response code="404">No Asset Administration Shell with passed id found</response>   
        [HttpPut("api/v1/registry/{aasId}/submodels/{submodelId}", Name = "RegisterSubmodelAtAssetAdministrationShell")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelDescriptor), 201)]
        public IActionResult RegisterSubmodelAtAssetAdministrationShell(string aasId, string submodelId, [FromBody] ISubmodelDescriptor submodelDescriptor)
        {
            if (string.IsNullOrEmpty(aasId))
                return ResultHandling.NullResult(nameof(aasId));
            if (string.IsNullOrEmpty(submodelId))
                return ResultHandling.NullResult(nameof(submodelId));
            if (submodelDescriptor == null)
                return ResultHandling.NullResult(nameof(submodelDescriptor));
            if (submodelDescriptor.Identification == null || string.IsNullOrEmpty(submodelDescriptor.Identification.Id))
                return ResultHandling.NullResult("The identification property of the Submodel Descriptor is null or empty");

            aasId = HttpUtility.UrlDecode(aasId);
            submodelId = HttpUtility.UrlDecode(submodelId);

            if (submodelId != submodelDescriptor.Identification.Id)
                return ResultHandling.BadRequestResult($"Path parameter {aasId} does not equal Submodel Descriptor identification property {submodelDescriptor.Identification.Id}");

            var result = serviceProvider.CreateOrUpdateSubmodelRegistration(aasId, submodelId, submodelDescriptor);
            return result.CreateActionResult(CrudOperation.Create, "api/v1/registry/" + HttpUtility.UrlEncode(aasId) + "/submodels/" + HttpUtility.UrlEncode(submodelId));
        }

        /// <summary>
        /// Retrieves the Submodel registration from a specific Asset Administration Shell registered at the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Submodels Descriptor</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">No Asset Administration Shell / Submodel with passed id found</response>     
        [HttpGet("api/v1/registry/{aasId}/submodels/{submodelId}", Name = "GetSubmodelDescriptorFromAssetAdministrationShell")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelDescriptor), 200)]
        public IActionResult GetSubmodelDescriptorFromAssetAdministrationShell(string aasId, string submodelId)
        {
            if (string.IsNullOrEmpty(aasId))
                return ResultHandling.NullResult(nameof(aasId));
            if (string.IsNullOrEmpty(submodelId))
                return ResultHandling.NullResult(nameof(submodelId));

            aasId = HttpUtility.UrlDecode(aasId);
            submodelId = HttpUtility.UrlDecode(submodelId);

            var result = serviceProvider.RetrieveSubmodelRegistration(aasId, submodelId);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }
        /// <summary>
        /// Unregisters the Submodel from a specific Asset Administration Shell registered at the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <returns></returns>
        /// <response code="200">The Submodel Descriptor was successfully unregistered</response>
        /// <response code="400">Bad Request</response>    
        /// <response code="404">No Asset Administration Shell / Submodel with passed id found</response>  
        [HttpDelete("api/v1/registry/{aasId}/submodels/{submodelId}", Name = "DeleteSubmodelDescriptorFromAssetAdministrationShell")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 204)]
        public IActionResult DeleteSubmodelDescriptorFromAssetAdministrationShell(string aasId, string submodelId)
        {
            if (string.IsNullOrEmpty(aasId))
                return ResultHandling.NullResult(nameof(aasId));
            if (string.IsNullOrEmpty(submodelId))
                return ResultHandling.NullResult(nameof(submodelId));

            aasId = HttpUtility.UrlDecode(aasId);
            submodelId = HttpUtility.UrlDecode(submodelId);

            var result = serviceProvider.DeleteSubmodelRegistration(aasId, submodelId);
            return result.CreateActionResult(CrudOperation.Delete);
        }
        /// <summary>
        /// Retrieves all Submodel registrations from a specific Asset Administration Shell registered at the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <returns></returns>
        /// <response code="200">Returns a list of found Submodels Descriptors</response>
        /// <response code="400">Bad Request</response>  
        /// <response code="404">No Asset Administration Shell with passed id found</response>  
        [HttpGet("api/v1/registry/{aasId}/submodels", Name = "GetAllSubmodelDescriptorsFromAssetAdministrationShell")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<SubmodelDescriptor>), 200)]
        public IActionResult GetAllSubmodelDescriptorsFromAssetAdministrationShell(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return ResultHandling.NullResult(nameof(aasId));

            aasId = HttpUtility.UrlDecode(aasId);
            var result = serviceProvider.RetrieveAllSubmodelRegistrations(aasId);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }
    }
}
