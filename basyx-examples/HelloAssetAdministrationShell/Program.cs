/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Distribution License 1.0 which is available at
* https://www.eclipse.org/org/documents/edl-v10.html
*
* 
*******************************************************************************/
using BaSyx.AAS.Server.Http;
using BaSyx.API.Components;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Discovery.mDNS;
using BaSyx.Utils.Settings.Types;
using NLog;

namespace HelloAssetAdministrationShell
{
    class Program
    {
        //Enable logging
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("Starting Asset Administration Shell's HTTP-REST interface...");

            //Loading server configurations settings from ServerSettings.xml;
            ServerSettings serverSettings = ServerSettings.LoadSettingsFromFile("ServerSettings.xml");

            //Initialize generic HTTP-REST interface passing previously loaded server configuration
            AssetAdministrationShellHttpServer aasServer = new AssetAdministrationShellHttpServer(serverSettings);

            //Instantiate Asset Administration Shell Service
            HelloAssetAdministrationShellService shellService = new HelloAssetAdministrationShellService();

            //Dictate Asset Administration Shell service to use provided endpoints from the server configuration
            shellService.UseAutoEndpointRegistration(serverSettings.ServerConfig);

            //Assign Asset Administration Shell Service to the generic HTTP-REST interface
            aasServer.SetServiceProvider(shellService);

            //Add Swagger documentation and UI
            aasServer.AddSwagger(Interface.AssetAdministrationShell);

            //Add BaSyx Web UI
            aasServer.AddBaSyxUI(PageNames.AssetAdministrationShellServer);

            aasServer.ApplicationStarted = () =>
            {
                shellService.StartDiscovery();
            };

            aasServer.ApplicationStopping = () =>
            {
                shellService.StopDiscovery();
            };

            //Run HTTP-REST interface
            aasServer.Run();           
        }
    }
}
