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
using BaSyx.Models.AdminShell;
using BaSyx.Servers.AdminShell.Http;
using BaSyx.Utils.Settings;
using NLog.Web;
using System;

namespace SimpleAssetAdministrationShell
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            AssetAdministrationShell aas = SimpleAssetAdministrationShell.GetAssetAdministrationShell();
            ISubmodel testSubmodel = aas.Submodels["TestSubmodel"];

            ServerSettings submodelServerSettings = ServerSettings.CreateSettings();
            submodelServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            submodelServerSettings.ServerConfig.Hosting.Environment = "Development";
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("http://+:5040");
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("https://+:5440");

            SubmodelHttpServer submodelServer = new SubmodelHttpServer(submodelServerSettings);
            submodelServer.WebHostBuilder.UseNLog();
            ISubmodelServiceProvider submodelServiceProvider = testSubmodel.CreateServiceProvider();
            submodelServer.SetServiceProvider(submodelServiceProvider);
            submodelServiceProvider.UseAutoEndpointRegistration(submodelServerSettings.ServerConfig);
            submodelServer.AddBaSyxUI(PageNames.SubmodelServer);
            submodelServer.AddSwagger(Interface.Submodel);
            _ = submodelServer.RunAsync();

            ServerSettings aasServerSettings = ServerSettings.CreateSettings();
            aasServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            aasServerSettings.ServerConfig.Hosting.Environment = "Development";
            aasServerSettings.ServerConfig.Hosting.Urls.Add("http://+:5080");
            aasServerSettings.ServerConfig.Hosting.Urls.Add("https://+:5443");
            aasServerSettings.Miscellaneous.Add("CompanyLogo", "/images/MyCompanyLogo.png");

            IAssetAdministrationShellServiceProvider serviceProvider = aas.CreateServiceProvider(true);
            serviceProvider.SubmodelProviderRegistry.RegisterSubmodelServiceProvider(testSubmodel.Identification.Id, submodelServiceProvider);
            serviceProvider.UseAutoEndpointRegistration(aasServerSettings.ServerConfig);

            AssetAdministrationShellHttpServer aasServer = new AssetAdministrationShellHttpServer(aasServerSettings);
            aasServer.WebHostBuilder.UseNLog();
            aasServer.SetServiceProvider(serviceProvider);
            aasServer.AddBaSyxUI(PageNames.AssetAdministrationShellServer);
            aasServer.AddSwagger(Interface.AssetAdministrationShell);
            aasServer.Run();
        }     
    }
}
