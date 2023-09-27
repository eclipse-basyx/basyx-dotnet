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
using BaSyx.API.Http;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.Extensions;
using BaSyx.Utils.Network;
using BaSyx.Utils.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace BaSyx.API.ServiceProvider
{
    public static class DefaultEndpointRegistration
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger("DefaultEndpointRegistration");
        public static void UseAutoEndpointRegistration(this IAssetAdministrationShellRepositoryServiceProvider serviceProvider, ServerConfiguration serverConfiguration)
        {
            string multiUrl = serverConfiguration.Hosting.Urls.Find(u => u.Contains("+"));
            if (!string.IsNullOrEmpty(multiUrl))
            {
                Uri uri = new Uri(multiUrl.Replace("+", "0.0.0.0"));
                bool includeIPv6 = false;
                if (serverConfiguration.Hosting.EnableIPv6.HasValue && serverConfiguration.Hosting.EnableIPv6.Value)
                    includeIPv6 = true;

                List<IEndpoint> endpoints = GetNetworkInterfaceBasedEndpoints(uri.Scheme, uri.Port, includeIPv6, InterfaceName.AssetAdministrationShellRepositoryInterface);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
            else
            {
                List<IEndpoint> endpoints = ConvertEndpoints(serverConfiguration.Hosting.Urls, InterfaceName.AssetAdministrationShellRepositoryInterface);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
        }

        public static void UseAutoEndpointRegistration(this ISubmodelRepositoryServiceProvider serviceProvider, ServerConfiguration serverConfiguration)
        {
            string multiUrl = serverConfiguration.Hosting.Urls.Find(u => u.Contains("+"));
            if (!string.IsNullOrEmpty(multiUrl))
            {
                Uri uri = new Uri(multiUrl.Replace("+", "0.0.0.0"));
                bool includeIPv6 = false;
                if (serverConfiguration.Hosting.EnableIPv6.HasValue && serverConfiguration.Hosting.EnableIPv6.Value)
                    includeIPv6 = true;

                List<IEndpoint> endpoints = GetNetworkInterfaceBasedEndpoints(uri.Scheme, uri.Port, includeIPv6, InterfaceName.SubmodelRepositoryInterface);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
            else
            {
                List<IEndpoint> endpoints = ConvertEndpoints(serverConfiguration.Hosting.Urls, InterfaceName.SubmodelRepositoryInterface);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
        }

        private static List<IEndpoint> ConvertEndpoints(List<string> urls, InterfaceName interfaceName)
        {
            try
            {
                List<IEndpoint> endpoints = new List<IEndpoint>();
                foreach (var url in urls)
                {
                    endpoints.Add(new Endpoint(url, interfaceName));
                }
                return endpoints;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error converting urls");
                return null;
            }
        }

        public static void UseAutoEndpointRegistration(this IAssetAdministrationShellServiceProvider serviceProvider, ServerConfiguration serverConfiguration)
        {
            string multiUrl = serverConfiguration.Hosting.Urls.Find(u => u.Contains("+"));
            if (!string.IsNullOrEmpty(multiUrl))
            {
                Uri uri = new Uri(multiUrl.Replace("+", "localhost"));
                bool includeIPv6 = false;
                if (serverConfiguration.Hosting.EnableIPv6.HasValue && serverConfiguration.Hosting.EnableIPv6.Value)
                    includeIPv6 = true;

                List<IEndpoint> endpoints = GetNetworkInterfaceBasedEndpoints(uri.Scheme, uri.Port, includeIPv6, InterfaceName.AssetAdministrationShellInterface);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
            else
            {
                List<IEndpoint> endpoints = ConvertEndpoints(serverConfiguration.Hosting.Urls, InterfaceName.AssetAdministrationShellInterface);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
        }

        public static void UseAutoEndpointRegistration(this ISubmodelServiceProvider serviceProvider, ServerConfiguration serverConfiguration)
        {
            string multiUrl = serverConfiguration.Hosting.Urls.Find(u => u.Contains("+"));
            if (!string.IsNullOrEmpty(multiUrl))
            {
                Uri uri = new Uri(multiUrl.Replace("+", "localhost"));
                bool includeIPv6 = false;
                if (serverConfiguration.Hosting.EnableIPv6.HasValue && serverConfiguration.Hosting.EnableIPv6.Value)
                    includeIPv6 = true;

                List<IEndpoint> endpoints = GetNetworkInterfaceBasedEndpoints(uri.Scheme, uri.Port, includeIPv6, InterfaceName.SubmodelInterface);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
            else
            {
                List<IEndpoint> endpoints = ConvertEndpoints(serverConfiguration.Hosting.Urls, InterfaceName.SubmodelInterface);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
        }

        private static List<IEndpoint> GetNetworkInterfaceBasedEndpoints(string endpointType, int port, bool includeIPv6, InterfaceName interfaceName)
        {
            IEnumerable<IPAddress> ipAddresses = NetworkUtils.GetIPAddresses(includeIPv6);
            List<IEndpoint> aasEndpoints = new List<IEndpoint>();
            foreach (var ipAddress in ipAddresses)
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    string address = endpointType + "://" + ipAddress.ToString() + ":" + port;
                    aasEndpoints.Add(new Endpoint(address, interfaceName));
                    logger.LogInformation($"Using {address} as endpoint");
                }
                else if (includeIPv6 && ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    string address = endpointType + "://[" + ipAddress.ToString() + "]:" + port;
                    aasEndpoints.Add(new Endpoint(address, interfaceName));
                    logger.LogInformation($"Using {address} as endpoint");
                }
                else
                    continue;
            }
            return aasEndpoints;
        }

        public static void UseDefaultEndpointRegistration(this IAssetAdministrationShellRepositoryServiceProvider serviceProvider, IEnumerable<IEndpoint> endpoints)
        {
            List<IEndpoint> repositoryEndpoints = new List<IEndpoint>();
            foreach (var endpoint in endpoints)
            {
                string epAddress = endpoint.ProtocolInformation.EndpointAddress;
                if (!epAddress.EndsWith(AssetAdministrationShellRepositoryRoutes.SHELLS))
                    epAddress = epAddress.TrimEnd('/') + AssetAdministrationShellRepositoryRoutes.SHELLS;

                repositoryEndpoints.Add(new Endpoint(epAddress, InterfaceName.AssetAdministrationShellRepositoryInterface));
            }

            serviceProvider.ServiceDescriptor.AddEndpoints(repositoryEndpoints);
            var aasRepositoryDescriptor = serviceProvider.ServiceDescriptor;
            foreach (var aasDescriptor in aasRepositoryDescriptor.AssetAdministrationShellDescriptors)
            {
                List<IEndpoint> aasEndpoints = new List<IEndpoint>();
                foreach (var endpoint in repositoryEndpoints)
                {
                    var ep = new Endpoint(GetAssetAdministrationShellEndpoint(endpoint, aasDescriptor.Identification.Id), InterfaceName.AssetAdministrationShellRepositoryInterface);
                    aasEndpoints.Add(ep);
                }
                aasDescriptor.AddEndpoints(aasEndpoints);

                foreach (var submodelDescriptor in aasDescriptor.SubmodelDescriptors)
                {
                    List<IEndpoint> submodelEndpoints = new List<IEndpoint>();
                    foreach (var endpoint in aasEndpoints)
                    {
                        var ep = new Endpoint(GetSubmodelEndpoint(endpoint, submodelDescriptor.Identification.Id), InterfaceName.AssetAdministrationShellRepositoryInterface);
                        submodelEndpoints.Add(ep);
                    }
                    submodelDescriptor.AddEndpoints(submodelEndpoints);
                }
            }
        }

        public static void UseDefaultEndpointRegistration(this ISubmodelRepositoryServiceProvider serviceProvider, IEnumerable<IEndpoint> endpoints)
        {
            List<IEndpoint> repositoryEndpoints = new List<IEndpoint>();
            foreach (var endpoint in endpoints)
            {
                string epAddress = endpoint.ProtocolInformation.EndpointAddress;
                if (!epAddress.EndsWith(SubmodelRepositoryRoutes.SUBMODELS))
                    epAddress = epAddress.TrimEnd('/') + SubmodelRepositoryRoutes.SUBMODELS;

                repositoryEndpoints.Add(new Endpoint(epAddress, InterfaceName.SubmodelRepositoryInterface));
            }

            serviceProvider.ServiceDescriptor.AddEndpoints(repositoryEndpoints);
            var submodelRepositoryDescriptor = serviceProvider.ServiceDescriptor;
            foreach (var submodelDescriptor in submodelRepositoryDescriptor.SubmodelDescriptors)
            {
                List<IEndpoint> submodelEndpoints = new List<IEndpoint>();
                foreach (var endpoint in repositoryEndpoints)
                {
                    var ep = new Endpoint(GetSubmodelInRepositoryEndpoint(endpoint, submodelDescriptor.Identification.Id), InterfaceName.SubmodelRepositoryInterface);
                    submodelEndpoints.Add(ep);
                }
                submodelDescriptor.AddEndpoints(submodelEndpoints);                
            }
        }

        public static void UseDefaultEndpointRegistration(this IAssetAdministrationShellServiceProvider serviceProvider, IEnumerable<IEndpoint> endpoints)
        {
            List<IEndpoint> aasEndpoints = new List<IEndpoint>();
            foreach (var endpoint in endpoints)
            {
                string epAddress = endpoint.ProtocolInformation.EndpointAddress;
                if (!epAddress.EndsWith(AssetAdministrationShellRoutes.AAS))
                    epAddress = epAddress.TrimEnd('/') + AssetAdministrationShellRoutes.AAS;

                aasEndpoints.Add(new Endpoint(epAddress, InterfaceName.AssetAdministrationShellInterface));
            }

            serviceProvider.ServiceDescriptor.AddEndpoints(aasEndpoints);
            var aasDescriptor = serviceProvider.ServiceDescriptor;
            foreach (var submodel in aasDescriptor.SubmodelDescriptors)
            {
                List<IEndpoint> spEndpoints = new List<IEndpoint>();
                foreach (var endpoint in aasEndpoints)
                {
                    var ep = new Endpoint(GetSubmodelEndpoint(endpoint, submodel.Identification.Id), InterfaceName.AssetAdministrationShellInterface);
                    spEndpoints.Add(ep);
                }
                submodel.AddEndpoints(spEndpoints);
            }
        }

        public static void UseDefaultEndpointRegistration(this ISubmodelServiceProvider serviceProvider, IEnumerable<IEndpoint> endpoints)
        {
            List<IEndpoint> submodelEndpoints = new List<IEndpoint>();
            foreach (var endpoint in endpoints)
            {
                string epAddress = endpoint.ProtocolInformation.EndpointAddress;
                if (!epAddress.EndsWith(SubmodelRoutes.SUBMODEL))
                    epAddress = epAddress.TrimEnd('/') + SubmodelRoutes.SUBMODEL;

                submodelEndpoints.Add(new Endpoint(epAddress, InterfaceName.SubmodelInterface));
            }

            serviceProvider.ServiceDescriptor.AddEndpoints(submodelEndpoints);         
        }

        public static string GetSubmodelInRepositoryEndpoint(IEndpoint endpoint, string submodelId)
        {
            string epAddress = endpoint.ProtocolInformation.EndpointAddress;
            if (!epAddress.EndsWith(SubmodelRepositoryRoutes.SUBMODELS))
                epAddress = epAddress.TrimEnd('/') + SubmodelRepositoryRoutes.SUBMODELS;

            submodelId = StringOperations.Base64UrlEncode(submodelId);

            return epAddress + "/" + submodelId + SubmodelRoutes.SUBMODEL;
        }

        public static string GetSubmodelEndpoint(IEndpoint endpoint, string submodelId)
        {
            string epAddress = endpoint.ProtocolInformation.EndpointAddress;
            if (!epAddress.EndsWith(AssetAdministrationShellRoutes.AAS))
                epAddress = epAddress.TrimEnd('/') + AssetAdministrationShellRoutes.AAS;

            submodelId = StringOperations.Base64UrlEncode(submodelId);

            return epAddress + SubmodelRepositoryRoutes.SUBMODELS + "/" + submodelId + SubmodelRoutes.SUBMODEL;
        }

        public static string GetAssetAdministrationShellEndpoint(IEndpoint endpoint, string aasId)
        {
            string epAddress = endpoint.ProtocolInformation.EndpointAddress;
            if (!epAddress.EndsWith(AssetAdministrationShellRepositoryRoutes.SHELLS))
                epAddress = epAddress.TrimEnd('/') + AssetAdministrationShellRepositoryRoutes.SHELLS;

            aasId = StringOperations.Base64UrlEncode(aasId);

            return epAddress + "/" + aasId + AssetAdministrationShellRoutes.AAS;
        }
    }
}
