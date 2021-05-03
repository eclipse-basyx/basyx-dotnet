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
using BaSyx.Models.Connectivity;
using BaSyx.Utils.Network;
using BaSyx.Utils.Settings.Sections;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;

namespace BaSyx.API.Components
{
    public static class DefaultEndpointRegistration
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public static void UseAutoEndpointRegistration(this IAssetAdministrationShellRepositoryServiceProvider serviceProvider, ServerConfiguration serverConfiguration)
        {
            string multiUrl = serverConfiguration.Hosting.Urls.Find(u => u.Contains("+"));
            if (!string.IsNullOrEmpty(multiUrl))
            {
                Uri uri = new Uri(multiUrl.Replace("+", "localhost"));
                bool includeIPv6 = false;
                if (serverConfiguration.Hosting.EnableIPv6.HasValue && serverConfiguration.Hosting.EnableIPv6.Value)
                    includeIPv6 = true;

                List<IEndpoint> endpoints = GetNetworkInterfaceBasedEndpoints(uri.Scheme, uri.Port, includeIPv6);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
            else
            {
                List<IEndpoint> endpoints = serverConfiguration.Hosting.Urls.ConvertAll(EndpointConverter);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
        }

        public static void UseAutoEndpointRegistration(this ISubmodelRepositoryServiceProvider serviceProvider, ServerConfiguration serverConfiguration)
        {
            string multiUrl = serverConfiguration.Hosting.Urls.Find(u => u.Contains("+"));
            if (!string.IsNullOrEmpty(multiUrl))
            {
                Uri uri = new Uri(multiUrl.Replace("+", "localhost"));
                bool includeIPv6 = false;
                if (serverConfiguration.Hosting.EnableIPv6.HasValue && serverConfiguration.Hosting.EnableIPv6.Value)
                    includeIPv6 = true;

                List<IEndpoint> endpoints = GetNetworkInterfaceBasedEndpoints(uri.Scheme, uri.Port, includeIPv6);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
            else
            {
                List<IEndpoint> endpoints = serverConfiguration.Hosting.Urls.ConvertAll(EndpointConverter);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
        }

        private static IEndpoint EndpointConverter(string input)
        {
            try
            {
                Uri uri = new Uri(input);
                return EndpointFactory.CreateEndpoint(uri, null);
            }
            catch (Exception e)
            {
                logger.Warn(e, "Error converting input string: " + input + " - Message: " + e.Message);
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

                List<IEndpoint> endpoints = GetNetworkInterfaceBasedEndpoints(uri.Scheme, uri.Port, includeIPv6);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
            else
            {
                List<IEndpoint> endpoints = serverConfiguration.Hosting.Urls.ConvertAll(EndpointConverter);
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

                List<IEndpoint> endpoints = GetNetworkInterfaceBasedEndpoints(uri.Scheme, uri.Port, includeIPv6);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
            else
            {
                List<IEndpoint> endpoints = serverConfiguration.Hosting.Urls.ConvertAll(EndpointConverter);
                serviceProvider.UseDefaultEndpointRegistration(endpoints);
            }
        }

        private static List<IEndpoint> GetNetworkInterfaceBasedEndpoints(string endpointType, int port, bool includeIPv6)
        {
            IEnumerable<IPAddress> ipAddresses = NetworkUtils.GetIPAddresses(includeIPv6);
            List<IEndpoint> aasEndpoints = new List<IEndpoint>();
            foreach (var ipAddress in ipAddresses)
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    string address = endpointType + "://" + ipAddress.ToString() + ":" + port;
                    aasEndpoints.Add(EndpointFactory.CreateEndpoint(endpointType, address, null));
                    logger.Info($"Using {address} as endpoint");
                }
                else if (includeIPv6 && ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    string address = endpointType + "://[" + ipAddress.ToString() + "]:" + port;
                    aasEndpoints.Add(EndpointFactory.CreateEndpoint(endpointType, address, null));
                    logger.Info($"Using {address} as endpoint");
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
                string epAddress = endpoint.Address;
                if (!epAddress.EndsWith("/shells"))
                    epAddress = epAddress + (epAddress.EndsWith("/") ? "" : "/") + "shells";

                repositoryEndpoints.Add(EndpointFactory.CreateEndpoint(endpoint.Type, epAddress, endpoint.Security));
            }

            serviceProvider.ServiceDescriptor.AddEndpoints(repositoryEndpoints);
            var aasRepositoryDescriptor = serviceProvider.ServiceDescriptor;
            foreach (var aasDescriptor in aasRepositoryDescriptor.AssetAdministrationShellDescriptors.Values)
            {
                List<IEndpoint> aasEndpoints = new List<IEndpoint>();
                foreach (var endpoint in repositoryEndpoints)
                {
                    var ep = EndpointFactory.CreateEndpoint(endpoint.Type, GetAssetAdministrationShellEndpoint(endpoint, aasDescriptor.IdShort), endpoint.Security);
                    aasEndpoints.Add(ep);
                }
                aasDescriptor.AddEndpoints(aasEndpoints);

                foreach (var submodelDescriptor in aasDescriptor.SubmodelDescriptors.Values)
                {
                    List<IEndpoint> submodelEndpoints = new List<IEndpoint>();
                    foreach (var endpoint in aasEndpoints)
                    {
                        var ep = EndpointFactory.CreateEndpoint(endpoint.Type, GetSubmodelEndpoint(endpoint, submodelDescriptor.IdShort), endpoint.Security);
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
                string epAddress = endpoint.Address;
                if (!epAddress.EndsWith("/submodels"))
                    epAddress = epAddress + (epAddress.EndsWith("/") ? "" : "/") + "submodels";

                repositoryEndpoints.Add(EndpointFactory.CreateEndpoint(endpoint.Type, epAddress, endpoint.Security));
            }

            serviceProvider.ServiceDescriptor.AddEndpoints(repositoryEndpoints);
            var submodelRepositoryDescriptor = serviceProvider.ServiceDescriptor;
            foreach (var submodelDescriptor in submodelRepositoryDescriptor.SubmodelDescriptors.Values)
            {
                List<IEndpoint> submodelEndpoints = new List<IEndpoint>();
                foreach (var endpoint in repositoryEndpoints)
                {
                    var ep = EndpointFactory.CreateEndpoint(endpoint.Type, GetSubmodelInRepositoryEndpoint(endpoint, submodelDescriptor.IdShort), endpoint.Security);
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
                string epAddress = endpoint.Address;
                if (!epAddress.EndsWith("/aas"))
                    epAddress = epAddress + (epAddress.EndsWith("/") ? "" : "/") + "aas";

                aasEndpoints.Add(EndpointFactory.CreateEndpoint(endpoint.Type, epAddress, endpoint.Security));
            }

            serviceProvider.ServiceDescriptor.AddEndpoints(aasEndpoints);
            var aasDescriptor = serviceProvider.ServiceDescriptor;
            foreach (var submodel in aasDescriptor.SubmodelDescriptors.Values)
            {
                List<IEndpoint> spEndpoints = new List<IEndpoint>();
                foreach (var endpoint in aasEndpoints)
                {
                    var ep = EndpointFactory.CreateEndpoint(endpoint.Type, GetSubmodelEndpoint(endpoint, submodel.IdShort), endpoint.Security);
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
                string epAddress = endpoint.Address;
                if (!epAddress.EndsWith("/submodel"))
                    epAddress = epAddress + (epAddress.EndsWith("/") ? "" : "/") + "submodel";

                submodelEndpoints.Add(EndpointFactory.CreateEndpoint(endpoint.Type, epAddress, endpoint.Security));
            }

            serviceProvider.ServiceDescriptor.AddEndpoints(submodelEndpoints);         
        }

        public static string GetSubmodelInRepositoryEndpoint(IEndpoint endpoint, string submodelId)
        {
            string epAddress = endpoint.Address;
            if (!epAddress.EndsWith("/submodels"))
                epAddress = epAddress + (epAddress.EndsWith("/") ? "" : "/") + "submodels";

            return epAddress + "/" + submodelId + "/submodel";
        }

        public static string GetSubmodelEndpoint(IEndpoint endpoint, string submodelId)
        {
            string epAddress = endpoint.Address;
            if (!epAddress.EndsWith("/aas"))
                epAddress = epAddress + (epAddress.EndsWith("/") ? "" : "/") + "aas";

            return epAddress + "/submodels/" + submodelId + "/submodel";
        }

        public static string GetAssetAdministrationShellEndpoint(IEndpoint endpoint, string aasId)
        {
            string epAddress = endpoint.Address;
            if (!epAddress.EndsWith("/shells"))
                epAddress = epAddress + (epAddress.EndsWith("/") ? "" : "/") + "shells";

            return epAddress + "/" + aasId + "/aas";
        }
    }
}
