/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.API.ServiceProvider;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Models.AdminShell;
using BaSyx.Servers.AdminShell.Http;
using BaSyx.Utils.Settings;
using NLog.Web;
using System;

namespace DevelopmentSubmodel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            AssetAdministrationShell aas = DevelopmentSubmodel.GetAssetAdministrationShell();
            ISubmodel testSubmodel = aas.Submodels["DevSubmodel"];

            ServerSettings submodelServerSettings = ServerSettings.CreateSettings();
            submodelServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            submodelServerSettings.ServerConfig.Hosting.Environment = "Development";
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("http://+:5080");

            SubmodelHttpServer submodelServer = new SubmodelHttpServer(submodelServerSettings);
            submodelServer.WebHostBuilder.UseNLog();
            ISubmodelServiceProvider submodelServiceProvider = testSubmodel.CreateServiceProvider();
            submodelServer.SetServiceProvider(submodelServiceProvider);
            submodelServiceProvider.UseAutoEndpointRegistration(submodelServerSettings.ServerConfig);
            submodelServer.AddBaSyxUI(PageNames.SubmodelServer);
            submodelServer.AddSwagger(Interface.Submodel);
            submodelServer.Run();
        }     
    }
}
