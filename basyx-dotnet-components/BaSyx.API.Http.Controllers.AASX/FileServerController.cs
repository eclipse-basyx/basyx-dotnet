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
using BaSyx.API.ServiceProvider;
using BaSyx.Models.Export;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;
using BaSyx.Models.AdminShell;
using System.Linq;
using BaSyx.Utils.FileSystem;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using BaSyx.Models.Extensions;
using System;
using BaSyx.Utils.ResultHandling;

namespace BaSyx.API.Http.Controllers.PackageService
{
    /// <summary>
    /// The File Server Controller Controller
    /// </summary>
    public class FileServerController : Controller
    {
        private readonly IFileServiceProvider serviceProvider;
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// Constructor for the File Server Controller
        /// </summary>
        /// <param name="serviceProvider">The Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public FileServerController(IFileServiceProvider serviceProvider, IWebHostEnvironment environment)
        {
            this.serviceProvider = serviceProvider;
            hostingEnvironment = environment;
        }

        /// <summary>
        /// Returns a list of available AASX packages at the server
        /// </summary>
        /// <returns>Requested package list</returns>
        /// <response code="200"></response>     
        [HttpGet("packages", Name = "GetAllAASXPackageIds")]
        [ProducesResponseType(200, Type = typeof(List<PackageDescription>))]
        public IActionResult GetAllAASXPackageIds()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Stores the AASX package at the server
        /// </summary>
        /// <returns>Package Description</returns>
        /// <response code="200"></response>     
        [HttpPost("packages", Name = "PostAASXPackage")]
        [ProducesResponseType(201, Type = typeof(PackageDescription))]
        [DisableRequestSizeLimit]
        public IActionResult PostAASXPackage([FromForm] List<string> aasIds, [FromForm] IFormFile file, [FromForm] string fileName)
        {
            PackageDescription packageDescription = new PackageDescription()
            {
                AdminShellIds = aasIds,
                PackageId = Guid.NewGuid().ToString(),
                FileName = fileName
            };

            try
            {
                //using (var stream = file.OpenReadStream())
                //{
                //    var result = serviceProvider.CreatePackage(packageDescription, stream);
                //    if (result.Success)
                //        return new OkObjectResult(result.Entity);
                //    else
                //        return new BadRequestObjectResult(result);
                //}
                return Ok();
            }
            catch (Exception e)
            {
                var result = new Result(e);
                return new BadRequestObjectResult(result);
            }
        }

        /// <summary>
        /// Returns a specific AASX package from the server
        /// </summary>
        /// <returns>Requested AASX package</returns>
        /// <response code="200"></response>     
        [HttpGet("packages/{packageId}", Name = "GetAASXByPackageId")]
        [ProducesResponseType(200, Type = typeof(PackageDescription))]
        public IActionResult GetAASXByPackageId(string packageId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Updates the AASX package at the server
        /// </summary>
        /// <returns></returns>
        /// <response code="204"></response>     
        [HttpPut("packages/{packageId}", Name = "PutAASXByPackageId")]
        [ProducesResponseType(204)]
        [DisableRequestSizeLimit]
        public IActionResult PutAASXByPackageId(string packageId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Deletes a specific AASX package from the server
        /// </summary>
        /// <returns></returns>
        /// <response code="204"></response>     
        [HttpDelete("packages/{packageId}", Name = "DeleteAASXByPackageId")]
        [ProducesResponseType(204)]
        public IActionResult DeleteAASXByPackageId(string packageId)
        {
            throw new System.NotImplementedException();
        }

    }
}