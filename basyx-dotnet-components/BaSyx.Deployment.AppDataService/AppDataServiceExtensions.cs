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
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using BaSyx.Utils.ResultHandling;
using BaSyx.Models.Extensions;
using BaSyx.API.ServiceProvider;
using BaSyx.Utils.Settings;
using BaSyx.Models.AdminShell;
using Endpoint = BaSyx.Models.Connectivity.Endpoint;

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

                var ips = NetworkUtils.GetIPAddresses(enableIPv6);
                var urls = new List<string>();
                foreach (var ip in ips)
                {
                    string url = $"https://{ip}{_pathBase}";
                    urls.Add(url);
                    logger.LogInformation($"Using Url: {url}");
                }

                if(AppDataService.IsX64)
                    urls.Add($"https://127.0.0.1:8443{_pathBase}");

                ServerConfiguration serverConfig = new ServerConfiguration()
                {
                    Hosting = new HostingConfiguration() { Urls = urls }
                };

                serviceProvider.UseAutoEndpointRegistration(serverConfig);
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
                endpoints = GetNetworkInterfaceBasedEndpoints(uri.Scheme, uri.Port, _pathBase, includeIPv6);
            else
                endpoints.Add(new Endpoint(uri, InterfaceName.AssetAdministrationShellInterface));
            return endpoints;
        }

        public static List<IEndpoint> GetNetworkInterfaceBasedEndpoints(string endpointType, int port, string pathBase, bool includeIPv6)
        {
            IEnumerable<IPAddress> IPAddresses = NetworkUtils.GetIPAddresses(includeIPv6);
            List<IEndpoint> endpoints = new List<IEndpoint>();
            foreach (IPAddress address in IPAddresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    string endpoint = endpointType + "://" + address.ToString() + ":" + port + pathBase;
                    endpoints.Add(new Endpoint(endpoint, InterfaceName.AssetAdministrationShellInterface));
                    logger.LogInformation("Using " + endpoint + " as endpoint");
                }
                else if (includeIPv6 && address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    string endpoint = endpointType + "://[" + address.ToString() + "]:" + port + pathBase;
                    endpoints.Add(new Endpoint(endpoint, InterfaceName.AssetAdministrationShellInterface));
                    logger.LogInformation("Using " + endpoint + " as endpoint");
                }
            }
            return endpoints;
        }

        public static List<IEndpoint> GetExternalEndpoint(string originalUrl, string pathBase = "")
        {
            List<IEndpoint> endpoints = new List<IEndpoint>();
            Uri uri = new Uri(originalUrl.Replace("+", "localhost").Replace("*", "localhost"));
            if (originalUrl.Contains("+") || originalUrl.Contains("*") || originalUrl.Contains("0.0.0.0"))
            {
                string externalIP = GetExternalIPAddress();
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

        public static string GetExternalIPAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                try
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    string localIPAddress = endPoint.Address.ToString();
                    return localIPAddress;
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Unable to get external IP address");
                    return null;
                }                
            }
        }


        public static void AddConfigurationSubmodel(this IAssetAdministrationShellServiceProvider serviceProvider, AppDataService appDataService)
        {
            Submodel configSubmodel = new Submodel("ShellConfiguration", new BaSyxSubmodelIdentifier("ShellConfiguration", "1.0.0"));

            foreach (var settingsEntry in appDataService.AppDataContext.Settings)
            {
                var settingsSmc = settingsEntry.Value.CreateSubmodelElementCollectionFromObject(settingsEntry.Value.Name);
                settingsSmc.Value.Add(
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
