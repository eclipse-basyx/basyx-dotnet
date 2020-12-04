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
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.API.Components;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Utils.Settings.Types;
using System;

namespace ClockAssetAdministrationShell
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            /* Clock Asset Administration Shell */
            AssetAdministrationShell clockAAS = ClockAssetAdministrationShell.GetAssetAdministrationShell();

            ServerSettings clockAASServerSettings = ServerSettings.CreateSettings();
            clockAASServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            clockAASServerSettings.ServerConfig.Hosting.Environment = "Development";
            clockAASServerSettings.ServerConfig.Hosting.Urls.Add("http://+:5080");
            clockAASServerSettings.ServerConfig.Hosting.Urls.Add("https://+:5443");

            IAssetAdministrationShellServiceProvider clockServiceProvider = clockAAS.CreateServiceProvider(true);
            clockServiceProvider.UseAutoEndpointRegistration(clockAASServerSettings.ServerConfig);

            AssetAdministrationShellHttpServer clockAASServer = new AssetAdministrationShellHttpServer(clockAASServerSettings);
            clockAASServer.SetServiceProvider(clockServiceProvider);
            clockAASServer.AddBaSyxUI(PageNames.AssetAdministrationShellServer);
            clockAASServer.AddSwagger(Interface.AssetAdministrationShell);
            _ = clockAASServer.RunAsync();

            /* Hours Asset Administration Shell */

            AssetAdministrationShell hoursAAS = HourAssetAdministrationShell.GetAssetAdministrationShell();

            ServerSettings hoursAASServerSettings = ServerSettings.CreateSettings();
            hoursAASServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            hoursAASServerSettings.ServerConfig.Hosting.Environment = "Development";
            hoursAASServerSettings.ServerConfig.Hosting.Urls.Add("http://+:5081");
            hoursAASServerSettings.ServerConfig.Hosting.Urls.Add("https://+:5444");

            IAssetAdministrationShellServiceProvider hoursServiceProvider = hoursAAS.CreateServiceProvider(true);
            hoursServiceProvider.UseAutoEndpointRegistration(hoursAASServerSettings.ServerConfig);

            AssetAdministrationShellHttpServer hoursAASServer = new AssetAdministrationShellHttpServer(hoursAASServerSettings);
            hoursAASServer.SetServiceProvider(hoursServiceProvider);
            hoursAASServer.AddBaSyxUI(PageNames.AssetAdministrationShellServer);
            hoursAASServer.AddSwagger(Interface.AssetAdministrationShell);
            hoursAASServer.Run();
        }     
    }
}
