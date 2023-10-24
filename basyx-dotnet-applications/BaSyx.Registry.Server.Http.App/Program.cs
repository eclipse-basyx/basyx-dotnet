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
using BaSyx.Registry.ReferenceImpl.FileBased;
using BaSyx.Discovery.mDNS;
using BaSyx.Utils.Settings;
using CommandLine;
using NLog;
using System;
using System.Linq;
using System.Collections.Generic;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using NLog.Web;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace BaSyx.Registry.Server.Http.App
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

            //Instantiate blank Registry-Http-Server with previously loaded server settings
            RegistryHttpServer server = new RegistryHttpServer(serverSettings);

            //Check if ServerCertificate is present
            if(!string.IsNullOrEmpty(serverSettings.ServerConfig.Security.ServerCertificatePath))
            {
                server.WebHostBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.ConfigureHttpsDefaults(listenOptions =>
                    {
                        X509Certificate2 certificate = new X509Certificate2(
                            serverSettings.ServerConfig.Security.ServerCertificatePath,
                            serverSettings.ServerConfig.Security.ServerCertificatePassword);
                        listenOptions.ServerCertificate = certificate;
                    });
                });
            }

            //Configure the entire application to use your own logger library (here: Nlog)
            server.WebHostBuilder.UseNLog();

            //Instantiate implementation backend for the Registry
            FileBasedRegistry fileBasedRegistry = new FileBasedRegistry();                       

            //Assign implemenation backend to blank Registry-Http-Server
            server.SetRegistryProvider(fileBasedRegistry);

            //Start mDNS Discovery ability when the server successfully booted up
            server.ApplicationStarted = () =>
            {
                if (serverSettings.Miscellaneous.ContainsKey("DiscoveryActivated") && serverSettings.Miscellaneous["DiscoveryActivated"] == "true")
                {
                    fileBasedRegistry.StartDiscovery();
                }                
            };

            //Start mDNS Discovery when the server is shutting down
            server.ApplicationStopping = () =>
            {
                if (serverSettings.Miscellaneous.ContainsKey("DiscoveryActivated") && serverSettings.Miscellaneous["DiscoveryActivated"] == "true")
                {
                    fileBasedRegistry.StopDiscovery();
                }                
            };

            //Add BaSyx Web UI
            server.AddBaSyxUI(PageNames.AssetAdministrationShellRegistryServer);

            //Add Swagger Documentation and UI
            server.AddSwagger(Interface.AssetAdministrationShellRegistry);

            //Run the server
            server.Run();
        }
    }
}
