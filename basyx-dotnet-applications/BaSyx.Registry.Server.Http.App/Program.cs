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
using BaSyx.Discovery.mDNS;
using BaSyx.Utils.Settings;
using NLog;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using NLog.Web;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography.X509Certificates;
using BaSyx.Deployment.AppDataService;
using BaSyx.Registry.ReferenceImpl.InMemory;

namespace BaSyx.Registry.Server.Http.App
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static AppDataService AppDataService { get; set; }

        static void Main(string[] args)
        {
            logger.Info("Starting AdminShell Registry HTTP server...");

            AppDataService = AppDataService.Create("adminshell-registry", "appsettings.json", args);

            //Loading server configurations settings from ServerSettings.xml;
            ServerSettings serverSettings = AppDataService.GetSettings<ServerSettings>();

            //Instantiate blank Registry-Http-Server with previously loaded server settings
            RegistryHttpServer server = new RegistryHttpServer(serverSettings);
            
            //Configure the entire application to use your own logger library (here: Nlog)
            server.WebHostBuilder.UseNLog();

            //Configure the pathbase as default prefix for all routes
            if (!string.IsNullOrEmpty(serverSettings.ServerConfig.PathBase))
                server.UsePathBase(serverSettings.ServerConfig.PathBase);

            //Check if ServerCertificate is present
            if (!string.IsNullOrEmpty(serverSettings.ServerConfig.Security.ServerCertificatePath))
            {
                server.WebHostBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.ConfigureHttpsDefaults(listenOptions =>
                    {
                        X509Certificate2 certificate = new X509Certificate2(
                            serverSettings.ServerConfig.Security.ServerCertificatePath,
                            serverSettings.ServerConfig.Security.ServerCertificatePassword);
                        listenOptions.ServerCertificate = certificate;
                    });
                });
            }

            //Instantiate implementation backend for the Registry
            InMemoryRegistry registryImpl = new InMemoryRegistry();                       

            //Assign implemenation backend to blank Registry-Http-Server
            server.SetRegistryProvider(registryImpl);

            //Start mDNS Discovery ability when the server successfully booted up
            server.ApplicationStarted = () =>
            {
                if (serverSettings.DiscoveryConfig.AutoDiscovery)
                {
                    registryImpl.StartDiscovery();
                }                
            };

            //Start mDNS Discovery when the server is shutting down
            server.ApplicationStopping = () =>
            {
                if (serverSettings.DiscoveryConfig.AutoDiscovery)
                {
                    registryImpl.StopDiscovery();
                }                
            };

            //Add BaSyx Web UI
            server.AddBaSyxUI(PageNames.AssetAdministrationShellRegistryServer);

            //Add Swagger Documentation and UI
            server.AddSwagger(Interface.AssetAdministrationShellRegistry);

            //Run the server
            server.Run();
        }
    }
}
