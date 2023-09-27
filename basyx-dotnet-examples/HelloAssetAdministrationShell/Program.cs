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
using BaSyx.Discovery.mDNS;
using BaSyx.Utils.Settings;
using NLog;
using NLog.Web;

namespace HelloAssetAdministrationShell
{
    class Program
    {
        //Create logger for the application
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("Starting HelloAssetAdministrationShell's HTTP server...");

            //Loading server configurations settings from ServerSettings.xml;
            ServerSettings serverSettings = ServerSettings.LoadSettingsFromFile("ServerSettings.xml");

            //Initialize generic HTTP-REST interface passing previously loaded server configuration
            AssetAdministrationShellHttpServer server = new AssetAdministrationShellHttpServer(serverSettings);

            //Configure the entire application to use your own logger library (here: Nlog)
            server.WebHostBuilder.UseNLog();

            //Instantiate Asset Administration Shell Service
            HelloAssetAdministrationShellService shellService = new HelloAssetAdministrationShellService();

            //Dictate Asset Administration Shell service to use provided endpoints from the server configuration
            shellService.UseAutoEndpointRegistration(serverSettings.ServerConfig);

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
                shellService.StartDiscovery();
            };

            //Action that gets executed when server is shutting down
            server.ApplicationStopping = () =>
            {
                //Stop mDNS discovery thread
                shellService.StopDiscovery();
            };

            //Run HTTP server
            server.Run();           
        }
    }
}
