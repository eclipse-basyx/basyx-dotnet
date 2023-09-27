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
using BaSyx.Utils.ResultHandling;
using System.Web;
using BaSyx.Models.Connectivity;
using BaSyx.API.Interfaces;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Asset Administration Shell Registry Controller
    /// </summary>
    [ApiController]
    public class AssetAdministrationShellRegistryController : Controller
    {
        private readonly IAssetAdministrationShellRegistryInterface serviceProvider;

        /// <summary>
        /// The constructor for the Asset Administration Shell Registry Controller
        /// </summary>
        /// <param name="aasRegistry">The backend implementation for the IAssetAdministrationShellRegistry interface. Usually provided by the Depedency Injection mechanism.</param>
        public AssetAdministrationShellRegistryController(IAssetAdministrationShellRegistryInterface aasRegistry)
        {
            serviceProvider = aasRegistry;
        }

        /// <summary>
        /// Returns all Asset Administration Shell Descriptors
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Requested Asset Administration Shell Descriptors</response>        
        [HttpGet(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTORS, Name = "GetAllAssetAdministrationShellDescriptors")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<AssetAdministrationShellDescriptor>), 200)]
        public IActionResult GetAllAssetAdministrationShellDescriptors()
        {
            var result = serviceProvider.RetrieveAllAssetAdministrationShellRegistrations();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a new Asset Administration Shell Descriptor, i.e. registers an AAS
        /// </summary>
        /// <param name="aasDescriptor">Asset Administration Shell Descriptor object</param>
        /// <returns></returns>
        /// <response code="201">Asset Administration Shell Descriptor created successfully</response>
        /// <response code="400">The syntax of the passed Asset Administration Shell is not valid or malformed request</response>      
        [HttpPost(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTORS, Name = "PostAssetAdministrationShellDescriptor")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(AssetAdministrationShellDescriptor), 201)]
        public IActionResult PostAssetAdministrationShellDescriptor([FromBody] IAssetAdministrationShellDescriptor aasDescriptor)
        {
            if (aasDescriptor.Identification == null || string.IsNullOrEmpty(aasDescriptor.Identification.Id))
                return ResultHandling.NullResult("The identification property of the Asset Administration Shell Descriptor is null or empty");

            string aasId = ResultHandling.Base64UrlEncode(aasDescriptor.Identification.Id);

            var result = serviceProvider.CreateAssetAdministrationShellRegistration(aasDescriptor);
            return result.CreateActionResult(CrudOperation.Create, AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID.Replace("{aasIdentifier}", aasId));
        }

        /// <summary>
        /// Returns a specific Asset Administration Shell Descriptor
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested Asset Administration Shell Descriptor</response>
        /// <response code="404">No Asset Administration Shell with passed id found</response>     
        [HttpGet(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID, Name = "GetAssetAdministrationShellDescriptorById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AssetAdministrationShellDescriptor), 200)]
        public IActionResult GetAssetAdministrationShellDescriptorById(string aasIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);
            var result = serviceProvider.RetrieveAssetAdministrationShellRegistration(aasIdentifier);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates an existing Asset Administration Shell Descriptor
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="aasDescriptor">Asset Administration Shell Descriptor object</param>
        /// <returns></returns>
        /// <response code="204">Asset Administration Shell Descriptor updated successfully</response>
        /// <response code="400">The syntax of the passed Asset Administration Shell is not valid or malformed request</response>    
        /// <response code="404">No Asset Administration Shell with passed id found</response>      
        [HttpPut(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID, Name = "PutAssetAdministrationShellDescriptorById")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult PutAssetAdministrationShellDescriptorById(string aasIdentifier, [FromBody] IAssetAdministrationShellDescriptor aasDescriptor)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));
            if(aasDescriptor.Identification == null || string.IsNullOrEmpty(aasDescriptor.Identification.Id))
                return ResultHandling.NullResult("The identification property of the Asset Administration Shell Descriptor is null or empty");

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);

            if (aasIdentifier != aasDescriptor.Identification.Id)
                return ResultHandling.BadRequestResult($"Path parameter {aasIdentifier} does not equal Asset Administration Shell Descriptor identification property {aasDescriptor.Identification.Id}");
            
            var result = serviceProvider.UpdateAssetAdministrationShellRegistration(aasIdentifier, aasDescriptor);
            return result.CreateActionResult(CrudOperation.Create, "api/v1/registry/" + HttpUtility.UrlEncode(aasIdentifier));
        }

        /// <summary>
        /// Deletes an Asset Administration Shell Descriptor, i.e. de-registers an AAS
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="204">Asset Administration Shell Descriptor deleted successfully</response>
        /// <response code="404">No Asset Administration Shell with passed id found</response>     
        [HttpDelete(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID, Name = "DeleteAssetAdministrationShellDescriptorById")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult DeleteAssetAdministrationShellDescriptorById(string aasIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);
            var result = serviceProvider.DeleteAssetAdministrationShellRegistration(aasIdentifier);
            return result.CreateActionResult(CrudOperation.Delete);
        }

        #region Submodel Descriptors

        /// <inheritdoc cref="SubmodelRegistryController.GetAllSubmodelDescriptors"/>
        [HttpGet(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTORS, Name = "ShellRegistry_GetAllSubmodelDescriptors")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<SubmodelDescriptor>), 200)]
        public IActionResult ShellRegistry_GetAllSubmodelDescriptors(string aasIdentifier)
        {
            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);
            var result = serviceProvider.RetrieveAllSubmodelRegistrations(aasIdentifier);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <inheritdoc cref="SubmodelRegistryController.PostSubmodelDescriptor(ISubmodelDescriptor)"/>
        [HttpPost(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTORS, Name = "ShellRegistry_PostSubmodelDescriptor")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelDescriptor), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public IActionResult ShellRegistry_PostSubmodelDescriptor(string aasIdentifier, [FromBody] ISubmodelDescriptor submodelDescriptor)
        {
            if (submodelDescriptor == null)
                return ResultHandling.NullResult(nameof(submodelDescriptor));
            if (submodelDescriptor.Identification == null || string.IsNullOrEmpty(submodelDescriptor.Identification.Id))
                return ResultHandling.NullResult("The identification property of the Submodel Descriptor is null or empty");

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);
            string submodelId = ResultHandling.Base64UrlEncode(submodelDescriptor.Identification.Id);

            var result = serviceProvider.CreateSubmodelRegistration(aasIdentifier, submodelDescriptor);
            return result.CreateActionResult(CrudOperation.Create, SubmodelRegistryRoutes.SUBMODEL_DESCRIPTOR_ID.Replace("{submodelIdentifier}", submodelId));
        }

        /// <inheritdoc cref="SubmodelRegistryController.PutSubmodelDescriptorById(string, ISubmodelDescriptor)"/>
        [HttpPut(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTOR_ID, Name = "ShellRegistry_PutSubmodelDescriptorById")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRegistry_PutSubmodelDescriptorById(string aasIdentifier, string submodelIdentifier, [FromBody] ISubmodelDescriptor submodelDescriptor)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));
            if (submodelDescriptor == null)
                return ResultHandling.NullResult(nameof(submodelDescriptor));
            if (submodelDescriptor.Identification == null || string.IsNullOrEmpty(submodelDescriptor.Identification.Id))
                return ResultHandling.NullResult("The identification property of the Submodel Descriptor is null or empty");

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);
            submodelIdentifier = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.UpdateSubmodelRegistration(aasIdentifier, submodelIdentifier, submodelDescriptor);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <inheritdoc cref="SubmodelRegistryController.GetSubmodelDescriptorById(string)"/>  
        [HttpGet(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTOR_ID, Name = "ShellRegistry_GetSubmodelDescriptorById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelDescriptor), 200)]
        public IActionResult ShellRegistry_GetSubmodelDescriptorById(string aasIdentifier, string submodelIdentifier)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);
            submodelIdentifier = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.RetrieveSubmodelRegistration(aasIdentifier, submodelIdentifier);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <inheritdoc cref="SubmodelRegistryController.DeleteSubmodelDescriptorById(string)"/>
        [HttpDelete(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTOR_ID, Name = "ShellRegistry_DeleteSubmodelDescriptorById")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRegistry_DeleteSubmodelDescriptorById(string aasIdentifier, string submodelIdentifier)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);
            submodelIdentifier = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.DeleteSubmodelRegistration(aasIdentifier, submodelIdentifier);
            return result.CreateActionResult(CrudOperation.Delete);
        }

        #endregion
    }
}
