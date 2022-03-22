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
using BaSyx.Submodel.Server.Http;
using BaSyx.Utils.Settings;
using SimpleAssetAdministrationShell;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubmodelClientServerTests
{
    class Server
    {
        public static string ServerUrl = "http://localhost:5070";
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
            SubmodelHttpServer submodelServer = new SubmodelHttpServer(settings);
            Submodel testSubmodel = TestSubmodel.GetSubmodel("TestSubmodel");
            ISubmodelServiceProvider submodelServiceProvider = testSubmodel.CreateServiceProvider();
            submodelServer.SetServiceProvider(submodelServiceProvider);
            submodelServiceProvider.UseAutoEndpointRegistration(settings.ServerConfig);
            _ = submodelServer.RunAsync();
        }
    }
}
