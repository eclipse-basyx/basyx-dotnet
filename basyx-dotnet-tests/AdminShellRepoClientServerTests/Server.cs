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
using BaSyx.API.ServiceProvider;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.Settings;
using SimpleAssetAdministrationShell;
using System.Collections.Generic;

namespace AdminShellRepoClientServerTests
{
    class Server
    {
        public static string ServerUrl = "http://localhost:6999";
        public static void Run()
        {
            ServerSettings settings = new ServerSettings()
            {
               ServerConfig = new ServerConfiguration()
               {
                   Hosting = new HostingConfiguration()
                   {
                       Urls = new List<string>() { ServerUrl }
                   }
               }
            };
            AssetAdministrationShellRepositoryHttpServer aasRepoServer = new AssetAdministrationShellRepositoryHttpServer(settings);
            AssetAdministrationShell testShell = TestAssetAdministrationShell.GetAssetAdministrationShell("MainAdminShell");
            Submodel mainSubmodel = TestSubmodel.GetSubmodel("MainSubmodel");
            testShell.Submodels.Add(mainSubmodel);

            AssetAdministrationShellRepositoryServiceProvider aasRepoServiceProvider = new AssetAdministrationShellRepositoryServiceProvider();
            IAssetAdministrationShellServiceProvider aasServiceProvider = testShell.CreateServiceProvider(true);
            aasRepoServiceProvider.RegisterAssetAdministrationShellServiceProvider(testShell.Id, aasServiceProvider);

            aasRepoServer.SetServiceProvider(aasRepoServiceProvider);
            aasRepoServiceProvider.UseAutoEndpointRegistration(settings.ServerConfig);
            _ = aasRepoServer.RunAsync();
        }
    }
}
