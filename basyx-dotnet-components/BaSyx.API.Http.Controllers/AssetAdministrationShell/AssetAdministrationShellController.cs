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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using BaSyx.API.ServiceProvider;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BaSyx.Models.Extensions;
using System;
using BaSyx.Utils.ResultHandling.ResultTypes;
using BaSyx.Utils.FileSystem;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Asset Administration Shell Controller
    /// </summary>
    [ApiController]
    public class AssetAdministrationShellController : Controller
    {
        private readonly IAssetAdministrationShellServiceProvider serviceProvider;
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Asset Administration Shell Controller
        /// </summary>
        /// <param name="aasServiceProvider">The Asset Administration Shell Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public AssetAdministrationShellController(IAssetAdministrationShellServiceProvider aasServiceProvider, IWebHostEnvironment environment)
        {
            serviceProvider = aasServiceProvider;
            hostingEnvironment = environment;
        }

        #region Asset Adminstration Shell Interface
        /// <summary>
        /// Returns a specific Asset Administration Shell
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Requested Asset Administration Shell</response>
        [HttpGet(AssetAdministrationShellRoutes.AAS, Name = "GetAssetAdministrationShell")]
        [ProducesResponseType(typeof(AssetAdministrationShell), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        [Produces("application/json")]
        public IActionResult GetAssetAdministrationShell()
        {
            var result = serviceProvider.RetrieveAssetAdministrationShell();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates an existing Asset Administration Shell
        /// </summary>
        /// <param name="aas">Asset Administration Shell object</param>
        /// <returns></returns>
        /// <response code="204">Asset Administration Shell updated successfully</response>
        [HttpPut(AssetAdministrationShellRoutes.AAS, Name = "PutAssetAdministrationShell")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        [Produces("application/json")]
        public IActionResult PutAssetAdministrationShell([FromBody] IAssetAdministrationShell aas)
        {
            if (aas == null)
                return ResultHandling.NullResult(nameof(aas));

            var result = serviceProvider.UpdateAssetAdministrationShell(aas);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <summary>
        /// Returns a specific Asset Administration Shell as a Reference
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Requested Asset Administration Shell</response>
        [HttpGet(AssetAdministrationShellRoutes.AAS + OutputModifier.REFERENCE, Name = "GetAssetAdministrationShellReference")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        [Produces("application/json")]
        public IActionResult GetAssetAdministrationShellReference()
        {
            var result = serviceProvider.RetrieveAssetAdministrationShell();
            if(!result.Success || result.Entity == null)
                return result.CreateActionResult(CrudOperation.Retrieve);

            IReference reference = result.Entity.CreateReference();
            return new OkObjectResult(reference);
        }

        /// <summary>
        /// Returns the Asset Information
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Requested Asset Information</response>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION, Name = "GetAssetInformation")]
        [ProducesResponseType(typeof(AssetInformation), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        [Produces("application/json")]
        public IActionResult GetAssetInformation()
        {
            var result = serviceProvider.RetrieveAssetInformation();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Updates the Asset Information
        /// </summary>
        /// <param name="assetInformation">Asset Information object</param>
        /// <returns></returns>
        /// <response code="204">Asset Information updated successfully</response>
        [HttpPut(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION, Name = "PutAssetInformation")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult PutAssetInformation([FromBody] IAssetInformation assetInformation)
        {
            if (assetInformation == null)
                return ResultHandling.NullResult(nameof(assetInformation));

            var result = serviceProvider.UpdateAssetInformation(assetInformation);
            return result.CreateActionResult(CrudOperation.Update);
        }

        /// <summary>
        /// The thumbnail of the Asset Information.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">The thumbnail of the Asset Information</response>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION_THUMBNAIL, Name = "GetThumbnail")]
        [ProducesResponseType(typeof(AssetInformation), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetThumbnail()
        {
            var retrieveAssetInformation = serviceProvider.RetrieveAssetInformation();
            if (!retrieveAssetInformation.Success || retrieveAssetInformation.Entity == null)
                return retrieveAssetInformation.CreateActionResult(CrudOperation.Retrieve);

            if (retrieveAssetInformation.Entity.DefaultThumbnail == null)
                return NotFound(new { message = "Asset information has no thumbnail" });

            var fileName = retrieveAssetInformation.Entity.DefaultThumbnail.Path.TrimStart('/');

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

            return NotFound(new { message = "Physical file not found", itemId = file.PhysicalPath });
        }

        /// <summary>
        /// Updates the thumbnail of the Asset Information.
        /// </summary>
        /// <param name="file">Thumbnail to upload</param>
        /// <returns></returns>
        /// <response code="204">Thumbnail updated successfully</response>
        [HttpPut(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION_THUMBNAIL, Name = "PutThumbnail")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public async Task<IActionResult> PutThumbnail(IFormFile file)
        {
            if (file == null)
                return ResultHandling.NullResult(nameof(file));

            var retrieveAssetInformation = serviceProvider.RetrieveAssetInformation();
            if (!retrieveAssetInformation.Success || retrieveAssetInformation.Entity == null)
                return retrieveAssetInformation.CreateActionResult(CrudOperation.Retrieve);

            string fileName;
            if (retrieveAssetInformation.Entity.DefaultThumbnail == null)
            {
                // create new Asset Information with Thumbnail
                var assetInformation = new AssetInformation
                {
                    DefaultThumbnail = new Resource { ContentType = file.ContentType, Path  = file.FileName},
                    AssetKind = retrieveAssetInformation.Entity.AssetKind,
                    AssetType = retrieveAssetInformation.Entity.AssetType,
                    GlobalAssetId = retrieveAssetInformation.Entity.GlobalAssetId,
                    SpecificAssetIds = retrieveAssetInformation.Entity.SpecificAssetIds
                };

                var updateAssetInformation = serviceProvider.UpdateAssetInformation(assetInformation);
                if (!updateAssetInformation.Success)
                    return updateAssetInformation.CreateActionResult(CrudOperation.Update);

                fileName = file.FileName;
            }
            else
                fileName = retrieveAssetInformation.Entity.DefaultThumbnail.Path.TrimStart('/');
            
            string filePath = Path.Combine(hostingEnvironment.ContentRootPath, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return Ok();
        }

        /// <summary>
        /// Delete the thumbnail of the Asset Information.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Thumbnail deletion successful</response>
        [HttpDelete(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_ASSET_INFORMATION_THUMBNAIL, Name = "DeleteThumbnail")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public async Task<IActionResult> DeleteThumbnail()
        {
            var retrieveAssetInformation = serviceProvider.RetrieveAssetInformation();
            if (!retrieveAssetInformation.Success || retrieveAssetInformation.Entity == null)
                return retrieveAssetInformation.CreateActionResult(CrudOperation.Retrieve);

            if (retrieveAssetInformation.Entity.DefaultThumbnail == null)
                return NotFound(new { message = "Asset information has no thumbnail" });

            var fileName = retrieveAssetInformation.Entity.DefaultThumbnail.Path.TrimStart('/');

            // create new Asset Information without Thumbnail
            var assetInformation = new AssetInformation
            {
                DefaultThumbnail = null,
                AssetKind = retrieveAssetInformation.Entity.AssetKind,
                AssetType = retrieveAssetInformation.Entity.AssetType,
                GlobalAssetId = retrieveAssetInformation.Entity.GlobalAssetId,
                SpecificAssetIds = retrieveAssetInformation.Entity.SpecificAssetIds
            };

            var updateAssetInformation = serviceProvider.UpdateAssetInformation(assetInformation);
            if (!updateAssetInformation.Success)
                return updateAssetInformation.CreateActionResult(CrudOperation.Update);

            IFileProvider fileProvider = hostingEnvironment.ContentRootFileProvider;
            var file = fileProvider.GetFileInfo(fileName);

            if (file.Exists && !string.IsNullOrEmpty(file.PhysicalPath))
                System.IO.File.Delete(file.PhysicalPath);
            else
                return NotFound(new { message = "Physical file not found", itemId = file.PhysicalPath });

            return Ok();
        }

        /// <summary>
        /// Returns all submodel references
        /// </summary>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel references</response> 
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODEL_REFS, Name = "GetAllSubmodelReferences")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult<Reference[]>), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult GetAllSubmodelReferences([FromQuery] int limit = 100, [FromQuery] string cursor = "")
        {
            string submodelId = ResultHandling.Base64UrlDecode(cursor);

            var result = serviceProvider.RetrieveAllSubmodelReferences(limit, submodelId);
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Creates a submodel reference at the Asset Administration Shell
        /// </summary>
        /// <param name="submodelReference">Reference to the Submodel</param>
        /// <returns></returns>
        /// <response code="201">Submodel reference created successfully</response>
        /// <response code="400">Bad Request</response>               
        [HttpPost(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODEL_REFS, Name = "PostSubmodelReference")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Reference), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public IActionResult PostSubmodelReference([FromBody] IReference submodelReference)
        {
            if (submodelReference == null)
                return ResultHandling.NullResult(nameof(submodelReference));

            var result = serviceProvider.CreateSubmodelReference(submodelReference);
            return result.CreateActionResult(CrudOperation.Create, AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID.Replace("{submodelIdentifier}", submodelReference.First.Value));
        }


        /// <summary>
        /// Deletes the submodel reference from the Asset Administration Shell. Does not delete the submodel itself!
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="204">Submodel deleted successfully</response>
        /// <response code="400">Bad Request</response>    
        [HttpDelete(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODEL_REFS_BYID, Name = "DeleteSubmodelReferenceById")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult DeleteSubmodelReferenceById(string submodelIdentifier)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));

            string submodelId = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.DeleteSubmodelReference(submodelId);
            return result.CreateActionResult(CrudOperation.Delete);
        }

        #endregion

        #region Submodel Interface
        /// <summary>
        /// Returns the Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel references</response> 
        /// <inheritdoc cref="SubmodelController.GetSubmodel(RequestLevel, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, Name = "Shell_GetSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetSubmodel(string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodel(level, extent);
        }

        /// <summary>
        /// Returns the metadata attributes of a specific Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <response code="404">Submodel not found</response>  
        /// <inheritdoc cref="SubmodelController.GetSubmodelMetadata()"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.METADATA, Name = "Shell_GetSubmodelMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetSubmodelMetadata(string submodelIdentifier)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelMetadata();
        }

        /// <summary>
        /// Updates the metadata attributes of the Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodel">The metadata attributes of the Submodel object</param>
        /// <returns></returns>
        /// <response code="200">Requested Submodel</response>
        /// <inheritdoc cref="SubmodelController.PatchSubmodelMetadata(ISubmodel)"/> 
        [HttpPatch(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.METADATA, Name = "Shell_PatchSubmodelMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_PatchSubmodelMetadata(string submodelIdentifier, [FromBody] ISubmodel submodel)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodelMetadata(submodel);
        }

        /// <summary>
        /// Returns the Submodel in the ValueOnly representation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">ValueOnly representation of the Submodel</response>   
        /// <inheritdoc cref="SubmodelController.GetSubmodelValueOnly(RequestLevel, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.VALUE, Name = "Shell_GetSubmodelValue")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetSubmodelValue(string submodelIdentifier, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelValueOnly(level, extent);
        }

        /// <summary>
        /// Updates the values of the Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel object in its ValueOnly representation</response>     
        /// <inheritdoc cref="SubmodelController.PatchSubmodelValueOnly(JsonDocument)"/>
        [HttpPatch(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.VALUE, Name = "Shell_PatchSubmodelValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_PatchSubmodelValueOnly(string submodelIdentifier, [FromBody] JsonDocument requestBody)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodelValueOnly(requestBody);
        }

        /// <summary>
        /// Returns the Reference of the Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">ValueOnly representation of the Submodel</response> 
        /// <inheritdoc cref="SubmodelController.GetSubmodelReference()"/>    
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.REFERENCE, Name = "Shell_GetSubmodelReference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetSubmodelReference(string submodelIdentifier)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelReference();
        }

        /// <summary>
        /// Returns the Submodel in the Path notation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Submodel in Path notation</response>
        /// <inheritdoc cref="SubmodelController.GetSubmodelPath(RequestLevel)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + OutputModifier.PATH, Name = "Shell_GetSubmodelPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetSubmodelPath(string submodelIdentifier, [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelPath(level);
        }

        /// <summary>
        /// Replaces the Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodel">Submodel object</param>
        /// <returns></returns>
        /// <response code="204">Submodel updated successfully</response>     
        /// <inheritdoc cref="SubmodelController.PutSubmodel(ISubmodel)"/>
        [HttpPut(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, Name = "Shell_PutSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PutSubmodel(string submodelIdentifier, [FromBody] ISubmodel submodel)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));

            var submodelId = ResultHandling.Base64UrlDecode(submodelIdentifier);
            
            var result = serviceProvider.PutSubmodel(new Identifier(submodelId), submodel);
            return result.CreateActionResult(CrudOperation.Delete);
        }

        /// <summary>
        /// Updates the Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodel">Submodel object</param>
        /// <returns></returns>
        /// <response code="204">Submodel updated successfully</response>
        /// <inheritdoc cref="SubmodelController.PatchSubmodel(ISubmodel)"/>
        [HttpPatch(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, Name = "Shell_PatchSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_PatchSubmodel(string submodelIdentifier, [FromBody] ISubmodel submodel)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodel(submodel);
        }

        /// <summary>
        /// Deletes the Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="204">Submodel deleted successfully</response>
        /// <inheritdoc cref="SubmodelController.PatchSubmodel(ISubmodel)"/>
        [HttpDelete(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID, Name = "Shell_DeleteSubmodel")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_DeleteSubmodel(string submodelIdentifier)
        {
            if (string.IsNullOrEmpty(submodelIdentifier))
                return ResultHandling.NullResult(nameof(submodelIdentifier));

            string submodelId = ResultHandling.Base64UrlDecode(submodelIdentifier);

            var result = serviceProvider.DeleteSubmodel(new Identifier(submodelId));
            return result.CreateActionResult(CrudOperation.Delete);
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>
        /// <response code="404">Submodel not found</response>   
        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElements(int, string, RequestLevel, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "Shell_GetAllSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetAllSubmodelElements(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = RequestLevel.Deep, [FromQuery] RequestExtent extent = RequestExtent.WithoutBlobValue)
        {           
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElements(limit, cursor, level, extent);
        }

        /// <summary>
        /// Returns the metadata attributes of all submodel elements including their hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>
        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElementsMetadata(int, string, RequestLevel)"/>   
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.METADATA, Name = "Shell_GetAllSubmodelElementsMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetAllSubmodelElementsMetadata(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElementsMetadata(limit, cursor, level);
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy in the ValueOnly representation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>  
        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElementsValueOnly(int, string, RequestLevel, RequestExtent)"/>   
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.VALUE, Name = "Shell_GetAllSubmodelElementsValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetAllSubmodelElementsValueOnly(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "", [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElementsValueOnly(limit, cursor, level, extent);
        }

        /// <summary>
        /// Returns the References of all submodel elements
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements</response>  
        /// <inheritdoc cref="SubmodelController.GetAllSubmodelElementsReference(int, string)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.REFERENCE, Name = "GetAllSubmodelElementsReference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetAllSubmodelElementsReference(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "")
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElementsReference(limit, cursor);
        }

        /// <summary>
        /// Returns all submodel elements including their hierarchy in the Path notation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="limit">The maximum number of elements in the response array</param>
        /// <param name="cursor">A server-generated identifier retrieved from pagingMetadata that specifies from which position the result listing should continue</param>      
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">List of found submodel elements in the Path notation</response>  
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS + OutputModifier.PATH,
            Name = "Shell_GetAllSubmodelElementsPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetAllSubmodelElementsPath(string submodelIdentifier, [FromQuery] int limit = 100, [FromQuery] string cursor = "",
            [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetAllSubmodelElementsPath(limit, cursor);
        }

        /// <summary>
        /// Creates a new submodel element
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="201">Submodel element created successfully</response>
        /// <inheritdoc cref="SubmodelController.PostSubmodelElement(ISubmodelElement, RequestLevel, RequestExtent)"/>
        [HttpPost(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS, Name = "Shell_PostSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PostSubmodelElement(string submodelIdentifier, [FromBody] ISubmodelElement submodelElement)
        {           
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PostSubmodelElement(submodelElement);
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element</response>  
        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPath(string, RequestLevel, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "Shell_GetSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPath(idShortPath, level, extent);
        }

        /// <summary>
        /// Returns the metadata attributes of a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        /// <response code="404">Submodel Element not found</response>     
        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPathMetadata(string, RequestLevel)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.METADATA, Name = "Shell_GetSubmodelElementByPathMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetSubmodelElementByPathMetadata(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPathMetadata(idShortPath, level);
        }

        /// <summary>
        /// Updates the metadata attributes an existing SubmodelElement
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Metadata attributes of the SubmodelElement</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response> 
        /// <inheritdoc cref="SubmodelController.PatchSubmodelElementByPathMetadata(string, ISubmodelElement)"/> 
        [HttpPatch(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.METADATA,
            Name = "Shell_PatchSubmodelElementByPathMetadata")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_PatchSubmodelElementByPathMetadata(string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodelElementByPathMetadata(idShortPath, submodelElement);
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path in the ValueOnly representation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <param name="extent">Determines to which extent the resource is being serialized</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPathValueOnly(string, RequestLevel, RequestExtent)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, Name = "Shell_GetSubmodelElementByPathValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetSubmodelElementByPathValueOnly(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default, [FromQuery] RequestExtent extent = default)
        {      
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPathValueOnly(idShortPath, level, extent);
        }

        /// <summary>
        /// Returns the Reference of a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPathReference(string)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.REFERENCE,
            Name = "Shell_GetSubmodelElementByPathReference")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetSubmodelElementByPathReference(string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPathReference(idShortPath);
        }

        /// <summary>
        /// Returns a specific submodel element from the Submodel at a specified path in the Path notation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="level">Determines the structural depth of the respective resource content</param>
        /// <returns></returns>
        /// <response code="200">Requested submodel element in its ValueOnly representation</response>
        /// <inheritdoc cref="SubmodelController.GetSubmodelElementByPathPath(string, RequestLevel)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.PATH,
            Name = "Shell_GetSubmodelElementByPathPath")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Reference), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetSubmodelElementByPathPath(string submodelIdentifier, string idShortPath, [FromQuery] RequestLevel level = default)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByPathPath(idShortPath, level);
        }

        /// <summary>
        /// Creates a new submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="201">Submodel element created successfully</response>
        /// <inheritdoc cref="SubmodelController.PostSubmodelElementByPath(string, ISubmodelElement)"/>
        [HttpPost(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "Shell_PostSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PostSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PostSubmodelElementByPath(idShortPath, submodelElement);
        }

        /// <summary>
        /// Replaces an existing submodel element at a specified path within the submodel element hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        /// <inheritdoc cref="SubmodelController.PutSubmodelElementByPath(string, ISubmodelElement)"/>
        [HttpPut(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "Shell_PutSubmodelElementByPath")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PutSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement requestBody)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PutSubmodelElementByPath(idShortPath, requestBody);
        }

        /// <summary>
        /// Updates an existing SubmodelElement
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="submodelElement">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        /// <inheritdoc cref="SubmodelController.PatchSubmodelElementByPath(string, ISubmodelElement)"/>
        [HttpPatch(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH,
            Name = "Shell_PatchSubmodelElementByPath")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_PatchSubmodelElementByPath(string submodelIdentifier, string idShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodelElementByPath(idShortPath, submodelElement);
        }

        /// <summary>
        /// Updates the value of an existing submodel element value at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="requestBody">Requested submodel element</param>
        /// <returns></returns>
        /// <response code="204">Submodel element updated successfully</response>
        /// <response code="400">Bad Request</response>
        /// <inheritdoc cref="SubmodelController.PatchSubmodelElementValueByPathValueOnly(string, JsonDocument)"/>
        [HttpPatch(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH + OutputModifier.VALUE, Name = "Shell_PatchSubmodelElementValueByPathValueOnly")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PatchSubmodelElementValueByPathValueOnly(string submodelIdentifier, string idShortPath, [FromBody] JsonDocument requestBody)
        {          
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PatchSubmodelElementValueByPathValueOnly(idShortPath, requestBody);
        }

        /// <summary>
        /// Deletes a submodel element at a specified path within the submodel elements hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        /// <response code="204">Submodel element deleted successfully</response>
        /// <inheritdoc cref="SubmodelController.DeleteSubmodelElementByPath(string)"/>
        [HttpDelete(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH, Name = "Shell_DeleteSubmodelElementByPath")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_DeleteSubmodelElementByPath(string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.DeleteSubmodelElementByPath(idShortPath);
        }

        /// <summary>
        /// Downloads file content from a specific submodel element from the Submodel at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <returns></returns>
        /// <response code="200">Requested file</response>
        /// <inheritdoc cref="SubmodelController.GetFileByPath(string)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "Shell_GetFileByPath")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetFileByPath(string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetFileByPath(idShortPath);
        }

        /// <summary>
        /// Uploads file content to an existing submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <param name="file">Content to upload</param>
        /// <returns></returns>
        /// <response code="200">Content uploaded successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">File not found</response>
        /// <inheritdoc cref="SubmodelController.PutFileByPath(string, IFormFile)"/>
        [HttpPut(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT, Name = "Shell_PutFileByPath")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> Shell_PutFileByPath(string submodelIdentifier, string idShortPath, IFormFile file)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return await service.PutFileByPath(idShortPath, file);
        }

        /// <summary>
        /// Deletes file content of an existing submodel element at a specified path within submodel elements hierarchy
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case a file</param>
        /// <returns></returns>
        /// <response code="200">File deleted successfully</response>
        /// <inheritdoc cref="SubmodelController.DeleteFileByPath(string)"/>
        [HttpDelete(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT,
            Name = "Shell_DeleteFileByPath")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_DeleteFileByPath(string submodelIdentifier, string idShortPath)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.DeleteFileByPath(idShortPath);
        }

        /// <summary>
        /// Synchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <inheritdoc cref="SubmodelController.InvokeOperationSync(string, InvocationRequest)"/>
        [HttpPost(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE, Name = "Shell_InvokeOperationSync")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_InvokeOperationSync(string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {           
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperationSync(idShortPath, operationRequest);
        }

        /// <summary>
        /// Asynchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <inheritdoc cref="SubmodelController.InvokeOperationAsync(string, InvocationRequest)"/>
        [HttpPost(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC, Name = "Shell_InvokeOperationAsync")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_InvokeOperationAsync(string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperationAsync(idShortPath, operationRequest);
        }

        /// <summary>
        /// Synchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <inheritdoc cref="SubmodelController.InvokeOperationSyncValueOnly(string, InvocationRequest)"/>
        [HttpPost(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE + OutputModifier.VALUE, Name = "Shell_InvokeOperationSyncValueOnly")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_InvokeOperationSyncValueOnly(string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperationSyncValueOnly(idShortPath, operationRequest);
        }

        /// <summary>
        /// Asynchronously invokes an Operation at a specified path
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="operationRequest">Operation request object</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <inheritdoc cref="SubmodelController.InvokeOperationAsyncValueOnly(string, InvocationRequest)"/>
        [HttpPost(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC + OutputModifier.VALUE, Name = "Shell_InvokeOperationAsyncValueOnly")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_InvokeOperationAsyncValueOnly(string submodelIdentifier, string idShortPath, [FromBody] InvocationRequest operationRequest)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperationAsyncValueOnly(idShortPath, operationRequest);
        }

        /// <summary>
        /// Returns the Operation status of an asynchronous invoked Operation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation�s asynchronous invocation used to request the current state of the operation�s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
        /// <inheritdoc cref="SubmodelController.GetOperationAsyncStatus(string, string)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_STATUS, Name = "Shell_GetOperationAsyncStatus")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 302)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetOperationAsyncStatus(string submodelIdentifier, string idShortPath, string handleId)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetOperationAsyncStatus(idShortPath, handleId); ;
        }

        /// <summary>
        /// Returns the Operation result of an asynchronous invoked Operation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation�s asynchronous invocation used to request the current state of the operation�s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
        /// <inheritdoc cref="SubmodelController.GetOperationAsyncResult(string, string)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS, Name = "Shell_GetOperationAsyncResult")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetOperationAsyncResult(string submodelIdentifier, string idShortPath, string handleId)
        {           
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetOperationAsyncResult(idShortPath, handleId);
        }

        /// <summary>
        /// Returns the Operation result of an asynchronous invoked Operation
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel�s unique id (BASE64-URL-encoded)</param>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated), in this case an operation</param>
        /// <param name="handleId">The returned handle id of an operation�s asynchronous invocation used to request the current state of the operation�s execution (BASE64-URL-encoded)</param>
        /// <returns></returns>
        /// <response code="200">Operation result object</response>
        /// <inheritdoc cref="SubmodelController.GetOperationAsyncResultValueOnly(string, string)"/>
        [HttpGet(AssetAdministrationShellRoutes.AAS + AssetAdministrationShellRoutes.AAS_SUBMODELS_BYID + SubmodelRoutes.SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS + OutputModifier.VALUE, Name = "Shell_GetOperationAsyncResultValueOnly")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 403)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 500)]
        public IActionResult Shell_GetOperationAsyncResultValueOnly(string submodelIdentifier, string idShortPath, string handleId)
        {
            if (serviceProvider.SubmodelProviderRegistry.IsNullOrNotFound(submodelIdentifier, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetOperationAsyncResultValueOnly(idShortPath, handleId);
        }
        #endregion        
    }
}
