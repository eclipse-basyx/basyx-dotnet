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
using BaSyx.Utils.Settings;
using CommandLine;
using NLog;
using System;
using System.Linq;
using System.Collections.Generic;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Servers.AdminShell.Http;
using BaSyx.API.ServiceProvider;
using NLog.Web;

namespace BaSyx.AssetAdministrationShellRepository.Server.Http.App
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public class ServerOptions
        {
            [Option('s', "settings", Required = false, HelpText = "Path to the ServerSettings.xml")]
            public string SettingsFilePath { get; set; }

            [Option('u', "urls", Required = false, HelpText = "Hosting Urls (semicolon separated), e.g. http://+:4999")]
            public string Urls { get; set; }
        }

        static void Main(string[] args)
        {
            ServerSettings serverSettings = null;

            //Parse command line arguments based on the options above
            Parser.Default.ParseArguments<ServerOptions>(args)
                   .WithParsed<ServerOptions>(o =>
                   {
                       if (!string.IsNullOrEmpty(o.SettingsFilePath))
                           serverSettings = ServerSettings.LoadSettingsFromFile(o.SettingsFilePath);
                       else
                           serverSettings = ServerSettings.LoadSettings();

                       if(!string.IsNullOrEmpty(o.Urls))
                       {
                           if (o.Urls.Contains(";"))
                           {
                               string[] splittedUrls = o.Urls.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                               serverSettings.ServerConfig.Hosting.Urls = splittedUrls.ToList();
                           }
                           else
                               serverSettings.ServerConfig.Hosting.Urls = new List<string>() { o.Urls };
                       }
                   });

            if(args.Contains("--help") || args.Contains("--version"))
                return;

            //Instantiate blank AssetAdministrationShellRepository-Http-Server with previously loaded server settings
            AssetAdministrationShellRepositoryHttpServer server = new AssetAdministrationShellRepositoryHttpServer(serverSettings);

            //Configure the entire application to use your own logger library (here: Nlog)
            server.WebHostBuilder.UseNLog();

            //Instantiate implementation backend for the Asset Administration Shell Repository
            AssetAdministrationShellRepositoryServiceProvider repositoryService = new AssetAdministrationShellRepositoryServiceProvider();

            //Use auto endpoint repgistration for all subcomponents of the repository service
            repositoryService.UseAutoEndpointRegistration(serverSettings.ServerConfig);

            //Assign implemenation backend to blank AssetAdministrationShellRepository-Http-Server
            server.SetServiceProvider(repositoryService);

            //Add BaSyx Web UI
            server.AddBaSyxUI(PageNames.AssetAdministrationShellRepositoryServer);

            //Add Swagger Documentation and UI
            server.AddSwagger(Interface.AssetAdministrationShellRepository);

            //Run the server
            server.Run();
        }
    }
}
