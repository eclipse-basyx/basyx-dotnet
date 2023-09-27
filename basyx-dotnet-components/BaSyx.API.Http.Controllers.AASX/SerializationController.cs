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
using Microsoft.AspNetCore.Mvc;
using BaSyx.API.ServiceProvider;
using BaSyx.Models.Export;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;
using BaSyx.Models.AdminShell;
using System.Linq;
using System.Collections.Generic;

namespace BaSyx.API.Http.Controllers.PackageService
{
    /// <summary>
    /// The Serialization Controller
    /// </summary>
    public class SerializationController : Controller
    {
        private readonly IServiceProvider serviceProvider;

#if NETCOREAPP3_1
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// Constructor for the Serialization Controller
        /// </summary>
        /// <param name="serviceProvider">The Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public PackageService(IServiceProvider serviceProvider, IWebHostEnvironment environment)
        {
            this.serviceProvider = serviceProvider;
            hostingEnvironment = environment;
        }
#else
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// Constructor for the Serialization Controller
        /// </summary>
        /// <param name="serviceProvider">The Service Provider implementation provided by the dependency injection</param>
        /// <param name="environment">The Hosting Environment provided by the dependency injection</param>
        public SerializationController(IServiceProvider serviceProvider, IHostingEnvironment environment)
        {
            this.serviceProvider = serviceProvider;
            hostingEnvironment = environment;
        }


#endif

        /// <summary>
        /// Returns an appropriate serialization based on the specified format (see SerializationFormat)
        /// </summary>
        /// <param name="aasIds">The Asset Administration Shells' unique ids (BASE64-URL-encoded)</param>
        /// <param name="submodelIds">The Submodels' unique ids (BASE64-URL-encoded)</param>
        /// <param name="includeConceptDescriptions">Include Concept Descriptions?</param>
        /// <returns></returns>
        [HttpGet("serialization", Name = "GenerateSerializationByIds")]
        [ProducesResponseType(200)]
        public IActionResult GenerateSerializationByIds([FromQuery(Name = "aasId[]")] List<string> aasIds, [FromQuery(Name = "submodelId[]")] List<string> submodelIds, [FromQuery] bool includeConceptDescriptions)
        {
            AssetAdministrationShellEnvironment_V2_0 environment = new AssetAdministrationShellEnvironment_V2_0();
            if (serviceProvider is IAssetAdministrationShellRepositoryServiceProvider aasRepoServiceProvider)
            {
                foreach (var aasId in aasIds)
                {
                    var retrieved_aasSP = aasRepoServiceProvider.ShellProviderRegistry.GetAssetAdministrationShellServiceProvider(aasId);
                    if (retrieved_aasSP.Success)
                    {
                        var retrieved_aas = retrieved_aasSP.Entity.RetrieveAssetAdministrationShell(default);
                        if (retrieved_aas.Success)
                            environment.AddAssetAdministrationShell(retrieved_aas.Entity, false);

                        var submodel = GetSubmodel(retrieved_aas.Entity, retrieved_aasSP.Entity.SubmodelProviderRegistry, submodelIds);
                        if(submodel != null && environment.Submodels.FindIndex(s => s.Identification.Id == submodel.Identification.Id) == -1)
                            environment.AddSubmodel(submodel);
                    }
                }
            }
            else if (serviceProvider is IAssetAdministrationShellServiceProvider aasSP)
            {
                var retrieved_aas = aasSP.RetrieveAssetAdministrationShell(default);
                if (retrieved_aas.Success && aasIds.Any(a => a == retrieved_aas.Entity.Identification.Id))
                {
                    environment.AddAssetAdministrationShell(retrieved_aas.Entity, false);

                    var submodel = GetSubmodel(retrieved_aas.Entity, aasSP.SubmodelProviderRegistry, submodelIds);
                    if (submodel != null && environment.Submodels.FindIndex(s => s.Identification.Id == submodel.Identification.Id) == -1)
                        environment.AddSubmodel(submodel);
                }
            }
            else if (serviceProvider is ISubmodelRepositoryServiceProvider submodelRepoSP)
            {
                foreach (var submodelId in submodelIds)
                {
                    var retrieved_submodel = submodelRepoSP.RetrieveSubmodel(submodelId);
                    if (retrieved_submodel.Success && environment.Submodels.FindIndex(s => s.Identification.Id == retrieved_submodel.Entity.Identification.Id) == -1)
                    {
                        environment.AddSubmodel(retrieved_submodel.Entity);
                    }
                }
            }
            else if (serviceProvider is ISubmodelServiceProvider submodelSP)
            {
                var retrieved_submodel = submodelSP.RetrieveSubmodel(default);
                if (retrieved_submodel.Success && submodelIds.Any(a => a == retrieved_submodel.Entity.Identification.Id) && environment.Submodels.FindIndex(s => s.Identification.Id == retrieved_submodel.Entity.Identification.Id) == -1)
                {
                    environment.AddSubmodel(retrieved_submodel.Entity);
                }
            }

            if (!includeConceptDescriptions)
                environment.ConceptDescriptions.Clear();

            environment.BuildEnvironment();

            string acceptType = HttpContext.Request.Headers["Accept"];
            switch (acceptType)
            {
                case "application/json":
                    {
                        return new JsonResult(environment);
                    }
                case "application/xml":
                    { 
                        var serializer = new System.Xml.Serialization.XmlSerializer(environment.GetType());
                        using (StringWriter textWriter = new StringWriter())
                        {
                            serializer.Serialize(textWriter, environment);
                            string xmlString = textWriter.ToString();
                            return this.Content(xmlString, acceptType);
                        }
                    }
                case "application/asset-administration-shell-package+xml":
                    {
                        string fileName = Path.GetRandomFileName() + ".aasx";
                        return GetAASXPackage(fileName, new Identifier("root", KeyType.Custom), environment);
                    }
                default:
                    break;
            }
            return BadRequest();
        }

        private ISubmodel GetSubmodel(IAssetAdministrationShell aas, ISubmodelServiceProviderRegistry smRegistry, List<string> submodelIds)
        {
            foreach (var submodelId in submodelIds)
            {
                if (aas.SubmodelReferences.FirstOrDefault(s => s.First.Value == submodelId) != null)
                {
                    var smProvider = smRegistry.GetSubmodelServiceProvider(submodelId);
                    if (smProvider.Success)
                    {
                        var retrieved_submodel = smProvider.Entity.RetrieveSubmodel();
                        if(retrieved_submodel.Success)
                            return retrieved_submodel.Entity;
                    }
                }
            }
            return null;
        }

        private IActionResult GetAASXPackage(string fileName, Identifier id, AssetAdministrationShellEnvironment_V2_0 environment)
        {
            string aasxFilePath = Path.Combine(hostingEnvironment.ContentRootPath, fileName);
            IFileProvider fileProvider = hostingEnvironment.ContentRootFileProvider;

            using (AASX aasx = new AASX(aasxFilePath))
                aasx.AddEnvironment(id, environment, ExportType.Xml);
      
            var fileInfo = fileProvider.GetFileInfo(fileName);
            var fileResult = new PhysicalFileResult(fileInfo.PhysicalPath, "application/asset-administration-shell-package+xml")
            {
                FileDownloadName = fileName
            };
            return fileResult;
        }
    }
}