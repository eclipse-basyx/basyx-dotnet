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

using System.Collections.Generic;
using BaSyx.API.ServiceProvider;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Models.Connectivity;
using BaSyx.Servers.AdminShell.Http;
using BaSyx.Utils.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using Endpoint = BaSyx.Models.Connectivity.Endpoint;

namespace BaSyx.AASX.SM.Server.Http.App
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


        static void Main(string[] args)
        {
            Logger.Info("Starting AASX Http-Server...");

            var reverseProxyUrl = "http://0.0.0.0:5043";
            var submodelRepoPort = "5041";
            var aasRepoPort = "5042";

            // SM Repo Server
            var smRepoSettings = ServerSettings.CreateSettings();
            smRepoSettings.ServerConfig.Hosting.ContentPath = "Content";
            smRepoSettings.ServerConfig.Hosting.Environment = "Development";
            smRepoSettings.ServerConfig.Hosting.Urls.Add($"http://0.0.0.0:{submodelRepoPort}");

            var smRepositoryServer = new SubmodelRepositoryHttpServer(smRepoSettings);
            smRepositoryServer.WebHostBuilder.UseNLog();
            var smRepositoryService = new SubmodelRepositoryServiceProvider();
            var smEndpoints = smRepoSettings.ServerConfig.Hosting.Urls.ConvertAll(c =>
            {
                Logger.Info("Using " + c + " as submodel repository base endpoint url");
                return new Endpoint(c, InterfaceName.SubmodelInterface);
            });
            smRepositoryService.UseDefaultEndpointRegistration(smEndpoints);
            smRepositoryServer.SetServiceProvider(smRepositoryService);

            smRepositoryServer.AddBaSyxUI(PageNames.SubmodelRepositoryServer);
            smRepositoryServer.AddSwagger(Interface.SubmodelRepository);

            //smRepositoryServer.Run();
            _ = smRepositoryServer.RunAsync();

            Logger.Info("Starting Submodel Http-Server...");

            // AAS Repo Server
            var aasRepoSettings = ServerSettings.CreateSettings();
            aasRepoSettings.ServerConfig.Hosting.ContentPath = "Content";
            aasRepoSettings.ServerConfig.Hosting.Environment = "Development";
            aasRepoSettings.ServerConfig.Hosting.Urls.Add($"http://0.0.0.0:{aasRepoPort}");

            var aasRepositoryServer = new AssetAdministrationShellRepositoryHttpServer(aasRepoSettings);
            aasRepositoryServer.WebHostBuilder.UseNLog();
            var aasRepositoryService = new AssetAdministrationShellRepositoryServiceProvider();
            var aasEndpoints = aasRepoSettings.ServerConfig.Hosting.Urls.ConvertAll(url =>
            {
                Logger.Info("Using " + url + " as aas repository base endpoint url");
                return new Endpoint(url, InterfaceName.AssetAdministrationShellRepositoryInterface);
            });
            aasRepositoryService.UseDefaultEndpointRegistration(aasEndpoints);
            aasRepositoryServer.SetServiceProvider(aasRepositoryService);

            aasRepositoryServer.AddBaSyxUI(PageNames.AssetAdministrationShellRepositoryServer);
            aasRepositoryServer.AddSwagger(Interface.AssetAdministrationShellRepository);

            //aasRepositoryServer.Run();
            _ = aasRepositoryServer.RunAsync();

            // Start YARP reverse proxy on public port (e.g. 5043)
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls(reverseProxyUrl);
            Logger.Info($"Reverse Proxy URL: {reverseProxyUrl}");

            builder.Services.AddReverseProxy()
                .LoadFromMemory(
                    [
                        new Yarp.ReverseProxy.Configuration.RouteConfig
                        {
                            RouteId = "shells",
                            Match = new Yarp.ReverseProxy.Configuration.RouteMatch { Path = "/shells/{**catch-all}" },
                            ClusterId = "aasCluster"
                        },
                        new Yarp.ReverseProxy.Configuration.RouteConfig
                        {
                            RouteId = "submodels",
                            Match = new Yarp.ReverseProxy.Configuration.RouteMatch { Path = "/submodels/{**catch-all}" },
                            ClusterId = "submodelCluster"
                        }
                    ],
                    [
                        new Yarp.ReverseProxy.Configuration.ClusterConfig
                        {
                            ClusterId = "aasCluster",
                            Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
                            {
                                { "shells", new Yarp.ReverseProxy.Configuration.DestinationConfig { Address = $"http://127.0.0.1:{aasRepoPort}/" }}
                            }
                        },
                        new Yarp.ReverseProxy.Configuration.ClusterConfig
                        {
                            ClusterId = "submodelCluster",
                            Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
                            {
                                { "submodels", new Yarp.ReverseProxy.Configuration.DestinationConfig { Address = $"http://127.0.0.1:{submodelRepoPort}/" }}
                            }
                        }
                    ]
                );

            var app = builder.Build();
            app.MapReverseProxy();
            app.MapGet("/", () => Results.Json(new
            {
                message = "Welcome to Fluid4.0 AAS / Submodel Server",
                status_code = 200,
                aas_repository_server_url = $"http://127.0.0.1:{aasRepoPort}/",
                submodel_repository_server_url = $"http://127.0.0.1:{submodelRepoPort}/"
            }));
            app.Run();
        }
    }
}
