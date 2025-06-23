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

using System;
using BaSyx.API.ServiceProvider;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Servers.AdminShell.Http;
using BaSyx.Utils.Settings;
using NLog.Web;

namespace SubmodelDev
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            
            var aas = DevelopmentSubmodel.DevelopmentSubmodel.GetAssetAdministrationShell();
            var submodel = aas.Submodels[0];
            var submodel2 = aas.Submodels[1];

            // Settings
            // Submodel Server
            var submodelSettings = ServerSettings.CreateSettings();
            submodelSettings.ServerConfig.Hosting.ContentPath = "Content";
            submodelSettings.ServerConfig.Hosting.Environment = "Development";
            submodelSettings.ServerConfig.Hosting.Urls.Add("http://+:5030");
            submodelSettings.ServerConfig.Hosting.Urls.Add("https://+:5430");
            // Submodel Repository Server
            var submodelRepoSettings = ServerSettings.CreateSettings();
            submodelRepoSettings.ServerConfig.Hosting.ContentPath = "Content";
            submodelRepoSettings.ServerConfig.Hosting.Environment = "Development";
            submodelRepoSettings.ServerConfig.Hosting.Urls.Add("http://+:5031");
            submodelRepoSettings.ServerConfig.Hosting.Urls.Add("https://+:5431");
            //AAS Server
            var aasSettings = ServerSettings.CreateSettings();
            aasSettings.ServerConfig.Hosting.ContentPath = "Content";
            aasSettings.ServerConfig.Hosting.Environment = "Development";
            aasSettings.ServerConfig.Hosting.Urls.Add("http://+:5032");
            aasSettings.ServerConfig.Hosting.Urls.Add("https://+:5432");
            //// AAS Repository Server
            var aasRepoSettings = ServerSettings.CreateSettings();
            aasRepoSettings.ServerConfig.Hosting.ContentPath = "Content";
            aasRepoSettings.ServerConfig.Hosting.Environment = "Development";
            aasRepoSettings.ServerConfig.Hosting.Urls.Add("http://+:5033");
            aasRepoSettings.ServerConfig.Hosting.Urls.Add("https://+:5433");

            //SM Service Porvider
            var smServiceProvider = submodel.CreateServiceProvider();
            var sm2ServiceProvider = submodel2.CreateServiceProvider();

            // Server configuration
            // Submodel Server------------------------------------------------------
            var submodelServer = new SubmodelHttpServer(submodelSettings);
            submodelServer.WebHostBuilder.UseNLog();
            submodelServer.AddBaSyxUI(PageNames.SubmodelServer);
            submodelServer.AddSwagger(Interface.Submodel);

            smServiceProvider.UseAutoEndpointRegistration(submodelSettings.ServerConfig);
            submodelServer.SetServiceProvider(smServiceProvider);
            //submodelServer.Run();
            _ = submodelServer.RunAsync();

            // Submodel Repository Server------------------------------------------------------
            var submodelRepoServer = new SubmodelRepositoryHttpServer(submodelRepoSettings);
            submodelRepoServer.WebHostBuilder.UseNLog();
            submodelRepoServer.AddBaSyxUI(PageNames.SubmodelRepositoryServer);
            submodelRepoServer.AddSwagger(Interface.SubmodelRepository);

            var submodelRepoServiceProvider = new SubmodelRepositoryServiceProvider();
            submodelRepoServiceProvider.UseAutoEndpointRegistration(submodelRepoServer.Settings.ServerConfig);
            
            submodelRepoServiceProvider.RegisterSubmodelServiceProvider(submodel.Id, smServiceProvider);
            submodelRepoServiceProvider.RegisterSubmodelServiceProvider(submodel2.Id, sm2ServiceProvider);
            submodelRepoServer.SetServiceProvider(submodelRepoServiceProvider);
            //submodelRepoServer.Run();
            _ = submodelRepoServer.RunAsync();

            ////AAS Server------------------------------------------------------
            var aasServer = new AssetAdministrationShellHttpServer(aasSettings);
            aasServer.WebHostBuilder.UseNLog();
            aasServer.AddBaSyxUI(PageNames.AssetAdministrationShellServer);
            aasServer.AddSwagger(Interface.AssetAdministrationShell);

            var aasServiceProvider = aas.CreateServiceProvider(true);
            aasServiceProvider.UseAutoEndpointRegistration(aasSettings.ServerConfig);
            aasServiceProvider.SubmodelProviderRegistry.RegisterSubmodelServiceProvider(submodel.Id, smServiceProvider);

            aasServer.SetServiceProvider(aasServiceProvider);
            //aasServer.Run();
            _ = aasServer.RunAsync();

            //// AAS Repository Server------------------------------------------------------
            var aasRepoServer = new AssetAdministrationShellRepositoryHttpServer(aasRepoSettings);
            aasRepoServer.WebHostBuilder.UseNLog();
            aasRepoServer.AddBaSyxUI(PageNames.AssetAdministrationShellRepositoryServer);
            aasRepoServer.AddSwagger(Interface.AssetAdministrationShellRepository);

            var repositoryService = new AssetAdministrationShellRepositoryServiceProvider();
            repositoryService.RegisterAssetAdministrationShellServiceProvider(aas.Id, aasServiceProvider);
            repositoryService.UseAutoEndpointRegistration(aasRepoServer.Settings.ServerConfig);

            aasRepoServer.SetServiceProvider(repositoryService); 
            aasRepoServer.Run();
        }
    }
}
