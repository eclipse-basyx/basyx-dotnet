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
using Microsoft.AspNetCore.Hosting;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using BaSyx.API.Components;
using BaSyx.Models.Connectivity;
using System.Linq;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Communication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Asset Administration Shell Controller
    /// </summary>
    [ApiController]
    public class AssetAdministrationShellController : Controller
    {
        private readonly IAssetAdministrationShellServiceProvider serviceProvider;

#if NETCOREAPP3_1
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Asset Administration Shell Controller
        /// </summary>
        /// <param name="assetAdministrationShellServiceProvider">The Asset Administration Shell Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public AssetAdministrationShellController(IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider, IWebHostEnvironment environment)
        {
            shellServiceProvider = aasServiceProvider;
            hostingEnvironment = environment;
        }
#else
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// The constructor for the Asset Administration Shell Controller
        /// </summary>
        /// <param name="assetAdministrationShellServiceProvider">The Asset Administration Shell Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public AssetAdministrationShellController(IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider, IHostingEnvironment environment)
        {
            serviceProvider = assetAdministrationShellServiceProvider;
            hostingEnvironment = environment;
        }
#endif

        #region AssetAdminstrationShell Core Services
        /// <summary>
        /// Retrieves the Asset Administration Shell Descriptor
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Success</response>   
        [HttpGet("aas", Name = "GetAssetAdministrationShell")]
        [ProducesResponseType(typeof(AssetAdministrationShellDescriptor), 200)]
        [Produces("application/json")]
        public IActionResult GetAssetAdministrationShell()
        {
            var serviceDescriptor = serviceProvider?.ServiceDescriptor;

            if (serviceDescriptor == null)
                return StatusCode(502);
            else
                return new OkObjectResult(serviceProvider.ServiceDescriptor);
        }

        /// <summary>
        /// Retrieves all Submodels from the  Asset Administration Shell
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns a list of found Submodels</response>
        /// <response code="404">No Submodel Service Providers found</response>       
        [HttpGet("aas/submodels", Name = "GetSubmodelsFromShell")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetSubmodelsFromShell()
        {
            var submodelProviders = serviceProvider.SubmodelRegistry.GetSubmodelServiceProviders();

            if (!submodelProviders.Success || submodelProviders.Entity?.Count() == 0)
                return NotFound(new Result(false, new NotFoundMessage("Submodels")));

            var submodelBindings = submodelProviders.Entity.Select(s => s.GetBinding()).ToArray();

            return new OkObjectResult(submodelBindings);
        }

        /// <summary>
        /// Creates or updates a Submodel to an existing Asset Administration Shell
        /// </summary>
        /// <param name="submodelIdShort">The Submodel's short id</param>
        /// <param name="submodel">The serialized Submodel object</param>
        /// <returns></returns>
        /// <response code="201">Submodel created successfully</response>
        /// <response code="400">Bad Request</response>               
        [HttpPut("aas/submodels/{submodelIdShort}", Name = "PutSubmodelToShell")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Submodel), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public IActionResult PutSubmodelToShell(string submodelIdShort, [FromBody] ISubmodel submodel)
        {
            if (string.IsNullOrEmpty(submodelIdShort))
                return ResultHandling.NullResult(nameof(submodelIdShort));

            if (submodel == null)
                return ResultHandling.NullResult(nameof(submodel));

            if (submodelIdShort != submodel.IdShort)
            {
                Result badRequestResult = new Result(false,
                    new Message(MessageType.Error, $"Passed path parameter {submodelIdShort} does not equal the Submodel's IdShort {submodel.IdShort}", "400"));

                return badRequestResult.CreateActionResult(CrudOperation.Create, "aas/submodels/" + submodelIdShort);
            }

            var spEndpoints = serviceProvider
                .ServiceDescriptor
                .Endpoints
                .ToList()
                .ConvertAll(c => new HttpEndpoint(DefaultEndpointRegistration.GetSubmodelEndpoint(c, submodel.IdShort)));

            ISubmodelDescriptor descriptor = new SubmodelDescriptor(submodel, spEndpoints);
            SubmodelServiceProvider cssp = new SubmodelServiceProvider(submodel, descriptor);
            cssp.UseInMemorySubmodelElementHandler();
            var result = serviceProvider.SubmodelRegistry.RegisterSubmodelServiceProvider(submodelIdShort, cssp);

            return result.CreateActionResult(CrudOperation.Create, "aas/submodels/" + submodelIdShort);
        }
        /// <summary>
        /// Retrieves the Submodel from the Asset Administration Shell
        /// </summary>
        /// <param name="submodelIdShort">The Submodel's short id</param>
        /// <returns></returns>
        /// <response code="200">Submodel retrieved successfully</response>
        /// <response code="404">No Submodel Service Provider found</response>    
        [HttpGet("aas/submodels/{submodelIdShort}")]
        [HttpGet("aas/submodels/{submodelIdShort}/submodel", Name = "GetSubmodelFromShellByIdShort")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Submodel), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult GetSubmodelFromShellByIdShort(string submodelIdShort)
        {
            if (string.IsNullOrEmpty(submodelIdShort))
                return ResultHandling.NullResult(nameof(submodelIdShort));

            var submodelProvider = serviceProvider.SubmodelRegistry.GetSubmodelServiceProvider(submodelIdShort);
            if (!submodelProvider.Success || submodelProvider?.Entity == null)
                return NotFound(new Result(false, new NotFoundMessage("Submodel")));

            var result = submodelProvider.Entity.RetrieveSubmodel();
            return result.CreateActionResult(CrudOperation.Retrieve);
        }

        /// <summary>
        /// Deletes a specific Submodel from the Asset Administration Shell
        /// </summary>
        /// <param name="submodelIdShort">The Submodel's short id</param>
        /// <returns></returns>
        /// <response code="204">Submodel deleted successfully</response>
        /// <response code="400">Bad Request</response>    
        [HttpDelete("aas/submodels/{submodelIdShort}", Name = "DeleteSubmodelFromShellByIdShort")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        public IActionResult DeleteSubmodelFromShellByIdShort(string submodelIdShort)
        {
            if (string.IsNullOrEmpty(submodelIdShort))
                return ResultHandling.NullResult(nameof(submodelIdShort));

            var result = serviceProvider.SubmodelRegistry.UnregisterSubmodelServiceProvider(submodelIdShort);
            return result.CreateActionResult(CrudOperation.Delete);
        }
        
        #endregion

        #region Submodel Services

        /// <summary>
        /// Retrieves the minimized version of a Submodel, i.e. only the values of SubmodelElements are serialized and returned
        /// </summary>
        /// <param name="submodelIdShort">The Submodel's short id</param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet("aas/submodels/{submodelIdShort}/submodel/values", Name = "Shell_GetSubmodelValues")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetSubmodelValues(string submodelIdShort)
        {
            if (serviceProvider.SubmodelRegistry.IsNullOrNotFound(submodelIdShort, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelValues();
        }

        /// <summary>
        /// Retrieves all Submodel-Elements from the Submodel
        /// </summary>
        /// <param name="submodelIdShort">The Submodel's short id</param>
        /// <returns></returns>
        /// <response code="200">Returns a list of found Submodel-Elements</response>
        /// <response code="404">Submodel not found</response>       
        [HttpGet("aas/submodels/{submodelIdShort}/submodel/submodelElements", Name = "Shell_GetSubmodelElements")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement[]), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetSubmodelElements(string submodelIdShort)
        {
            if (serviceProvider.SubmodelRegistry.IsNullOrNotFound(submodelIdShort, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElements();
        }

        /// <summary>
        /// Creates or updates a Submodel-Element at the Submodel
        /// </summary>
        /// <param name="submodelIdShort">The Submodel's short id</param>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <param name="submodelElement">The Submodel-Element object</param>
        /// <returns></returns>
        /// <response code="201">Submodel-Element created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Submodel not found</response>
        [HttpPut("aas/submodels/{submodelIdShort}/submodel/submodelElements/{seIdShortPath}", Name = "Shell_PutSubmodelElement")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PutSubmodelElement(string submodelIdShort, string seIdShortPath, [FromBody] ISubmodelElement submodelElement)
        {
            if (serviceProvider.SubmodelRegistry.IsNullOrNotFound(submodelIdShort, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PutSubmodelElement(seIdShortPath, submodelElement);
        }
        /// <summary>
        /// Retrieves a specific Submodel-Element from the Submodel
        /// </summary>
        /// <param name="submodelIdShort">The Submodel's short id</param>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Submodel-Element</response>
        /// <response code="404">Submodel / Submodel-Element not found</response>     
        [HttpGet("aas/submodels/{submodelIdShort}/submodel/submodelElements/{seIdShortPath}", Name = "Shell_GetSubmodelElementByIdShort")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SubmodelElement), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetSubmodelElementByIdShort(string submodelIdShort, string seIdShortPath)
        {
            if (serviceProvider.SubmodelRegistry.IsNullOrNotFound(submodelIdShort, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementByIdShort(seIdShortPath);
        }

        /// <summary>
        /// Retrieves the value of a specific Submodel-Element from the Submodel
        /// </summary>
        /// <param name="submodelIdShort">The Submodel's short id</param>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <returns></returns>
        /// <response code="200">Returns the value of a specific Submodel-Element</response>
        /// <response code="404">Submodel / Submodel-Element not found</response>  
        /// <response code="405">Method not allowed</response>  
        [HttpGet("aas/submodels/{submodelIdShort}/submodel/submodelElements/{seIdShortPath}/value", Name = "Shell_GetSubmodelElementValueByIdShort")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        [ProducesResponseType(typeof(Result), 405)]
        public IActionResult Shell_GetSubmodelElementValueByIdShort(string submodelIdShort, string seIdShortPath)
        {
            if (serviceProvider.SubmodelRegistry.IsNullOrNotFound(submodelIdShort, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetSubmodelElementValueByIdShort(seIdShortPath);
        }

        /// <summary>
        /// Updates the Submodel-Element's value
        /// </summary>
        /// <param name="submodelIdShort">The Submodel's short id</param>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <param name="value">The new value</param>
        /// <returns></returns>
        /// <response code="200">Submodel-Element's value changed successfully</response>
        /// <response code="404">Submodel / Submodel-Element not found</response>     
        /// <response code="405">Method not allowed</response>  
        [HttpPut("aas/submodels/{submodelIdShort}/submodel/submodelElements/{seIdShortPath}/value", Name = "Shell_PutSubmodelElementValueByIdShort")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ElementValue), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_PutSubmodelElementValueByIdShort(string submodelIdShort, string seIdShortPath, [FromBody] object value)
        {
            if (serviceProvider.SubmodelRegistry.IsNullOrNotFound(submodelIdShort, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.PutSubmodelElementValueByIdShort(seIdShortPath, value);
        }

        /// <summary>
        /// Deletes a specific Submodel-Element from the Submodel
        /// </summary>
        /// <param name="submodelIdShort">The Submodel's short id</param>
        /// <param name="seIdShortPath">The Submodel-Element's IdShort-Path</param>
        /// <returns></returns>
        /// <response code="204">Submodel-Element deleted successfully</response>
        /// <response code="404">Submodel / Submodel-Element not found</response>
        [HttpDelete("aas/submodels/{submodelIdShort}/submodel/submodelElements/{seIdShortPath}", Name = "Shell_DeleteSubmodelElementByIdShort")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), 200)]
        public IActionResult Shell_DeleteSubmodelElementByIdShort(string submodelIdShort, string seIdShortPath)
        {
            if (serviceProvider.SubmodelRegistry.IsNullOrNotFound(submodelIdShort, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.DeleteSubmodelElementByIdShort(seIdShortPath);
        }

        /// <summary>
        /// Uploads the actual file to the File-SubmodelElement
        /// </summary>
        /// <param name="submodelIdShort">Submodel's short id</param>
        /// <param name="idShortPathToFile">The IdShort path to the File</param>
        /// <param name="file">The actual File to upload</param>
        /// <returns></returns>
        /// <response code="200">File uploaded successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Submodel / Method handler not found</response>
        [HttpPost("aas/submodels/{submodelIdShort}/submodel/submodelElements/{idShortPathToFile}/upload", Name = "Shell_UploadFileContentByIdShort")]
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> Shell_UploadFileContentByIdShort(string submodelIdShort, string idShortPathToFile, IFormFile file)
        {
            if (serviceProvider.SubmodelRegistry.IsNullOrNotFound(submodelIdShort, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return await service.UploadFileContentByIdShort(idShortPathToFile, file);
        }

        /// <summary>
        /// Invokes a specific operation from the Submodel synchronously or asynchronously
        /// </summary>
        /// <param name="submodelIdShort">Submodel's short id</param>
        /// <param name="idShortPathToOperation">The IdShort path to the Operation</param>
        /// <param name="invocationRequest">The parameterized request object for the invocation</param>
        /// <param name="async">Determines whether the execution of the operation is asynchronous (true) or not (false)</param>
        /// <returns></returns>
        /// <response code="200">Operation invoked successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Submodel / Method handler not found</response>
        [HttpPost("aas/submodels/{submodelIdShort}/submodel/submodelElements/{idShortPathToOperation}/invoke", Name = "Shell_InvokeOperationByIdShort")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_InvokeOperationByIdShort(string submodelIdShort, string idShortPathToOperation, [FromBody] InvocationRequest invocationRequest, [FromQuery] bool async)
        {
            if (serviceProvider.SubmodelRegistry.IsNullOrNotFound(submodelIdShort, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.InvokeOperationByIdShort(idShortPathToOperation, invocationRequest, async);
        }

        /// <summary>
        /// Retrieves the result of an asynchronously started operation
        /// </summary>
        /// <param name="submodelIdShort">Submodel's short id</param>
        /// <param name="idShortPathToOperation">The IdShort path to the Operation</param>
        /// <param name="requestId">The request id</param>
        /// <returns></returns>
        /// <response code="200">Result found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Submodel / Operation / Request not found</response>
        [HttpGet("aas/submodels/{submodelIdShort}/submodel/submodelElements/{idShortPathToOperation}/invocationList/{requestId}", Name = "Shell_GetInvocationResultByIdShort")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvocationResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public IActionResult Shell_GetInvocationResultByIdShort(string submodelIdShort, string idShortPathToOperation, string requestId)
        {
            if (serviceProvider.SubmodelRegistry.IsNullOrNotFound(submodelIdShort, out IActionResult result, out ISubmodelServiceProvider provider))
                return result;

            var service = new SubmodelController(provider, hostingEnvironment);
            return service.GetInvocationResultByIdShort(idShortPathToOperation, requestId);
        }
        #endregion        
    }
}
