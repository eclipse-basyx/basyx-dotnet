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
using BaSyx.API.ServiceProvider;
using Microsoft.AspNetCore.Hosting;

namespace BaSyx.API.Http.Controllers.PackageService
{
    /// <summary>
    /// The AASX Package Controller
    /// </summary>
    public class PackageController : Controller
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// Constructor for the AASX Package Controller
        /// </summary>
        /// <param name="serviceProvider">The Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public PackageController(IServiceProvider serviceProvider, IWebHostEnvironment environment)
        {
            this.serviceProvider = serviceProvider;
            hostingEnvironment = environment;
        }


        /// <summary>
        /// Retrieves the full AASX package for a single Asset Administration Shell
        /// </summary>
        /// <returns>AASX Package as download</returns>
        /// <response code="200">Success</response>     
        [HttpGet("aasx", Name = "GetAASXPackage")]
        [ProducesResponseType(200)]
        public IActionResult GetAASXPackage()
        {
            //TODO
            return BadRequest();
            //IAssetAdministrationShell aas = (serviceProvider as IAssetAdministrationShellServiceProvider).GetBinding();
            //string aasxFileName = aas.IdShort + ".aasx";
            //string aasxFilePath = Path.Combine(hostingEnvironment.ContentRootPath, aasxFileName);
            //IFileProvider fileProvider = hostingEnvironment.ContentRootFileProvider;

            //using (AASX_V2_0 aasx = new AASX_V2_0(aasxFilePath))
            //{
            //    AssetAdministrationShellEnvironment_V2_0 env = new AssetAdministrationShellEnvironment_V2_0(aas);
            //    aasx.AddEnvironment(aas.Id, env, ExportType.Xml);

            //    AddFilesToAASX(fileProvider, "aasx", aasx);
            //    AddThumbnailToAASX(fileProvider, aasx);

            //}
            //var fileInfo = fileProvider.GetFileInfo(aasxFileName);
            //var fileResult = new PhysicalFileResult(fileInfo.PhysicalPath, "application/asset-administration-shell-package")
            //{
            //    FileDownloadName = aasxFileName
            //};
            //return fileResult;
        }

        ///// <summary>
        ///// Returns the thumbnail of the AASX package
        ///// </summary>
        ///// <returns>AASX Package as download</returns>
        ///// <response code="200">Success</response>     
        //[HttpGet("aasx/thumbnail", Name = "GetThumbnail")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(404)]
        //public IActionResult GetThumbnail()
        //{
        //    IFileProvider fileProvider = hostingEnvironment.ContentRootFileProvider;
        //    var files = fileProvider.GetDirectoryContents("");
        //    if (files?.Count() > 0)
        //    {
        //        foreach (var file in files)
        //        {
        //            if (file.IsDirectory)
        //                continue;

        //            string fileName = file.Name.ToLower();
        //            if (fileName.Contains(".jpg") ||
        //                fileName.Contains(".jpeg") ||
        //                fileName.Contains(".png") ||
        //                fileName.Contains(".bmp") ||
        //                fileName.Contains(".gif"))
        //            {
        //                if (MimeTypes.TryGetContentType(file.PhysicalPath, out string contentType))
        //                    return File(file.CreateReadStream(), contentType);
        //            }
        //        }
        //    }
        //    return NotFound();
        //}

        //TODO
        //private void AddThumbnailToAASX(IFileProvider fileProvider, AASX_V2_0 aasx)
        //{
        //    foreach (var item in fileProvider.GetDirectoryContents(""))
        //    {
        //        if (item.IsDirectory)
        //            continue;

        //        string fileName = item.Name.ToLower();
        //        if (fileName.Contains(".jpg") ||
        //            fileName.Contains(".jpeg") ||
        //            fileName.Contains(".png") ||
        //            fileName.Contains(".bmp") ||
        //            fileName.Contains(".gif"))
        //        {
        //            aasx.AddThumbnail(item.PhysicalPath);
        //        }
        //    }
        //}

        //TODO
        //private void AddFilesToAASX(IFileProvider fileProvider, string path, AASX_V2_0 aasx)
        //{
        //    foreach (var item in fileProvider.GetDirectoryContents(path))
        //    {
        //        if (item.IsDirectory)
        //        {
        //            AddFilesToAASX(fileProvider, path + "/" + item.Name, aasx);
        //        }
        //        else
        //        {
        //            if (item.Exists)
        //                aasx.AddFileToAASX("/" + path + "/" + item.Name, item.PhysicalPath);
        //        }
        //    }
        //}
    }
}