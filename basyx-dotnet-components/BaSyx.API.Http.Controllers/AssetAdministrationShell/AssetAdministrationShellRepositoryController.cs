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
using BaSyx.Utils.ResultHandling.ResultTypes;
using System.Text.Json;
using BaSyx.Models.Connectivity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using BaSyx.Models.Extensions;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Asset Administration Shell Repository Controller
    /// </summary>
    [ApiController]
    public class AssetAdministrationShellRepositoryController : Controller
    {
        private readonly IAssetAdministrationShellRepositoryServiceProvider serviceProvider;
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Asset Administration Shell Repository Controller
        /// </summary>
        /// <param name="assetAdministrationShellRepositoryServiceProvider"></param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public AssetAdministrationShellRepositoryController(IAssetAdministrationShellRepositoryServiceProvider assetAdministrationShellRepositoryServiceProvider, IWebHostEnvironment environment)
        {
            serviceProvider = assetAdministrationShellRepositoryServiceProvider;
            hostingEnvironment = environment;
        }

        /// <summary>
        /// Returns all Asset Administration Shells
        /// </summary>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested Asset Administration Shells</response>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS, Name = "GetAllAssetAdministrationShells")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult<List<AssetAdministrationShell>>), 200)]
        public IActionResult GetAllAssetAdministrationShells([FromQuery] int limit = 100, [FromQuery] string cursor = "")
        {
            var aasId = ResultHandling.Base64UrlDecode(cursor);

            var result = serviceProvider.RetrieveAssetAdministrationShells(limit, aasId);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a new Asset Administration Shell
        /// </summary>
        /// <param name="aas">Asset Administration Shell object</param>
        /// <returns></returns>
        /// <response code="201">Asset Administration Shell created successfully</response>
        /// <response code="400">Bad Request</response>             
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS, Name = "PostAssetAdministrationShell")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(AssetAdministrationShell), 201)]
        public IActionResult PostAssetAdministrationShell([FromBody] IAssetAdministrationShell aas)
        {
            if (aas == null)
                return ResultHandling.NullResult(nameof(aas));

            string aasIdentifier = ResultHandling.Base64UrlEncode(aas.Id);

            var result = serviceProvider.CreateAssetAdministrationShell(aas);
            return result.CreateActionResult(CrudOperation.Create, AssetAdministrationShellRepositoryRoutes.SHELLS_AAS.Replace("{aasIdentifier}", aasIdentifier));
        }

        /// <summary>
        /// Returns References to all Asset Administration Shells
        /// </summary>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested Asset Administration Shells</response>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS + OutputModifier.REFERENCE, Name = "GetAllAssetAdministrationShells-Reference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult<List<AssetAdministrationShell>>), 200)]
        public IActionResult GetAllAssetAdministrationShellsReference([FromQuery] int limit = 100, [FromQuery] string cursor = "")
        {
            var aasId = ResultHandling.Base64UrlDecode(cursor);

            var result = serviceProvider.RetrieveAssetAdministrationShellsReference(limit, aasId);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Returns a specific Asset Administration Shell
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Asset Administration Shell</response>
        /// <response code="404">No Asset Administration Shell found</response>           
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS, Name = "GetAssetAdministrationShellById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AssetAdministrationShell), 200)]
        public IActionResult GetAssetAdministrationShellById(string aasIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);

            var result = serviceProvider.RetrieveAssetAdministrationShell(aasIdentifier);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Returns a specific Asset Administration Shell Descriptor
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Asset Administration Shell Descriptor</response>
        /// <response code="404">No Asset Administration Shell Descriptor found</response>           
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + DescriptionRoutes.DESCRIPTOR, Name = "GetAssetAdministrationShellDescriptorFromRepoById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AssetAdministrationShellDescriptor), 200)]
        public IActionResult GetAssetAdministrationShellDescriptorFromRepoById(string aasIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);

            var serviceDescriptor =
                serviceProvider.ServiceDescriptor.AssetAdministrationShellDescriptors.FirstOrDefault(s => s.Id == aasIdentifier);

            if (serviceDescriptor != null)
                return new OkObjectResult(serviceDescriptor);
            else
                return NotFound();
        }

        /// <summary>
        /// Updates an existing Asset Administration Shell
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="aas">Asset Administration Shell object</param>
        /// <returns></returns>
        /// <response code="204">Asset Administration Shell updated successfully</response>          
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS, Name = "PutAssetAdministrationShellById")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        public IActionResult PutAssetAdministrationShellById(string aasIdentifier, [FromBody] IAssetAdministrationShell aas)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));
            if(aas == null)
                return ResultHandling.NullResult(nameof(aas));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);

            var result = serviceProvider.UpdateAssetAdministrationShell(aasIdentifier, aas);
            return result.CreateActionResult(CrudOperation.Update);
        }


        /// <summary>
        /// Deletes an Asset Administration Shell
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Asset Administration Shell deleted successfully</response>
        [HttpDelete(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS, Name = "DeleteAssetAdministrationShellById")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        public IActionResult DeleteAssetAdministrationShellById(string aasIdentifier)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);

            var result = serviceProvider.DeleteAssetAdministrationShell(aasIdentifier);
            return result.CreateActionResult(CrudOperation.Delete);
        }

        #region Asset Adminstration Shell Interface

        /// <summary>
        /// Returns a specific Asset Administration Shell as a Reference
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested Asset Administration Shel</response>
        /// <inheritdoc cref="AssetAdministrationShellController.GetAssetAdministrationShellReference"/>
        [HttpDelete(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + OutputModifier.REFERENCE, Name = "ShellRepo_GetAssetAdministrationShellsReference")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        [Produces("application/json")]
        public IActionResult ShellRepo_GetAssetAdministrationShellsReference(string aasIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.GetAssetAdministrationShellReference();
        }

        /// <summary>
        /// Returns the Asset Information
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested Asset Information</response>
        /// <inheritdoc cref="AssetAdministrationShellController.GetAssetInformation"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION, Name = "ShellRepo_GetAssetInformation")]
        [ProducesResponseType(200)]
        [Produces("application/json")]
        public IActionResult ShellRepo_GetAssetInformation(string aasIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.GetAssetInformation();
        }

        /// <summary>
        /// Updates the Asset Information
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="assetInformation">Asset Information object</param>
        /// <returns></returns>
        /// <response code="204">Asset Information updated successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.PutAssetInformation(IAssetInformation)"/>
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION, Name = "ShellRepo_PutAssetInformation")]
        [ProducesResponseType(204)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult ShellRepo_PutAssetInformation(string aasIdentifier, [FromBody] IAssetInformation assetInformation)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.PutAssetInformation(assetInformation);
        }

        /// <summary>
        /// The thumbnail of the Asset Information.
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">The thumbnail of the Asset Information</response>
        /// <inheritdoc cref="AssetAdministrationShellController.GetThumbnail()"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION_THUMBNAIL,
            Name = "ShellRepo_GetThumbnail")]
        [ProducesResponseType(typeof(AssetInformation), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_GetThumbnail(string aasIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.GetThumbnail();
        }

        /// <summary>
        /// Updates the thumbnail of the Asset Information.
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="file">Thumbnail to upload</param>
        /// <returns></returns>
        /// <response code="204">Thumbnail updated successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.PutThumbnail(IFormFile)"/>
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION_THUMBNAIL,
            Name = "ShellRepo_PutThumbnail")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public async Task<IActionResult> ShellRepo_PutThumbnail(string aasIdentifier, IFormFile file)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return await service.PutThumbnail(file);
        }

        /// <summary>
        /// Delete the thumbnail of the Asset Information.
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Thumbnail deletion successful</response>
        [HttpDelete(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION_THUMBNAIL, Name = "DeleteThumbnail")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public async Task<IActionResult> ShellRepo_DeleteThumbnail(string aasIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return await service.DeleteThumbnail();
        }

        /// <summary>
        /// Returns all submodel references
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel references</response> 
        /// <inheritdoc cref="AssetAdministrationShellController.GetAllSubmodelReferences"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODEL_REFS, Name = "ShellRepo_GetAllSubmodelReferences")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult<Reference[]>), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetAllSubmodelReferences(string aasIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "")
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.GetAllSubmodelReferences(limit, cursor);
        }

        /// <summary>
        /// Creates a submodel reference at the Asset Administration Shell
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelReference">Reference to the Submodel</param>
        /// <returns></returns>
        /// <response code="201">Submodel reference created successfully</response>
        /// <response code="400">Bad Request</response>       
        /// <inheritdoc cref="AssetAdministrationShellController.PostSubmodelReference(IReference)"/>          
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODEL_REFS, Name = "ShellRepo_PostSubmodelReference")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Reference), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public IActionResult ShellRepo_PostSubmodelReference(string aasIdentifier, [FromBody] IReference submodelReference)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.PostSubmodelReference(submodelReference);
        }

        /// <summary>
        /// Deletes the submodel reference from the Asset Administration Shell. Does not delete the submodel itself!
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="204">Submodel deleted successfully</response>
        /// <response code="400">Bad Request</response>    
        /// <inheritdoc cref="AssetAdministrationShellController.DeleteSubmodelReferenceById(string)"/>          
        [HttpDelete(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODEL_REFS_BYID, Name = "ShellRepo_DeleteSubmodelReferenceById")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        public IActionResult ShellRepo_DeleteSubmodelReferenceById(string aasIdentifier, string submodelIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.DeleteSubmodelReferenceById(submodelIdentifier);
        }

        #endregion

        #region Submodel Interface

        /// <summary>
        /// Returns the Submodel
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel references</response> 
        /// <response code="200">Submodel in Path notation</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodel(string, RequestLevel, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, Name = "ShellRepo_GetSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetSubmodel(string aasIdentifier, string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodel(submodelIdentifier, level, extent);
        }

        /// <summary>
        /// Replaces the Submodel
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodel">Submodel object</param>
        /// <returns></returns>
        /// <response code="204">Submodel updated successfully</response>     
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PutSubmodel(string, ISubmodel)"/>
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, Name = "ShellRepo_PutSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_PutSubmodel(string aasIdentifier, string submodelIdentifier, [FromBody] ISubmodel submodel)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PutSubmodel(submodelIdentifier, submodel);
        }

        /// <summary>
        /// Updates the Submodel
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodel">Submodel object</param>
        /// <returns></returns>
        /// <response code="204">Submodel updated successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PatchSubmodel(string, ISubmodel)"/>
        [HttpPatch(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, Name = "ShellRepo_PatchSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_PatchSubmodel(string aasIdentifier, string submodelIdentifier, [FromBody] ISubmodel submodel)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PatchSubmodel(submodelIdentifier, submodel);
        }

        /// <summary>
        /// Deletes the submodel from the Asset Administration Shell and the Repository.
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="204">Submodel deleted successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_DeleteSubmodel(string)"/>
        [HttpDelete(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, Name = "ShellRepo_DeleteSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_DeleteSubmodel(string aasIdentifier, string submodelIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_DeleteSubmodel(submodelIdentifier);
        }

        /// <summary>
        /// Returns the metadata attributes of a specific Submodel
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <response code="404">Submodel not found</response>  
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodelMetadata(string)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.METADATA, Name = "ShellRepo_GetSubmodelMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetSubmodelMetadata(string aasIdentifier, string submodelIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodelMetadata(submodelIdentifier);
        }

        /// <summary>
        /// Updates the metadata attributes of an existing Submodel
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodel">The metadata attributes of the Submodel object</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PatchSubmodelMetadata(string, ISubmodel)"/> 
        [HttpPatch(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.METADATA, Name = "ShellRepo_PatchSubmodelMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_PatchSubmodelMetadata(string aasIdentifier, string submodelIdentifier, [FromBody] ISubmodel submodel)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PatchSubmodelMetadata(submodelIdentifier, submodel);
        }

        /// <summary>
        /// Returns a specific Submodel in the ValueOnly representation
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">ValueOnly representation of the Submodel</response>   
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodelValue(string, RequestLevel, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.VALUE, Name = "ShellRepo_GetSubmodelValue")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetSubmodelValue(string aasIdentifier, string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodelValue(submodelIdentifier, level, extent);
        }

        /// <summary>
        /// Updates the values of an existing Submodel
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel object in its ValueOnly representation</response>  
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PatchSubmodelValueOnly(string, JsonDocument)"/>
        [HttpPatch(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.VALUE, Name = "ShellRepo_PatchSubmodelValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_PatchSubmodelValueOnly(string aasIdentifier, string submodelIdentifier, [FromBody] JsonDocument requestBody)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PatchSubmodelValueOnly(submodelIdentifier, requestBody);
        }

        /// <summary>
        /// Returns the Reference of a specific Submodel
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">ValueOnly representation of the Submodel</response> 
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodelReference(string)"/>    
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.REFERENCE, Name = "ShellRepo_GetSubmodelReference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_GetSubmodelReference(string aasIdentifier, string submodelIdentifier)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodelReference(submodelIdentifier);
        }

        /// <summary>
        /// Returns a specific Submodel in the Path notation
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">ValueOnly representation of the Submodel</response> 
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodelPath(string, RequestLevel)"/>    
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.PATH, Name = "ShellRepo_GetSubmodelPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_GetSubmodelPath(string aasIdentifier, string submodelIdentifier, [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodelPath(submodelIdentifier, level);
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>
        /// <response code="404">Submodel not found</response>   
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetAllSubmodelElements(string, int, string, RequestLevel, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "ShellRepo_GetAllSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetAllSubmodelElements(string aasIdentifier, string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetAllSubmodelElements(submodelIdentifier, limit, cursor, level, extent);
        }

        /// <summary>
        /// Creates a new submodel element
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="201">Submodel element created successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PostSubmodelElement(string, ISubmodelElement)"/>
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "ShellRepo_PostSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_PostSubmodelElement(string aasIdentifier, string submodelIdentifier, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PostSubmodelElement(submodelIdentifier, submodelElement);
        }

        /// <summary>
        /// Returns the metadata attributes of all submodel elements including their hierarchy
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetAllSubmodelElementsMetadata(string, int, string, RequestLevel)"/>   
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.METADATA, Name = "ShellRepo_GetAllSubmodelElementsMetadata")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(Submodel), 200)]
		[ProducesResponseType(typeof(Result), 400)]
		[ProducesResponseType(typeof(Result), 403)]
		[ProducesResponseType(typeof(Result), 500)]
		public IActionResult ShellRepo_GetAllSubmodelElementsMetadata(string aasIdentifier, string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default)
		{
			if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
				return result;

			var service = new AssetAdministrationShellController(provider, hostingEnvironment);
			return service.Shell_GetAllSubmodelElementsMetadata(submodelIdentifier, limit, cursor, level);
		}

        /// <summary>
        /// Returns all submodel elements including their hierarchy in the ValueOnly representation
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>  
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetAllSubmodelElementsValueOnly(string, int, string, RequestLevel, RequestExtent)"/>   
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.VALUE, Name = "ShellRepo_GetAllSubmodelElementsValueOnly")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(Submodel), 200)]
		[ProducesResponseType(typeof(Result), 400)]
		[ProducesResponseType(typeof(Result), 403)]
		[ProducesResponseType(typeof(Result), 500)]
		public IActionResult ShellRepo_GetAllSubmodelElementsValueOnly(string aasIdentifier, string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
		{
			if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
				return result;

			var service = new AssetAdministrationShellController(provider, hostingEnvironment);
			return service.Shell_GetAllSubmodelElementsValueOnly(submodelIdentifier, limit, cursor, level, extent);
		}

        /// <summary>
        /// Returns the References of all submodel elements
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>  
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetAllSubmodelElementsReference(string, int, string)"/>   
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.REFERENCE, Name = "ShellRepo_GetAllSubmodelElementsReference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_GetAllSubmodelElementsReference(string aasIdentifier, string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "")
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetAllSubmodelElementsReference(submodelIdentifier, limit, cursor);
        }

        /// <summary>
        /// Returns the References of all submodel elements
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>  
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetAllSubmodelElementsPath(string, int, string, RequestLevel)"/>   
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.PATH, Name = "ShellRepo_GetAllSubmodelElementsPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_GetAllSubmodelElementsPath(string aasIdentifier, string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetAllSubmodelElementsPath(submodelIdentifier, limit, cursor, level);
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element</response>  
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodelElementByPath(string, string, RequestLevel, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "ShellRepo_GetSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetSubmodelElementByPath(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodelElementByPath(submodelIdentifier, idShortPath, level, extent);
        }

        /// <summary>
        /// Creates a new submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="201">Submodel element created successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PostSubmodelElementByPath(string, string, ISubmodelElement)"/>
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "ShellRepo_PostSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_PostSubmodelElementByPath(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PostSubmodelElementByPath(submodelIdentifier, idShortPath, submodelElement);
        }

        /// <summary>
        /// Replaces an existing submodel element at a specified path within the submodel element hierarchy
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PutSubmodelElementByPath(string, string, ISubmodelElement)"/>
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "ShellRepo_PutSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_PutSubmodelElementByPath(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement requestBody)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PutSubmodelElementByPath(submodelIdentifier, idShortPath, requestBody);
        }

        /// <summary>
        /// Updates an existing SubmodelElement
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PatchSubmodelElementByPath(string, string, ISubmodelElement)"/>
        [HttpPatch(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH,
            Name = "ShellRepo_PatchSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_PatchSubmodelElementByPath(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement requestBody)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PatchSubmodelElementByPath(submodelIdentifier, idShortPath, requestBody);
        }

        /// <summary>
        /// Deletes a submodel element at a specified path within the submodel elements hierarchy
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        /// <response code="204">Submodel element deleted successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_DeleteSubmodelElementByPath(string, string)"/>
        [HttpDelete(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "ShellRepo_DeleteSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_DeleteSubmodelElementByPath(string aasIdentifier, string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_DeleteSubmodelElementByPath(submodelIdentifier, idShortPath);
        }

        /// <summary>
        /// Returns the matadata attributes of a specific submodel element from the Submodel at a specified
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        /// <response code="404">Submodel Element not found</response>     
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodelElementByPathMetadata(string, string, RequestLevel)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.METADATA, Name = "ShellRepo_GetSubmodelElementByPathMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetSubmodelElementByPathMetadata(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodelElementByPathMetadata(submodelIdentifier, idShortPath, level);
        }

        /// <summary>
        /// Updates the metadata attributes an existing SubmodelElement
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Metadata attributes of the SubmodelElement</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response> 
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PatchSubmodelElementByPathMetadata(string, string, ISubmodelElement)"/> 
        [HttpPatch(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.METADATA,
            Name = "ShellRepo_PatchSubmodelElementByPathMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_PatchSubmodelElementByPathMetadata(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PatchSubmodelElementByPathMetadata(submodelIdentifier, idShortPath, submodelElement);
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path in the ValueOnly representation
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodelElementByPathValueOnly(string, string, RequestLevel, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, Name = "ShellRepo_GetSubmodelElementByPathValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetSubmodelElementByPathValueOnly(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodelElementByPathValueOnly(submodelIdentifier, idShortPath, level, extent);
        }

        /// <summary>
        /// Returns the Reference of a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodelElementByPathReference(string, string)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.REFERENCE,
            Name = "ShellRepo_GetSubmodelElementByPathReference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_GetSubmodelElementByPathReference(string aasIdentifier, string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodelElementByPathReference(submodelIdentifier, idShortPath);
        }

        /// <summary>
        /// Returns the Reference of a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetSubmodelElementByPathPath(string, string, RequestLevel)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.PATH,
            Name = "ShellRepo_GetSubmodelElementByPathPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_GetSubmodelElementByPathPath(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetSubmodelElementByPathPath(submodelIdentifier, idShortPath, level);
        }

        /// <summary>
        /// Downloads file content from a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <returns></returns>
        /// <response code="200">Requested file</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetFileByPath(string, string)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "ShellRepo_GetFileByPath")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_GetFileByPath(string aasIdentifier, string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetFileByPath(submodelIdentifier, idShortPath);
        }

        /// <summary>
        /// Uploads file content to an existing submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <param name="file">Content to upload</param>
        /// <returns></returns>
        /// <response code="200">Content uploaded successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">File not found</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PutFileByPath(string, string, IFormFile)"/>
        [HttpPut(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "ShellRepo_PutFileByPath")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public async Task<IActionResult> ShellRepo_PutFileByPath(string aasIdentifier, string submodelIdentifier, string idShortPath, IFormFile file)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return await service.Shell_PutFileByPath(submodelIdentifier, idShortPath, file);
        }

        /// <summary>
        /// Deletes file content of an existing submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <returns></returns>
        /// <response code="200">Content uploaded successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">File not found</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_DeleteFileByPath(string, string)"/>
        [HttpDelete(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "ShellRepo_DeleteFileByPath")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_DeleteFileByPath(string aasIdentifier, string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_DeleteFileByPath(submodelIdentifier, idShortPath);
        }

        /// <summary>
        /// Updates the value of an existing submodel element value at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        /// <response code="400">Bad Request</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_PatchSubmodelElementValueByPathValueOnly(string, string, JsonDocument)"/>
        [HttpPatch(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, Name = "ShellRepo_PatchSubmodelElementValueByPathValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        public IActionResult ShellRepo_PatchSubmodelElementValueByPathValueOnly(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] JsonDocument requestBody)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_PatchSubmodelElementValueByPathValueOnly(submodelIdentifier, idShortPath, requestBody);
        }

        /// <summary>
        /// Synchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_InvokeOperationSync(string, string, InvocationRequest)"/>
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE, Name = "ShellRepo_InvokeOperationSync")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_InvokeOperationSync(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_InvokeOperationSync(submodelIdentifier, idShortPath, operationRequest);
        }

        /// <summary>
        /// Asynchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_InvokeOperationAsync(string, string, InvocationRequest)"/>
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC, Name = "ShellRepo_InvokeOperationAsync")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_InvokeOperationAsync(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_InvokeOperationAsync(submodelIdentifier, idShortPath, operationRequest);
        }

        /// <summary>
        /// Returns the Operation result of an asynchronous invoked Operation
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation’s asynchronous invocation used to request the current state of the operation’s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetOperationAsyncResult(string, string, string)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS, Name = "ShellRepo_GetOperationAsyncResult")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult ShellRepo_GetOperationAsyncResult(string aasIdentifier, string submodelIdentifier, string idShortPath, string handleId)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetOperationAsyncResult(submodelIdentifier, idShortPath, handleId);
        }

        /// <summary>
        /// Synchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_InvokeOperationSyncValueOnly(string, string, InvocationRequest)"/>
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE + OutputModifier.VALUE, Name = "ShellRepo_InvokeOperationSyncValueOnly")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_InvokeOperationSyncValueOnly(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_InvokeOperationSyncValueOnly(submodelIdentifier, idShortPath, operationRequest);
        }

        /// <summary>
        /// Asynchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_InvokeOperationAsyncValueOnly(string, string, InvocationRequest)"/>
        [HttpPost(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC + OutputModifier.VALUE, Name = "ShellRepo_InvokeOperationAsyncValueOnly")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_InvokeOperationAsyncValueOnly(string aasIdentifier, string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_InvokeOperationAsyncValueOnly(submodelIdentifier, idShortPath, operationRequest);
        }

        /// <summary>
        /// Returns the Operation status of an asynchronous invoked Operation
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell’s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelIdentifier">The Submodel’s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation’s asynchronous invocation used to request the current state of the operation’s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
        /// <inheritdoc cref="AssetAdministrationShellController.Shell_GetOperationAsyncStatus(string, string, string)"/>
        [HttpGet(AssetAdministrationShellRepositoryRoutes.SHELLS_AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_STATUS, Name = "ShellRepo_GetOperationAsyncStatus")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 302)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult ShellRepo_GetOperationAsyncStatus(string aasIdentifier, string submodelIdentifier, string idShortPath, string handleId)
        {
            if (serviceProvider.IsNullOrNotFound(aasIdentifier, out IActionResult result, out IAssetAdministrationShellServiceProvider provider))
                return result;

            var service = new AssetAdministrationShellController(provider, hostingEnvironment);
            return service.Shell_GetOperationAsyncStatus(submodelIdentifier, idShortPath, handleId);
        }

        #endregion     
    }
}
