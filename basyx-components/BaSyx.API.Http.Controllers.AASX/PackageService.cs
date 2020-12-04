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
using BaSyx.API.Components;
using BaSyx.Models.Export;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using System.Linq;
using BaSyx.Utils.FileHandling;

namespace BaSyx.API.Http.Controllers.PackageService
{
    /// <summary>
    /// The AASX Package Service
    /// </summary>
    public class PackageService : Controller
    {
        private readonly IAssetAdministrationShellServiceProvider shellServiceProvider;

#if NETCOREAPP3_1
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// Constructor for the AASX-Package Services Controller
        /// </summary>
        /// <param name="aasServiceProvider">The Asset Administration Shell Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public PackageService(IAssetAdministrationShellServiceProvider aasServiceProvider, IWebHostEnvironment environment)
        {
            shellServiceProvider = aasServiceProvider;
            hostingEnvironment = environment;
        }
#else
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// Constructor for the AASX-Package Services Controller
        /// </summary>
        /// <param name="aasServiceProvider">The Asset Administration Shell Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public PackageService(IAssetAdministrationShellServiceProvider aasServiceProvider, IHostingEnvironment environment)
        {
            shellServiceProvider = aasServiceProvider;
            hostingEnvironment = environment;
        }
#endif


        /// <summary>
        /// Retrieves the full AASX package for a single Asset Administration Shell
        /// </summary>
        /// <returns>AASX Package as download</returns>
        /// <response code="200">Success</response>     
        [HttpGet("aasx", Name = "GetAASXPackage")]
        [ProducesResponseType(200)]
        public IActionResult GetAASXPackage()
        {
            IAssetAdministrationShell aas = shellServiceProvider.GetBinding();
            string aasxFileName = aas.IdShort + ".aasx";
            string aasxFilePath = Path.Combine(hostingEnvironment.ContentRootPath, aasxFileName);
            IFileProvider fileProvider = hostingEnvironment.ContentRootFileProvider;

            using (AASX aasx = new AASX(aasxFilePath))
            {
                AssetAdministrationShellEnvironment_V2_0 env = new AssetAdministrationShellEnvironment_V2_0(aas);
                aasx.AddEnvironment(aas.Identification, env, ExportType.Xml);

                AddFilesToAASX(fileProvider, "aasx", aasx);
                AddThumbnailToAASX(fileProvider, aasx);

            }
            var fileInfo = fileProvider.GetFileInfo(aasxFileName);
            var fileResult = new PhysicalFileResult(fileInfo.PhysicalPath, "application/asset-administration-shell-package")
            {
                FileDownloadName = aasxFileName
            };
            return fileResult;
        }

        private void AddThumbnailToAASX(IFileProvider fileProvider, AASX aasx)
        {
            foreach (var item in fileProvider.GetDirectoryContents(""))
            {
                if (item.IsDirectory)
                    continue;

                string fileName = item.Name.ToLower();
                if (fileName.Contains(".jpg") ||
                    fileName.Contains(".jpeg") ||
                    fileName.Contains(".png") ||
                    fileName.Contains(".bmp") ||
                    fileName.Contains(".gif"))
                {
                    aasx.AddThumbnail(item.PhysicalPath);
                }
            }
        }

        private void AddFilesToAASX(IFileProvider fileProvider, string path, AASX aasx)
        {
            foreach (var item in fileProvider.GetDirectoryContents(path))
            {
                if (item.IsDirectory)
                {
                    AddFilesToAASX(fileProvider, path + "/" + item.Name, aasx);
                }
                else
                {
                    if (item.Exists)
                        aasx.AddFileToAASX("/" + path + "/" + item.Name, item.PhysicalPath);
                }
            }
        }

        /// <summary>
        /// Returns the thumbnail of the AASX package
        /// </summary>
        /// <returns>AASX Package as download</returns>
        /// <response code="200">Success</response>     
        [HttpGet("aasx/thumbnail", Name = "GetThumbnail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetThumbnail()
        {
            IFileProvider fileProvider = hostingEnvironment.ContentRootFileProvider;
            var files = fileProvider.GetDirectoryContents("");
            if (files?.Count() > 0)
            {
                foreach (var file in files)
                {
                    if (file.IsDirectory)
                        continue;

                    string fileName = file.Name.ToLower();
                    if (fileName.Contains(".jpg") ||
                        fileName.Contains(".jpeg") ||
                        fileName.Contains(".png") ||
                        fileName.Contains(".bmp") ||
                        fileName.Contains(".gif"))
                    {
                        if (MimeTypes.TryGetContentType(file.PhysicalPath, out string contentType))
                            return File(file.CreateReadStream(), contentType);
                    }
                }
            }
            return NotFound();
        }
    }
}