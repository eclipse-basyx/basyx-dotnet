/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.AAS.Server.Http;
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.API.Components;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Models.Connectivity;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Export;
using BaSyx.Registry.Client.Http;
using BaSyx.Utils.Logging;
using BaSyx.Utils.Settings.Types;
using CommandLine;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BaSyx.AASX.Server.Http.App
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private static FileSystemWatcher watcher;
        private static AssetAdministrationShellRepositoryHttpServer repositoryServer;
        private static AssetAdministrationShellRepositoryServiceProvider repositoryService;
        private static List<HttpEndpoint> endpoints;
        private static ServerSettings serverSettings;
        private static RegistryHttpClient registryHttpClient;

        public class Options
        {
            [Option('s', "settings", Required = false, HelpText = "Path to the ServerSettings.xml")]
            public string SettingsFilePath { get; set; }

            [Option('u', "urls", Required = false, HelpText = "Hosting Urls (semicolon separated), e.g. http://+:4999")]
            public string Urls { get; set; }

            [Option('i', "input", Required = false, HelpText = "Path to AASX-File or Folder")]
            public string InputPath { get; set; }
        }

        static void Main(string[] args)
        {
            logger.Info("Starting AASX Http-Server...");

            string[] inputFiles = null;

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (!string.IsNullOrEmpty(o.SettingsFilePath))
                           serverSettings = ServerSettings.LoadSettingsFromFile(o.SettingsFilePath);
                       else
                           serverSettings = ServerSettings.LoadSettings();

                       if (!string.IsNullOrEmpty(o.Urls))
                       {
                           if (o.Urls.Contains(";"))
                           {
                               string[] splittedUrls = o.Urls.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                               serverSettings.ServerConfig.Hosting.Urls = splittedUrls.ToList();
                           }
                           else
                               serverSettings.ServerConfig.Hosting.Urls = new List<string>() { o.Urls };
                       }
                       if (!string.IsNullOrEmpty(o.InputPath))
                       {
                           if (Directory.Exists(o.InputPath))
                           {
                               inputFiles = Directory.GetFiles(o.InputPath, "*.aasx");

                               watcher = new FileSystemWatcher(o.InputPath, "*.aasx");
                               watcher.EnableRaisingEvents = true;
                               watcher.Changed += Watcher_Changed;
                           }
                           else if (System.IO.File.Exists(o.InputPath))
                           {
                               inputFiles = new string[] { o.InputPath };
                           }
                           else
                               throw new FileNotFoundException(o.InputPath);
                       }
                       else
                           throw new ArgumentNullException(o.InputPath);

                   });

            if (args.Contains("--help") || args.Contains("--version"))
                return;

            if (inputFiles == null || inputFiles.Length == 0)
            {
                logger.Error("No AASX-File(s) found --> Application is shutting down...");
                return;
            }
            else
            {
                registryHttpClient = new RegistryHttpClient();
                repositoryServer = new AssetAdministrationShellRepositoryHttpServer(serverSettings);
                repositoryService = new AssetAdministrationShellRepositoryServiceProvider();
                endpoints = serverSettings.ServerConfig.Hosting.Urls.ConvertAll(c =>
                {
                    string address = c.Replace("+", "127.0.0.1");
                    logger.Info("Using " + address + " as base endpoint url");
                    return new HttpEndpoint(address);
                });
                repositoryService.UseDefaultEndpointRegistration(endpoints);
                repositoryServer.SetServiceProvider(repositoryService);

                repositoryServer.AddBaSyxUI(PageNames.AssetAdministrationShellRepositoryServer);
                repositoryServer.AddSwagger(Interface.AssetAdministrationShellRepository);

                for (int i = 0; i < inputFiles.Length; i++)
                {
                    LoadAASX(inputFiles[i]);
                }

                repositoryServer.ApplicationStopping = () =>
                {
                    var providers = repositoryService.GetAssetAdministrationShellServiceProviders().Entity;
                    foreach (var shellProvider in providers)
                    {
                        var result = registryHttpClient
                        .DeleteAssetAdministrationShellRegistration(shellProvider.ServiceDescriptor.Identification.Id);

                        result.LogResult(logger, LogLevel.Info);
                    }
                };

                repositoryServer.Run();
            }
        }

        private static async void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            await Task.Delay(1000);
            LoadAASX(e.FullPath);
        }

        private static void LoadAASX(string aasxFilePath)
        {
            using (BaSyx.Models.Export.AASX aasx = new BaSyx.Models.Export.AASX(aasxFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                AssetAdministrationShellEnvironment_V2_0 environment = aasx.GetEnvironment_V2_0();
                if (environment == null)
                {
                    logger.Error("Asset Administration Shell Environment cannot be obtained from AASX-Package " + aasxFilePath);
                    return;
                }

                logger.Info("AASX-Package successfully loaded");

                if (environment.AssetAdministrationShells.Count != 0)
                {
                    PackagePart thumbnailPart = aasx.GetThumbnailAsPackagePart();
                    AddToAssetAdministrationShellRepository(environment.AssetAdministrationShells, aasx.SupplementaryFiles, thumbnailPart);
                }
                else
                {
                    logger.Error("No Asset Administration Shells found AASX-Package " + aasxFilePath);
                    return;
                }
            }
        }

        private static void AddToAssetAdministrationShellRepository(List<IAssetAdministrationShell> assetAdministrationShells, List<PackagePart> supplementaryFiles, PackagePart thumbnailPart)
        {
            foreach (var shell in assetAdministrationShells)
            {
                var aasServiceProvider = shell.CreateServiceProvider(true);
                var aasServiceEndpoints = endpoints.ConvertAll(e =>
                {
                    return new HttpEndpoint(
                        new Uri(e.Url, 
                        new Uri("/shells/" + HttpUtility.UrlEncode(shell.Identification.Id), UriKind.Relative)));
                });

                aasServiceProvider.UseDefaultEndpointRegistration(aasServiceEndpoints);
                repositoryService.RegisterAssetAdministrationShellServiceProvider(shell.Identification.Id, aasServiceProvider);

                var result = registryHttpClient.CreateOrUpdateAssetAdministrationShellRegistration(
                    shell.Identification.Id, aasServiceProvider.ServiceDescriptor);

                result.LogResult(logger, LogLevel.Info);
            }

            string aasIdName = assetAdministrationShells.First().Identification.Id;
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                aasIdName = aasIdName.Replace(invalidChar, '_');

            foreach (var file in supplementaryFiles)
            {
                using (Stream stream = file.GetStream())
                {
                    Uri fileUri = new Uri(aasIdName + "/" + file.Uri.ToString().TrimStart('/'), UriKind.Relative);
                    logger.Info("Providing content on server: " + fileUri);
                    repositoryServer.ProvideContent(fileUri, stream);
                }
            }
            if (thumbnailPart != null)
                using (Stream thumbnailStream = thumbnailPart.GetStream())
                {
                    Uri thumbnailUri = new Uri(aasIdName + "/" + thumbnailPart.Uri.ToString().TrimStart('/'), UriKind.Relative);
                    repositoryServer.ProvideContent(thumbnailUri, thumbnailStream);
                }
        }
    }
}
