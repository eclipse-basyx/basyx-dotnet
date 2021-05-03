/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Export;
using BaSyx.Utils.Settings.Sections;
using BaSyx.Utils.Settings.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using System.Web;

namespace BaSyx.WebUI
{
    public class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private static ServerSettings serverSettings;
        private static AssetAdministrationShellHttpServer shellServer;
        private static IAssetAdministrationShellServiceProvider shellProvider;

        public static void Main(string[] args)
        {
            serverSettings = ServerSettings.LoadSettings();
            shellServer = new AssetAdministrationShellHttpServer(serverSettings);

            string websiteHostName = Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%");
            if (!(string.IsNullOrEmpty(websiteHostName) || websiteHostName == "%WEBSITE_HOSTNAME%"))
            {
                string websiteUrl = string.Format("https://{0}", websiteHostName);
                serverSettings.ServerConfig = new ServerConfiguration()
                {
                    Hosting = new HostingConfiguration() { Urls = new List<string>() { websiteUrl } }
                };
            }               

            //Add BaSyx Web UI
            shellServer.AddBaSyxUI(PageNames.AssetAdministrationShellServer);

            //Add Swagger Documentation and UI
            shellServer.AddSwagger(Interface.AssetAdministrationShell);

            shellServer.ConfigureServices(services =>
            {
                services.AddTransient<IAssetAdministrationShellServiceProvider>(factory =>
                {
                    if (shellProvider == null)
                        return CreateDefaultShellProvider();
                    else
                        return shellProvider;
                });
            });

            shellServer.Configure(app =>
            {
                app.Use(async (context, next) =>
                {
                    try
                    {
                        string contentPath = Path.Combine(ServerSettings.ExecutingDirectory + "Content");
                        if (Directory.Exists(contentPath))
                           Directory.Delete(contentPath, true);

                        var requestPath = context.Request.Path.ToUriComponent();
                        if (requestPath.Contains("/ui"))
                        {
                            if (context.Request.Query.TryGetValue("path", out StringValues pathValue))
                            {
                                logger.Info("Middleware-Query-String: " + pathValue);
                                bool success = false;
                                if (Uri.TryCreate(pathValue, UriKind.Absolute, out Uri pathUri))
                                {
                                    var net = new System.Net.WebClient();
                                    var data = net.DownloadData(pathUri);
                                    MemoryStream memoryStream = new MemoryStream(data);
                                    Package package = Package.Open(memoryStream, FileMode.Open, FileAccess.Read);
                                    AASX aasx = new AASX(package);
                                    success = LoadAASX(aasx);
                                }
                                else
                                    success = false;

                                if (!success)
                                {
                                    logger.Info("success is false on " + pathValue);
                                    shellProvider = null;
                                }
                                context.Request.Path = new PathString("/ui");                         
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e, "Middleware-Exception: " + e.Message);
                    }
                  
                    await next();
                });
            });
            shellServer.Run();
        }

        private static IAssetAdministrationShellServiceProvider CreateDefaultShellProvider()
        {
            IAssetAdministrationShell shell = new AssetAdministrationShell("DefaultShell", new Identifier(Guid.NewGuid().ToString(), KeyType.Custom))
            {
                Asset = new Asset("DefaultAsset", new Identifier(Guid.NewGuid().ToString(), KeyType.Custom)),
                Submodels = { new Submodel("DefaultSubmodel", new Identifier(Guid.NewGuid().ToString(), KeyType.Custom)) }
            };

            var provider = shell.CreateServiceProvider(true);
            provider.UseAutoEndpointRegistration(serverSettings.ServerConfig);
            return provider;
        }

        private static bool LoadAASX(AASX aasx)
        {
            using (aasx)
            {
                IAssetAdministrationShell shell;
                try
                {
                    AssetAdministrationShellEnvironment_V2_0 environment = aasx.GetEnvironment_V2_0();
                    shell = environment.AssetAdministrationShells.FirstOrDefault();
                    if (shell == null)
                    {
                        logger.Error("Asset Administration Shell cannot be obtained from AASX-Package");
                        return false;
                    }
                }
                catch (Exception e1)
                {
                    logger.Warn(e1, "Error obtaining Asset Administration Shell from package. Trying AAS-Version 1.0 ...");
                    try
                    {
                        AssetAdministrationShellEnvironment_V1_0 environment = aasx.GetEnvironment_V1_0();
                        shell = environment.AssetAdministrationShells.FirstOrDefault();
                        if (shell == null)
                        {
                            logger.Error("Asset Administration Shell cannot be obtained from AASX-Package");
                            return false;
                        }
                    }
                    catch (Exception e2)
                    {
                        logger.Error(e2, "Asset Administration Shell cannot be obtained from AASX-Package");
                        return false;
                    }

                }

                logger.Info("AASX-Package successfully loaded");

                PackagePart thumbnailPart = aasx.GetThumbnailAsPackagePart();
                RegisterShellServiceProvider(shell, aasx.SupplementaryFiles, thumbnailPart);
            }
            return true;
        }

        private static void RegisterShellServiceProvider(IAssetAdministrationShell shell, List<PackagePart> supplementaryFiles, PackagePart thumbnailPart)
        {
            var aasServiceProvider = shell.CreateServiceProvider(true);
            aasServiceProvider.UseAutoEndpointRegistration(serverSettings.ServerConfig);

            foreach (var file in supplementaryFiles)
                using (Stream stream = file.GetStream())
                    shellServer.ProvideContent(file.Uri, stream);

            if (thumbnailPart != null)
                using (Stream thumbnailStream = thumbnailPart.GetStream())
                    shellServer.ProvideContent(thumbnailPart.Uri, thumbnailStream);

            shellProvider = aasServiceProvider;
        }
    }
}
