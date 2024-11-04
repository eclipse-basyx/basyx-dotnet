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

            
            var aas = DevelopmentSubmodel.GetAssetAdministrationShell();
            var submodel = aas.Submodels[0];

            // Submodel Server
            ServerSettings submodelServerSettings = ServerSettings.CreateSettings();
            submodelServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            submodelServerSettings.ServerConfig.Hosting.Environment = "Development";
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("http://+:5040");
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("https://+:5440");

            SubmodelHttpServer submodelServer = new SubmodelHttpServer(submodelServerSettings);
            submodelServer.WebHostBuilder.UseNLog();
            ISubmodelServiceProvider submodelServiceProvider = submodel.CreateServiceProvider();
            submodelServer.SetServiceProvider(submodelServiceProvider);
            submodelServiceProvider.UseAutoEndpointRegistration(submodelServerSettings.ServerConfig);
            submodelServer.AddBaSyxUI(PageNames.SubmodelServer);
            submodelServer.AddSwagger(Interface.Submodel);
            //submodelServer.Run();
            _ = submodelServer.RunAsync();
            
            // AAS Server
            ServerSettings aasServerSettings = ServerSettings.CreateSettings();
            aasServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            aasServerSettings.ServerConfig.Hosting.Environment = "Development";
            aasServerSettings.ServerConfig.Hosting.Urls.Add("http://+:5060");
            aasServerSettings.ServerConfig.Hosting.Urls.Add("https://+:5443");
            aasServerSettings.Miscellaneous.Add("CompanyLogo", "/images/MyCompanyLogo.png");

            IAssetAdministrationShellServiceProvider aasServiceProvider = aas.CreateServiceProvider(true);
            aasServiceProvider.SubmodelProviderRegistry.RegisterSubmodelServiceProvider(submodel.Id, submodelServiceProvider);
            aasServiceProvider.UseAutoEndpointRegistration(aasServerSettings.ServerConfig);

            var aasServer = new AssetAdministrationShellHttpServer(aasServerSettings);
            aasServer.WebHostBuilder.UseNLog();
            aasServer.SetServiceProvider(aasServiceProvider);
            aasServer.AddBaSyxUI(PageNames.AssetAdministrationShellServer);
            aasServer.AddSwagger(Interface.AssetAdministrationShell);
            //aasServer.Run();
            _ = aasServer.RunAsync();


            // AAS Repository Server
            ServerSettings aasRepositorySettings = ServerSettings.CreateSettings();
            aasRepositorySettings.ServerConfig.Hosting.ContentPath = "Content";
            aasRepositorySettings.ServerConfig.Hosting.Urls.Add("http://+:5080");
            aasRepositorySettings.ServerConfig.Hosting.Urls.Add("https://+:5446");

            AssetAdministrationShellRepositoryHttpServer server = new AssetAdministrationShellRepositoryHttpServer(aasRepositorySettings);
            AssetAdministrationShellRepositoryServiceProvider repositoryService = new AssetAdministrationShellRepositoryServiceProvider();
            repositoryService.RegisterAssetAdministrationShellServiceProvider(aas.Id, aasServiceProvider);
            repositoryService.UseAutoEndpointRegistration(server.Settings.ServerConfig);

            server.SetServiceProvider(repositoryService);
            server.AddBaSyxUI(PageNames.AssetAdministrationShellRepositoryServer);
            server.AddSwagger(Interface.AssetAdministrationShellRepository);
            server.Run();
        }     
    }
}
