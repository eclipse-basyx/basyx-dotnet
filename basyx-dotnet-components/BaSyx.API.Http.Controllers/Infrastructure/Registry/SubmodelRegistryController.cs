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
using BaSyx.Utils.ResultHandling;
using BaSyx.Models.Connectivity;
using BaSyx.API.Interfaces;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Submodel Registry Controller
    /// </summary>
    [ApiController]
    public class SubmodelRegistryController : Controller
    {
        private readonly ISubmodelRegistryInterface serviceProvider;

        /// <summary>
        /// The constructor for the Submodel Registry Controller
        /// </summary>
        /// <param name="submodelRegistry">The backend implementation for the ISubmodelRegistry interface. Usually provided by the Depedency Injection mechanism.</param>
        public SubmodelRegistryController(ISubmodelRegistryInterface submodelRegistry)
        {
            serviceProvider = submodelRegistry;
        }

        /// <summary>
        /// Returns all Submodel Descriptors
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns a list of found Submodels Descriptors</response>
        [HttpGet(SubmodelRegistryRoutes.SUBMODEL_DESCRIPTORS, Name = "GetAllSubmodelDescriptors")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<SubmodelDescriptor>), 200)]
        public IActionResult GetAllSubmodelDescriptors()
        {
            var result = serviceProvider.RetrieveAllSubmodelRegistrations();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a new Submodel Descriptor, i.e. registers a submodel
        /// </summary>
        /// <param name="submodelDescriptor">Submodel Descriptor object</param>
        /// <returns></returns>
        /// <response code="201">Submodel Descriptor created successfully</response>
        /// <response code="400">The syntax of the passed Submodel is not valid or malformed request</response>      
        [HttpPost(SubmodelRegistryRoutes.SUBMODEL_DESCRIPTORS, Name = "PostSubmodelDescriptor")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelDescriptor), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public IActionResult PostSubmodelDescriptor([FromBody] ISubmodelDescriptor submodelDescriptor)
        {
            if (submodelDescriptor == null)
                return ResultHandling.NullResult(nameof(submodelDescriptor));
            if (submodelDescriptor.Id == null || string.IsNullOrEmpty(submodelDescriptor.Id.Id))
                return ResultHandling.NullResult("The identification property of the Submodel Descriptor is null or empty");

            string submodelId = ResultHandling.Base64UrlEncode(submodelDescriptor.Id.Id);

            var result = serviceProvider.CreateSubmodelRegistration(submodelDescriptor);
            return result.CreateActionResult(CrudOperation.Create, SubmodelRegistryRoutes.SUBMODEL_DESCRIPTOR_ID.Replace("{submodelIdentifier}", submodelId));
        }

        /// <summary>
        /// Updates an existing Submodel Descriptor
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelDescriptor">The Submodel Descriptor</param>
        /// <returns></returns>
        /// <response code="204">Submodel Descriptor updated successfully</response>
        /// <response code="400">The syntax of the passed Submodel descriptor is not valid or malformed request</response>      
        /// <response code="404">No Submodel descriptor with passed id found</response>   
        [HttpPut(SubmodelRegistryRoutes.SUBMODEL_DESCRIPTOR_ID, Name = "PutSubmodelDescriptorById")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult PutSubmodelDescriptorById(string submodelIdentifier, [FromBody] ISubmodelDescriptor submodelDescriptor)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));
            if (submodelDescriptor == null)
                return ResultHandling.NullResult(nameof(submodelDescriptor));
            if (submodelDescriptor.Id == null || string.IsNullOrEmpty(submodelDescriptor.Id.Id))
                return ResultHandling.NullResult("The identification property of the Submodel Descriptor is null or empty");

            submodelIdentifier = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.UpdateSubmodelRegistration(submodelIdentifier, submodelDescriptor);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <summary>
        /// Returns a specific Submodel Descriptor
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel Descriptor</response>
        /// <response code="404">No Submodel descriptor with passed id found</response>     
        [HttpGet(SubmodelRegistryRoutes.SUBMODEL_DESCRIPTOR_ID, Name = "GetSubmodelDescriptorById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelDescriptor), 200)]
        public IActionResult GetSubmodelDescriptorById(string submodelIdentifier)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));

            submodelIdentifier = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.RetrieveSubmodelRegistration(submodelIdentifier);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }
        /// <summary>
        /// Deletes a Submodel Descriptor, i.e. de-registers a submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="204">Submodel Descriptor deleted successfully</response>
        /// <response code="404">No Submodel descriptor with passed id found</response>  
        [HttpDelete(SubmodelRegistryRoutes.SUBMODEL_DESCRIPTOR_ID, Name = "DeleteSubmodelDescriptorById")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult DeleteSubmodelDescriptorById(string submodelIdentifier)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));

            submodelIdentifier = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.DeleteSubmodelRegistration(submodelIdentifier);
            return result.CreateActionResult(CrudOperation.Delete);
        }
    }
}
