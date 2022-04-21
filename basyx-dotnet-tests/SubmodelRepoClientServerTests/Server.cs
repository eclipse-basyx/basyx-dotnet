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
using BaSyx.API.ServiceProvider;
using BaSyx.Models.AdminShell;
using BaSyx.Servers.AdminShell.Http;
using BaSyx.Utils.Settings;
using SimpleAssetAdministrationShell;
using System.Collections.Generic;

namespace SubmodelRepoClientServerTests
{
    class Server
    {
        public static string ServerUrl = "http://localhost:5999";
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
            SubmodelRepositoryHttpServer submodelRepoServer = new SubmodelRepositoryHttpServer(settings);
            Submodel mainSubmodel = TestSubmodel.GetSubmodel("MainSubmodel");

            SubmodelRepositoryServiceProvider submodelRepoServiceProvider = new SubmodelRepositoryServiceProvider();
            ISubmodelServiceProvider submodelServiceProvider = mainSubmodel.CreateServiceProvider();
            submodelRepoServiceProvider.RegisterSubmodelServiceProvider(mainSubmodel.Identification.Id, submodelServiceProvider);

            submodelRepoServer.SetServiceProvider(submodelRepoServiceProvider);
            submodelRepoServiceProvider.UseAutoEndpointRegistration(settings.ServerConfig);
            _ = submodelRepoServer.RunAsync();
        }
    }
}
