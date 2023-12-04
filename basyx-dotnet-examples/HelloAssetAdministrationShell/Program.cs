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
using BaSyx.Servers.AdminShell.Http;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Discovery.mDNS;
using BaSyx.Utils.Settings;
using NLog;
using NLog.Web;
using BaSyx.Deployment.AppDataService;
using BaSyx.Registry.Client.Http;

namespace HelloAssetAdministrationShell
{
    class Program
    {
        //Create logger for the application
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        //In case mDNS auto discovery is not available we use a client to register at the registry
        private static RegistryHttpClient registryHttpClient;

        private static AppDataService AppDataService { get; set; }

        static void Main(string[] args)
        {
            logger.Info("Starting HelloAssetAdministrationShell's HTTP server...");

            AppDataService = AppDataService.Create("hello-adminshell", "appsettings.json", args);            

            //Loading server configurations settings from ServerSettings.xml;
            ServerSettings serverSettings = AppDataService.GetSettings<ServerSettings>();

            //Loading registry client configuration settings from RegistryClientSettings.xml
            RegistryClientSettings registryClientSettings = AppDataService.GetSettings<RegistryClientSettings>();

            //Instantiate registry client passing previously loaded configuration
            registryHttpClient = new RegistryHttpClient(registryClientSettings);

            //Initialize generic HTTP-REST interface passing previously loaded server configuration
            AssetAdministrationShellHttpServer server = new AssetAdministrationShellHttpServer(serverSettings);

            //Configure the entire application to use your own logger library (here: Nlog)
            server.WebHostBuilder.UseNLog();

            //Instantiate Asset Administration Shell Service
            HelloAssetAdministrationShellService shellService = new HelloAssetAdministrationShellService();

            //Dictate Asset Administration Shell service to use provided endpoints from the server configuration
            shellService.UseUniversalEndpointRegistration(serverSettings.ServerConfig, serverSettings.ServerConfig.PathBase);

            //Assign Asset Administration Shell Service to the generic HTTP-REST interface
            server.SetServiceProvider(shellService);

            //Add Swagger documentation and UI
            server.AddSwagger(Interface.AssetAdministrationShell);

            //Add BaSyx Web UI
            server.AddBaSyxUI(PageNames.AssetAdministrationShellServer);

            //Action that gets executued when server is fully started
            server.ApplicationStarted = () =>
            {
                //Use mDNS discovery mechanism in the network. It is used to register at the Registry automatically.
                if (serverSettings.DiscoveryConfig.AutoDiscovery)
                {
                    shellService.StartDiscovery();
                }
                else
                {
                    var result = registryHttpClient.CreateOrUpdate(shellService.ServiceDescriptor);

                    logger.Info($"Success: {result.Success} | Messages: {result.Messages}");
                }
            };

            //Action that gets executed when server is shutting down
            server.ApplicationStopping = () =>
            {
                //Stop mDNS discovery thread
                if (serverSettings.DiscoveryConfig.AutoDiscovery)
                {
                    shellService.StopDiscovery();
                }
                else
                {
                    var result = registryHttpClient.DeleteAssetAdministrationShellRegistration(shellService.ServiceDescriptor.Id);

                    logger.Info($"Success: {result.Success} | Messages: {result.Messages}");
                }
            };

            //Run HTTP server
            server.Run();           
        }
    }
}
