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
using BaSyx.Servers.AdminShell.Http;
using BaSyx.API.ServiceProvider;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Models.Export;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using NLog;
using NLog.Web;
using BaSyx.Utils.Settings;
using BaSyx.Models.AdminShell;
using System.Net.Http;
using BaSyx.API.Clients;
using BaSyx.Clients.AdminShell.Http;

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
            shellServer.WebHostBuilder.UseNLog();

            string websiteHostName = System.Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%");
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
                        string contentPath = Path.Combine(serverSettings.ExecutionPath + "Content");
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
                                    var client = new HttpClient();
                                    var response = await client.GetAsync(pathUri);

                                    using(MemoryStream stream = new MemoryStream())
                                    {
                                        await response.Content.CopyToAsync(stream);
                                        Package package = Package.Open(stream, FileMode.Open, FileAccess.Read);
                                        AASX_V2_0 aasx = new AASX_V2_0(package);
                                        success = LoadAASX(aasx);
                                    }                            
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
                            else if (context.Request.Query.TryGetValue("remoteClient", out StringValues remoteClientValue))
                            {
                                logger.Info("Middleware-Query-String: " + remoteClientValue);
                                bool success = false;
                                if (Uri.TryCreate(remoteClientValue, UriKind.Absolute, out Uri pathUri))
                                {
                                    AssetAdministrationShellHttpClient client = new AssetAdministrationShellHttpClient(pathUri);
                                    shellProvider = client.CreateServiceProvider();

                                    var submodelRefs_retrieved = await client.RetrieveAllSubmodelReferencesAsync();
                                    if (submodelRefs_retrieved.Success)
                                    {
                                        foreach (var submodelRef in submodelRefs_retrieved.Entity)
                                        {
                                            var submodelClient = client.CreateSubmodelClient(submodelRef.First.Value);
                                            var submodelProvider = submodelClient.CreateServiceProvider();
                                            shellProvider.SubmodelProviderRegistry.RegisterSubmodelServiceProvider(submodelRef.First.Value, submodelProvider);
                                        }
                                    }

                                    success = true;
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
            IAssetAdministrationShell shell = new AssetAdministrationShell("DefaultShell", new Identifier(Guid.NewGuid().ToString()))
            {
                AssetInformation = new AssetInformation() { GlobalAssetId = new Identifier(Guid.NewGuid().ToString()) },
                Submodels = { new Submodel("DefaultSubmodel", new Identifier(Guid.NewGuid().ToString())) }
            };

            var provider = shell.CreateServiceProvider(true);
            provider.UseAutoEndpointRegistration(serverSettings.ServerConfig);
            return provider;
        }

        private static bool LoadAASX(AASX_V2_0 aasx)
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
                    logger.Error(e1, "Asset Administration Shell cannot be obtained from AASX-Package");
                    return false;
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
