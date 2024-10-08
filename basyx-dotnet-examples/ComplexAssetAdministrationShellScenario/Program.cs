﻿/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
using BaSyx.Models.Connectivity;
using BaSyx.Models.AdminShell;
using BaSyx.Registry.Client.Http;
using BaSyx.Registry.ReferenceImpl.FileBased;
using BaSyx.Registry.Server.Http;
using BaSyx.Utils.Settings;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaSyx.Deployment.AppDataService;
using BaSyx.Registry.ReferenceImpl.InMemory;

namespace ComplexAssetAdministrationShellScenario
{
    class Program
    {
        static RegistryHttpClient registryClient;
        private static AppDataService AppDataService { get; set; }
        static async Task Main(string[] args)
        {
            await Task.Delay(5000);
            AppDataService = AppDataService.Create("adminshell", "appsettings.json", args);
            RegistryClientSettings registryClientSettings = AppDataService.GetSettings<RegistryClientSettings>();
            registryClient = new RegistryHttpClient(registryClientSettings);
            LoadScenario();

            Console.WriteLine("Press enter to quit...");
            Console.ReadKey();
        }

        private static void LoadScenario()
        {
            LoadRegistry();
            LoadSingleShell();
            LoadMultipleShells();
            LoadMultipleSubmodels();
        }

        public static void LoadMultipleSubmodels()
        {
            ServerSettings submodelRepositorySettings = ServerSettings.CreateSettings();
            submodelRepositorySettings.ServerConfig.Hosting.ContentPath = "Content";
            submodelRepositorySettings.ServerConfig.Hosting.Urls.Add("http://+:6999");
            submodelRepositorySettings.ServerConfig.Hosting.Urls.Add("https://+:6499");

            SubmodelRepositoryHttpServer multiServer = new SubmodelRepositoryHttpServer(submodelRepositorySettings);
            multiServer.WebHostBuilder.UseNLog();
            SubmodelRepositoryServiceProvider repositoryService = new SubmodelRepositoryServiceProvider();

            for (int i = 0; i < 3; i++)
            {
                Submodel submodel = new Submodel("MultiSubmodel_" + i, new BaSyxSubmodelIdentifier("MultiSubmodel_" + i, "1.0.0"))
                {
                    Description = new LangStringSet()
                    {
                       new LangString("de", i + ". Teilmodell"),
                       new LangString("en", i + ". Submodel")
                    },
                    Administration = new AdministrativeInformation()
                    {
                        Version = "1.0",
                        Revision = "120"
                    },
                    SubmodelElements = new ElementContainer<ISubmodelElement>()
                    {
                        new Property<string>("Property_" + i, "TestValue_" + i),
                        new SubmodelElementCollection("Coll_" + i)
                        {
                            Value =
                            {
                                Value =
                                {
                                    new Property<string>("SubProperty_" + i, "TestSubValue_" + i)
                                }
                            }                           
                        }
                    }
                };

                var submodelServiceProvider = submodel.CreateServiceProvider();
                repositoryService.RegisterSubmodelServiceProvider(submodel.Id, submodelServiceProvider);
            }

            repositoryService.UseAutoEndpointRegistration(multiServer.Settings.ServerConfig);

            multiServer.SetServiceProvider(repositoryService);
            multiServer.ApplicationStopping = () =>
            {
                for (int i = 0; i < repositoryService.ServiceDescriptor.SubmodelDescriptors.Count(); i++)
                {
                    registryClient.DeleteSubmodelRegistration(new BaSyxShellIdentifier("MultiAAS_" + i, "1.0.0").ToIdentifier().Id, repositoryService.ServiceDescriptor.SubmodelDescriptors.ElementAt(i).Id.Id);
                }
            };

            multiServer.AddBaSyxUI(PageNames.SubmodelRepositoryServer);
            multiServer.AddSwagger(Interface.SubmodelRepository);

            _ = multiServer.RunAsync();

            var shells = registryClient.RetrieveAllAssetAdministrationShellRegistrations(p => p.Id.Id.Contains("SimpleAAS"));
            var shell = shells.Entity?.Result?.FirstOrDefault();
            for (int i = 0; i < repositoryService.ServiceDescriptor.SubmodelDescriptors.Count(); i++)
            {
                var descriptor = repositoryService.ServiceDescriptor.SubmodelDescriptors.ElementAt(i);
                registryClient.CreateSubmodelRegistration(new BaSyxShellIdentifier("MultiAAS_" + i, "1.0.0").ToIdentifier().Id, descriptor);

                if(shell != null)
                    registryClient.CreateSubmodelRegistration(shell.Id.Id, descriptor);
            }
        }


        private static void LoadMultipleShells()
        {
            ServerSettings aasRepositorySettings = ServerSettings.CreateSettings();
            aasRepositorySettings.ServerConfig.Hosting.ContentPath = "Content";
            aasRepositorySettings.ServerConfig.Hosting.Urls.Add("http://+:5999");
            aasRepositorySettings.ServerConfig.Hosting.Urls.Add("https://+:5499");

            AssetAdministrationShellRepositoryHttpServer multiServer = new AssetAdministrationShellRepositoryHttpServer(aasRepositorySettings);
            multiServer.WebHostBuilder.UseNLog();
            AssetAdministrationShellRepositoryServiceProvider repositoryService = new AssetAdministrationShellRepositoryServiceProvider();

            for (int i = 0; i < 3; i++)
            {
                AssetAdministrationShell aas = new AssetAdministrationShell("MultiAAS_" + i, new BaSyxShellIdentifier("MultiAAS_" + i, "1.0.0"))
                {
                    Description = new LangStringSet()
                    {
                       new LangString("de", i + ". VWS"),
                       new LangString("en", i + ". AAS")
                    },
                    Administration = new AdministrativeInformation()
                    {
                        Version = "1.0",
                        Revision = "120"
                    },
                    AssetInformation = new AssetInformation()
                    {
                        AssetKind = AssetKind.Instance,
                        GlobalAssetId = new BaSyxAssetIdentifier("Asset_" + i, "1.0.0")
                    }
                };

                aas.Submodels.Create(new Submodel("TestSubmodel", new BaSyxSubmodelIdentifier("TestSubmodel", "1.0.0"))
                {
                    SubmodelElements =
                    {
                        new Property<string>("Property_" + i, "TestValue_" + i ),
                        new SubmodelElementCollection("Coll_" + i)
                        {
                            Value =
                            {
                                Value =
                                {
                                    new Property<string>("SubProperty_" + i, "TestSubValue_" + i)
                                }
                            }
                        }
                    }
                });

                var aasServiceProvider = aas.CreateServiceProvider(true);
                repositoryService.RegisterAssetAdministrationShellServiceProvider(new BaSyxShellIdentifier("MultiAAS_" + i, "1.0.0").ToIdentifier().Id, aasServiceProvider);
            }

            repositoryService.UseAutoEndpointRegistration(multiServer.Settings.ServerConfig);

            multiServer.SetServiceProvider(repositoryService);

            multiServer.ApplicationStarted = () =>
            {
                foreach (var aasDescriptor in repositoryService.ServiceDescriptor.AssetAdministrationShellDescriptors)
                {
                    registryClient.CreateAssetAdministrationShellRegistration(aasDescriptor);
                }
            };

            multiServer.ApplicationStopping = () =>
            {
                foreach (var aasDescriptor in repositoryService.ServiceDescriptor.AssetAdministrationShellDescriptors)
                {
                    registryClient.DeleteAssetAdministrationShellRegistration(aasDescriptor.Id.Id);
                }
            };

            multiServer.AddBaSyxUI(PageNames.AssetAdministrationShellRepositoryServer);
            multiServer.AddSwagger(Interface.AssetAdministrationShellRepository);

            _ = multiServer.RunAsync();       
        }

        private static void LoadSingleShell()
        {
            AssetAdministrationShell aas = SimpleAssetAdministrationShell.SimpleAssetAdministrationShell.GetAssetAdministrationShell();
            ISubmodel testSubmodel = aas.Submodels["TestSubmodel"];

            ServerSettings submodelServerSettings = ServerSettings.CreateSettings();
            submodelServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("http://localhost:5222");
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("https://localhost:5422");

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
            aasServerSettings.ServerConfig.Hosting.Urls.Add("http://localhost:5111");
            aasServerSettings.ServerConfig.Hosting.Urls.Add("https://localhost:5411");

            IAssetAdministrationShellServiceProvider aasServiceProvider = aas.CreateServiceProvider(true);
            aasServiceProvider.SubmodelProviderRegistry.RegisterSubmodelServiceProvider(testSubmodel.Id, submodelServiceProvider);
            aasServiceProvider.UseAutoEndpointRegistration(aasServerSettings.ServerConfig);

            AssetAdministrationShellHttpServer aasServer = new AssetAdministrationShellHttpServer(aasServerSettings);
            aasServer.WebHostBuilder.UseNLog();
            aasServer.SetServiceProvider(aasServiceProvider);

            aasServer.ApplicationStarted = () =>
            {
                registryClient.CreateAssetAdministrationShellRegistration(new AssetAdministrationShellDescriptor(aas, aasServiceProvider.ServiceDescriptor.Endpoints));
                registryClient.CreateSubmodelRegistration(aas.Id, new SubmodelDescriptor(testSubmodel, submodelServiceProvider.ServiceDescriptor.Endpoints));
            };

            aasServer.ApplicationStopping = () => 
            { 
                registryClient.DeleteAssetAdministrationShellRegistration(aas.Id); 
            };

            aasServer.AddBaSyxUI(PageNames.AssetAdministrationShellServer);
            aasServer.AddSwagger(Interface.AssetAdministrationShell);
            _ = aasServer.RunAsync();
        }

        private static void LoadRegistry()
        {
            ServerSettings registrySettings = ServerSettings.CreateSettings();
            registrySettings.ServerConfig.Hosting = new HostingConfiguration()
            {
                Urls = new List<string>()
                {
                    "http://localhost:3999",
                    "https://localhost:3399"
                },
                Environment = "Development",
                ContentPath = "Content"
            };

            RegistryHttpServer registryServer = new RegistryHttpServer(registrySettings);
            registryServer.WebHostBuilder.UseNLog();
            //FileBasedRegistry fileBasedRegistry = new FileBasedRegistry();
            InMemoryRegistry inMemoryRegistry = new InMemoryRegistry();
            registryServer.SetRegistryProvider(inMemoryRegistry);
            registryServer.AddBaSyxUI(PageNames.AssetAdministrationShellRegistryServer);
            registryServer.AddSwagger(Interface.AssetAdministrationShellRegistry);
            _ = registryServer.RunAsync();
        }
    }
}
