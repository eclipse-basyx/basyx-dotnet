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
using BaSyx.Models.Connectivity;
using BaSyx.Utils.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BaSyx.Utils.ResultHandling;
using BaSyx.Models.Extensions;
using BaSyx.API.ServiceProvider;
using BaSyx.Utils.Settings;
using BaSyx.Models.AdminShell;
using Endpoint = BaSyx.Models.Connectivity.Endpoint;
using Microsoft.Extensions.DependencyInjection;
using BaSyx.API.Http;

namespace BaSyx.Deployment.AppDataService
{
    public static class AppDataServiceExtensions
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger("AppDataServiceExtensions");

        public static void UseUniversalEndpointRegistration(this IAssetAdministrationShellServiceProvider serviceProvider, ServerConfiguration serverConfiguration, string pathBase = "")
        {
            string _pathBase = string.Empty;
            if (!string.IsNullOrEmpty(pathBase))
                _pathBase = pathBase;
            else if (!string.IsNullOrEmpty(serverConfiguration.PathBase))
                _pathBase = serverConfiguration.PathBase;

            if (AppDataService.IsSnapped)
            {
                bool enableIPv6 = false;
                if(serverConfiguration.Hosting.EnableIPv6.HasValue)
                    enableIPv6 = serverConfiguration.Hosting.EnableIPv6.Value;

                IEnumerable<IPAddress> ips;
				List<IEndpoint> endpoints = new List<IEndpoint>();

				if (AppDataService.IsVirtual)
                {
                    Endpoint virtualExternalEndpoint = new Endpoint($"https://127.0.0.1:8443{_pathBase}{AssetAdministrationShellRoutes.AAS}", InterfaceName.AssetAdministrationShellInterface);
                    endpoints.Add(virtualExternalEndpoint);
                    foreach (var url in serverConfiguration.Hosting.Urls)
                    {
                        string harmonizedUrl = url.Replace("+", "0.0.0.0");
                        Uri harmonizedUri = new Uri(harmonizedUrl);
                        Endpoint virtualInternalEndpoint = new Endpoint($"http://localhost:{harmonizedUri.Port}{_pathBase}{AssetAdministrationShellRoutes.AAS}", InterfaceName.AssetAdministrationShellInterface);
                        virtualInternalEndpoint.ProtocolInformation.Subprotocol = "ipc";
                        endpoints.Add(virtualInternalEndpoint);
                    }
                   
                }
                else
                {                 
                    if (serverConfiguration.Hosting.Urls.FindIndex(u => u.Contains("+")) != -1)
                        ips = NetworkUtils.GetIPAddresses(enableIPv6);
                    else
                        ips = serverConfiguration.Hosting.Urls.ConvertAll(c => IPAddress.Parse(new Uri(c).Host));

                    foreach (var ip in ips)
                    {
                        if (ip == IPAddress.Loopback)
                            continue;

                        Endpoint externalEndpoint = new Endpoint($"https://{ip}{_pathBase}{AssetAdministrationShellRoutes.AAS}", InterfaceName.AssetAdministrationShellInterface);
                        endpoints.Add(externalEndpoint);
                        logger.LogInformation($"Using Url: {externalEndpoint.ProtocolInformation.EndpointAddress}");
                    }

                    foreach (var url in serverConfiguration.Hosting.Urls)
                    {
                        string harmonizedUrl = url.Replace("+", "0.0.0.0");
                        Uri harmonizedUri = new Uri(harmonizedUrl);
                        Endpoint internalEndpoint = new Endpoint($"http://localhost:{harmonizedUri.Port}{_pathBase}{AssetAdministrationShellRoutes.AAS}", InterfaceName.AssetAdministrationShellInterface);
                        internalEndpoint.ProtocolInformation.Subprotocol = "ipc";
                        endpoints.Add(internalEndpoint);
                    }
                }
                serviceProvider.UseSnappedEndpointRegistration(endpoints);
            } 
            else if(!string.IsNullOrEmpty(Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%")) && 
                Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%") != "%WEBSITE_HOSTNAME%")
            {
                string websiteHostName = Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%");
                string websiteUrl = $"https://{websiteHostName}{_pathBase}";
                ServerConfiguration serverConfig = new ServerConfiguration()
                {
                    Hosting = new HostingConfiguration() { Urls = new List<string>() { websiteUrl } }
                };
                serviceProvider.UseAutoEndpointRegistration(serverConfig);
            }
            else
            {
                List<IEndpoint> endpoints = GetMinimalEndpoints(serverConfiguration);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
        }

        public static void UseSnappedEndpointRegistration(this IAssetAdministrationShellServiceProvider serviceProvider, IEnumerable<IEndpoint> endpoints)
        {
            serviceProvider.ServiceDescriptor.SetEndpoints(endpoints);
            foreach (var submodel in serviceProvider.ServiceDescriptor.SubmodelDescriptors)
            {
                List<IEndpoint> submodelEndpoints = new List<IEndpoint>();
                foreach (var endpoint in endpoints)
                {
                    var ep = new Endpoint(DefaultEndpointRegistration.GetSubmodelEndpoint(endpoint, submodel.Id.Id), InterfaceName.SubmodelInterface);
                    ep.ProtocolInformation.Subprotocol = endpoint.ProtocolInformation.Subprotocol;
                    submodelEndpoints.Add(ep);
                }
                submodel.SetEndpoints(submodelEndpoints);
            }
        }

        public static List<IEndpoint> GetMinimalEndpoints(ServerConfiguration serverConfig)
        {
            List<IEndpoint> endpoints = new List<IEndpoint>();
            foreach (var url in serverConfig.Hosting.Urls)
            {
                var endpoint = GetExternalEndpoint(url, serverConfig.PathBase);
                endpoints.AddRange(endpoint);
            }
            return endpoints;
        }

        public static List<IEndpoint> GetAllEndpoints(ServerConfiguration serverConfig)
        {
            List<IEndpoint> endpoints = new List<IEndpoint>();
            foreach (var url in serverConfig.Hosting.Urls)
            {
                bool enableIPv6 = serverConfig.Hosting.EnableIPv6.HasValue ? serverConfig.Hosting.EnableIPv6.Value : false;
                var urlEndpoints = GetEndpoints(url, serverConfig.PathBase, enableIPv6);
                endpoints.AddRange(urlEndpoints);
            }
            return endpoints;
        }

        public static List<IEndpoint> GetEndpoints(string originalUrl, string pathBase = "", bool includeIPv6 = false)
        {
            List<IEndpoint> endpoints = new List<IEndpoint>();
            Uri uri = new Uri(originalUrl.Replace("+", "localhost").Replace("*", "localhost"));
            string _pathBase = string.Empty;
            if (!string.IsNullOrEmpty(pathBase))
            {
                uri = new Uri(uri, pathBase);
                _pathBase = pathBase;
            }
            else if (!string.IsNullOrEmpty(uri.PathAndQuery))
                _pathBase = uri.PathAndQuery;

            if (originalUrl.Contains("+") || originalUrl.Contains("*"))
                endpoints = DefaultEndpointRegistration.GetNetworkInterfaceBasedEndpoints(uri.Scheme, uri.Port, includeIPv6, InterfaceName.AssetAdministrationShellInterface, _pathBase);
            else
                endpoints.Add(new Endpoint(uri, InterfaceName.AssetAdministrationShellInterface));
            return endpoints;
        }

        public static List<IEndpoint> GetExternalEndpoint(string originalUrl, string pathBase = "")
        {
            List<IEndpoint> endpoints = new List<IEndpoint>();
            Uri uri = new Uri(originalUrl.Replace("+", "localhost").Replace("*", "localhost"));
            if (originalUrl.Contains("+") || originalUrl.Contains("*") || originalUrl.Contains("0.0.0.0"))
            {
                string externalIP = NetworkUtils.GetExternalIPAddress().ToString();
                if(!string.IsNullOrEmpty(externalIP))
                {
                    string endpointString = uri.Scheme + "://" + externalIP + ":" + uri.Port + pathBase;
                    Uri endpointUri = new Uri(endpointString);
                    endpoints.Add(new Endpoint(endpointUri, InterfaceName.AssetAdministrationShellInterface));
                }
                else
                    return GetEndpoints(originalUrl, pathBase);           
            }
            else
            {
                endpoints.Add(new Endpoint(new Uri(uri, pathBase), InterfaceName.AssetAdministrationShellInterface));
            }
            return endpoints;            
        }        

        public static IServiceCollection AddSettings<T>(this IServiceCollection services, AppDataService appDataService) where T : Settings
        {
            services.Configure<T>(appDataService.Configuration.GetSection(typeof(T).Name));
            return services;
        }


        public static void AddConfigurationSubmodel(this IAssetAdministrationShellServiceProvider serviceProvider, AppDataService appDataService)
        {
            Submodel configSubmodel = new Submodel("ShellConfiguration", new BaSyxSubmodelIdentifier("ShellConfiguration", "1.0.0"));

            foreach (var settingsEntry in appDataService.AppDataContext.Settings)
            {
                var settingsSmc = settingsEntry.Value.CreateSubmodelElementCollectionFromObject(settingsEntry.Value.Name);
                settingsSmc.Value.Value.Add(
                new Operation("Save")
                {
                    OnMethodCalled = (op, inArgs, inoutArgs, outArgs, ct) =>
                    {
                        settingsEntry.Value.SaveSettings(settingsEntry.Value.FilePath, settingsEntry.Value.GetType());
                        return new OperationResult(true);
                    }
                });
                configSubmodel.SubmodelElements.Add(settingsSmc);
            }  

            configSubmodel.SubmodelElements.Add(
                new Operation("Restart")
                {
                    OnMethodCalled = (op, inArgs, inoutArgs, outArgs, ct) =>
                    {
                        Task.Run(async () =>
                        {
                            await Task.Delay(3000);
                            Environment.ExitCode = 0;
                            Environment.Exit(Environment.ExitCode);
                        });
                        return new OperationResult(true);
                    }
                });

            ISubmodelServiceProvider submodelServiceProvider = configSubmodel.CreateServiceProvider();
            serviceProvider.SubmodelProviderRegistry.RegisterSubmodelServiceProvider(configSubmodel.Id, submodelServiceProvider);            
        }
    }
}
