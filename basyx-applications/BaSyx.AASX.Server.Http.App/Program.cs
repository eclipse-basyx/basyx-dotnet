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
using BaSyx.AAS.Server.Http;
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.API.Components;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Models.Connectivity;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Export;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;

namespace BaSyx.AASX.Server.Http.App
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("Starting AASX Hoster...");

            if(args?.Length == 0)
            {
                logger.Error("No arguments provided --> Application is shutting down...");
                return;
            }
            if (!string.IsNullOrEmpty(args[0]) && File.Exists(args[0]))
            {              
                using (BaSyx.Models.Export.AASX aasx = new BaSyx.Models.Export.AASX(args[0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    AssetAdministrationShellEnvironment_V2_0 environment = aasx.GetEnvironment_V2_0();
                    if(environment == null)
                    {
                        logger.Error("Asset Administration Shell Environment cannot be obtained from AASX-Package " + args[0]);
                        return;
                    }

                    logger.Info("AASX-Package successfully loaded");

                    if (environment.AssetAdministrationShells.Count == 1)
                    {
                        LoadSingleAssetAdministrationShellServer(environment.AssetAdministrationShells.ElementAt(0), aasx.SupplementaryFiles, args);
                    }
                    else if (environment.AssetAdministrationShells.Count > 1)
                    {
                        LoadMultiAssetAdministrationShellServer(environment.AssetAdministrationShells, aasx.SupplementaryFiles, args);
                    }
                    else
                    {
                        logger.Error("No Asset Administration Shells found AASX-Package " + args[0]);
                        return;
                    }
                }
            }
            Console.WriteLine("Press any key to exit...");
            Console.Read();
            logger.Info("Application shut down");
        }

        private static void LoadMultiAssetAdministrationShellServer(List<IAssetAdministrationShell> assetAdministrationShells, List<PackagePart> supplementaryFiles, string[] args)
        {
            AssetAdministrationShellRepositoryHttpServer multiServer = new AssetAdministrationShellRepositoryHttpServer();
            AssetAdministrationShellRepositoryServiceProvider repositoryService = new AssetAdministrationShellRepositoryServiceProvider();

            foreach (var shell in assetAdministrationShells)
            {
                var aasServiceProvider = shell.CreateServiceProvider(true);
                repositoryService.RegisterAssetAdministrationShellServiceProvider(shell.IdShort, aasServiceProvider);
            }

            List<HttpEndpoint> endpoints;
            if (args.Length == 2 && !string.IsNullOrEmpty(args[1]))
            {
                logger.Info("Using " + args[1] + " as host url");
                endpoints = new List<HttpEndpoint>() { new HttpEndpoint(args[1]) };
            }
            else
            {
                endpoints = multiServer.Settings.ServerConfig.Hosting.Urls.ConvertAll(c =>
                {
                    string address = c.Replace("+", "127.0.0.1");
                    logger.Info("Using " + address + " as host url");
                    return new HttpEndpoint(address);
                });

            }

            repositoryService.UseDefaultEndpointRegistration(endpoints);
            multiServer.SetServiceProvider(repositoryService);

            foreach (var file in supplementaryFiles)
            {
                using (Stream stream = file.GetStream())
                {
                    logger.Info("Providing content on server: " + file.Uri);
                    multiServer.ProvideContent(file.Uri, stream);
                }
            }
            multiServer.AddBaSyxUI(PageNames.AssetAdministrationShellRepositoryServer);
            multiServer.AddSwagger(Interface.AssetAdministrationShellRepository);
            multiServer.RunAsync();
        }

        private static void LoadSingleAssetAdministrationShellServer(IAssetAdministrationShell assetAdministrationShell, List<PackagePart> supplementaryFiles, string[] args)
        {
            AssetAdministrationShellHttpServer server = new AssetAdministrationShellHttpServer();
            IAssetAdministrationShellServiceProvider service = assetAdministrationShell.CreateServiceProvider(true);

            List<HttpEndpoint> endpoints;
            if (args.Length == 2 && !string.IsNullOrEmpty(args[1]))
            {
                logger.Info("Using " + args[1] + " as host url");
                endpoints = new List<HttpEndpoint>() { new HttpEndpoint(args[1]) };
            }
            else
            {
                endpoints = server.Settings.ServerConfig.Hosting.Urls.ConvertAll(c =>
                {
                    string address = c.Replace("+", "127.0.0.1");
                    logger.Info("Using " + address + " as host url");
                    return new HttpEndpoint(address);
                });

            }
            service.UseDefaultEndpointRegistration(endpoints);
            server.SetServiceProvider(service);

            server.ConfigureServices(services =>
            {
                Assembly controllerAssembly = Assembly.Load("BaSyx.API.Http.Controllers.AASX");
                services.AddMvc()
                    .AddApplicationPart(controllerAssembly)
                    .AddControllersAsServices();
            });

            foreach (var file in supplementaryFiles)
            {
                using (Stream stream = file.GetStream())
                {
                    logger.Info("Providing content on server: " + file.Uri);
                    server.ProvideContent(file.Uri, stream);
                }
            }
            logger.Info("Server is starting up...");

            server.AddBaSyxUI(PageNames.AssetAdministrationShellServer);
            server.AddSwagger(Interface.AssetAdministrationShell);
            server.RunAsync();
        }
    }
}
