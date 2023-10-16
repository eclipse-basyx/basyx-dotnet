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
using BaSyx.Clients.AdminShell.Http;
using BaSyx.API.ServiceProvider;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BaSyx.Utils.Client.Http;
using Microsoft.Extensions.Logging;
using BaSyx.API.Interfaces;

namespace BaSyx.Discovery.mDNS
{
    public static class DiscoveryExtensions
    {
        private static DiscoveryServer discoveryServer;
        private static DiscoveryClient discoveryClient;
        private static IAssetAdministrationShellRegistryInterface assetAdministrationShellRegistry;

        public const string ASSETADMINISTRATIONSHELL_ID = "aas.id";
        public const string ASSETADMINISTRATIONSHELL_IDSHORT = "aas.idShort";
        public const string ASSETADMINISTRATIONSHELL_ENDPOINT = "aas.endpoint";
        public const string KEY_VALUE_SEPERATOR = "=";

        private static readonly ILogger logger = LoggingExtentions.CreateLogger("DiscoveryExtensions");

        public static void StartDiscovery(this IAssetAdministrationShellRegistryInterface registry)
        {
            assetAdministrationShellRegistry = registry;

            discoveryServer = new DiscoveryServer(ServiceTypes.AAS_SERVICE_TYPE);
            discoveryServer.ServiceInstanceDiscovered += DiscoveryServer_ServiceInstanceDiscovered;
            discoveryServer.ServiceInstanceShutdown += DiscoveryServer_ServiceInstanceShutdown;
            discoveryServer.Start();
        }

        private static async void DiscoveryServer_ServiceInstanceDiscovered(object sender, ServiceInstanceEventArgs e)
        {
            try
            {
                IAssetAdministrationShellDescriptor aasDescriptor = null;
                if (e?.Servers?.Count > 0)
                {
                    foreach (var server in e.Servers)
                    {
                        bool pingable = await BaSyx.Utils.Network.NetworkUtils.PingHostAsync(server.Address.ToString()).ConfigureAwait(false);
                        if (pingable)
                        {
                            string uri = string.Empty;
                            if (server.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                uri = "http://" + server.Address.ToString() + ":" + server.Port + "/aas";
                            else if (server.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                                uri = "http://[" + server.Address.ToString() + "]:" + server.Port + "/aas";
                            else
                                continue;
                            
                            AssemblyDescriptor(ref aasDescriptor, new Uri(uri));
                        }
                    }
                }
                else
                {
                    logger.LogWarning("Informations about the server are not available. Trying endpoints from TXT-record...");
                    if (e?.TxtRecords?.Count > 0)
                    {
                        foreach (var txtRecord in e.TxtRecords)
                        {
                            if (txtRecord.StartsWith(ASSETADMINISTRATIONSHELL_ENDPOINT))
                            {
                                string[] splittedEndpoint = txtRecord.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                                if (splittedEndpoint.Length == 2 && splittedEndpoint[0].ToLower().Contains("http"))
                                {
                                    AssemblyDescriptor(ref aasDescriptor, new Uri(splittedEndpoint[1]));
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("Got no discovery information even from TXT-records. I'm afraid.");
                    }
                }
                if (aasDescriptor != null)
                {
                    var registeredResult = assetAdministrationShellRegistry.UpdateAssetAdministrationShellRegistration(aasDescriptor.Id.Id, aasDescriptor);
                    if (registeredResult.Success)
                        logger.LogInformation($"Successfully registered Asset Administration Shell with {aasDescriptor.Id.Id} at registry");
                    else
                        registeredResult.LogResult(logger, LogLevel.Error, $"Could not register Asset Administration Shell with {aasDescriptor.Id.Id} at registry");
                }
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Error accessing discovered service instance");
            }
        }

        private static void AssemblyDescriptor(ref IAssetAdministrationShellDescriptor aasDescriptor, Uri aasEndpoint)
        {
            IResult<IAssetAdministrationShellDescriptor> retrieveDescriptor;
            using (var client = new AssetAdministrationShellHttpClient(aasEndpoint, SimpleHttpClient.DEFAULT_HTTP_CLIENT_HANDLER))
                retrieveDescriptor = client.RetrieveAssetAdministrationShellDescriptor();

            if (retrieveDescriptor.Success && retrieveDescriptor.Entity != null)
            {
                logger.LogInformation($"Successfully retrieved AssetAdministrationShellDescriptor with id {retrieveDescriptor.Entity.Id.Id}");
                if (aasDescriptor == null)
                {
                    aasDescriptor = retrieveDescriptor.Entity;
                    aasDescriptor.SetEndpoints(new List<IEndpoint>() { new Endpoint(aasEndpoint, InterfaceName.AssetAdministrationShellInterface) });

                    foreach (var submodelDescriptor in retrieveDescriptor.Entity.SubmodelDescriptors)
                    {
                        List<IEndpoint> submodelEndpoints = new List<IEndpoint>();
                        foreach (var submodelEndpoint in submodelDescriptor.Endpoints)
                        {
                            if (submodelEndpoint.ProtocolInformation.EndpointAddress.Contains(aasEndpoint.Host))
                            {
                                submodelEndpoints.Add(submodelEndpoint);
                            }
                        }
                        aasDescriptor.SubmodelDescriptors.First(s => s.IdShort == submodelDescriptor.IdShort).SetEndpoints(submodelEndpoints);
                    }
                }
                else
                {
                    aasDescriptor.AddEndpoints(new List<IEndpoint>() { new Endpoint(aasEndpoint, InterfaceName.AssetAdministrationShellInterface) });

                    foreach (var submodelDescriptor in retrieveDescriptor.Entity.SubmodelDescriptors)
                    {
                        List<IEndpoint> submodelEndpoints = new List<IEndpoint>();
                        foreach (var submodelEndpoint in submodelDescriptor.Endpoints)
                        {
                            if (submodelEndpoint.ProtocolInformation.EndpointAddress.Contains(aasEndpoint.Host))
                            {
                                if (aasDescriptor.SubmodelDescriptors.First(s => s.IdShort == submodelDescriptor.IdShort).Endpoints.FirstOrDefault(f => f.ProtocolInformation.EndpointAddress == submodelEndpoint.ProtocolInformation.EndpointAddress) == null)
                                    submodelEndpoints.Add(submodelEndpoint);
                            }
                        }
                        aasDescriptor.SubmodelDescriptors.First(s => s.IdShort == submodelDescriptor.IdShort).AddEndpoints(submodelEndpoints);
                    }
                }
            }
            else
                retrieveDescriptor.LogResult(logger, LogLevel.Error, "Could not retrieve AssetAdministrationShell descriptor");
        }

        private class EndpointComparer : IEqualityComparer<IEndpoint>
        {
            public bool Equals(IEndpoint x, IEndpoint y)
            {
                if (x.ProtocolInformation.EndpointAddress == y.ProtocolInformation.EndpointAddress)
                    return true;
                else
                    return false;
            }

            public int GetHashCode(IEndpoint obj)
            {
                return obj.GetHashCode();
            }
        }

        private static void DiscoveryServer_ServiceInstanceShutdown(object sender, ServiceInstanceEventArgs e)
        {
            try
            {
                if (assetAdministrationShellRegistry != null && e.TxtRecords?.Count > 0)
                {
                    string aasIdKeyValue = e.TxtRecords.FirstOrDefault(t => t.StartsWith(ASSETADMINISTRATIONSHELL_ID + KEY_VALUE_SEPERATOR));
                    if (!string.IsNullOrEmpty(aasIdKeyValue))
                    {
                        string[] splittedItem = aasIdKeyValue.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        if (splittedItem != null && splittedItem.Length == 2)
                        {
                            if (splittedItem[0] == ASSETADMINISTRATIONSHELL_ID)
                            {
                                var deletedResult = assetAdministrationShellRegistry.DeleteAssetAdministrationShellRegistration(splittedItem[1]);
                                if (deletedResult.Success)
                                    logger.LogInformation($"Successfully deregistered Asset Administration Shell with id {splittedItem[1]} from registry");                                    
                                else
                                    deletedResult.LogResult(logger, LogLevel.Error, $"Could not unregister Asset Administration Shell with id {splittedItem[1]} from registry");
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Error service instance shutdown");
            }
        }

        public static void StopDiscovery(this IAssetAdministrationShellRegistryInterface registryHttpServer)
        {
            discoveryServer.ServiceInstanceDiscovered -= DiscoveryServer_ServiceInstanceDiscovered;
            discoveryServer.ServiceInstanceShutdown -= DiscoveryServer_ServiceInstanceShutdown;
            discoveryServer.Stop();
        }

        /// <summary>
        /// Starts mDNS dicovery for an Asset Administration Shell Service Provider with included endpoints in its Service Descriptor
        /// </summary>
        /// <param name="serviceProvider">The Asset Administration Shell Service Provider</param>
        public static void StartDiscovery(this IAssetAdministrationShellServiceProvider serviceProvider)
        {
            List<IPAddress> ipAddresses = new List<IPAddress>();
            int port = -1;
            foreach (var endpoint in serviceProvider.ServiceDescriptor.Endpoints)
            {
                Uri uriEndpoint = new Uri(endpoint.ProtocolInformation.EndpointAddress);
                if(port == -1)
                    port = uriEndpoint.Port;

                if (IPAddress.TryParse(uriEndpoint.Host, out IPAddress address))
                    ipAddresses.Add(address);
            }
            StartDiscovery(serviceProvider, port, ipAddresses);
        }
        /// <summary>
        /// Starts mDNS discovery for an Asset Administration Shell Service Provider with a list of given IP-addresses and a port
        /// </summary>
        /// <param name="serviceProvider">The Asset Administration Shell Service Provider</param>
        /// <param name="port">The port to advertise</param>
        /// <param name="iPAddresses">A list of IP-addresses to advertise, if empty uses locally dicoverable multicast IP addresses</param>
        public static void StartDiscovery(this IAssetAdministrationShellServiceProvider serviceProvider, int port, IEnumerable<IPAddress> iPAddresses)
        {
            discoveryClient = new DiscoveryClient(serviceProvider.ServiceDescriptor.IdShort, (ushort)port, ServiceTypes.AAS_SERVICE_TYPE, iPAddresses);
            discoveryClient.AddProperty(ASSETADMINISTRATIONSHELL_ID, serviceProvider.ServiceDescriptor.Id.Id);
            discoveryClient.AddProperty(ASSETADMINISTRATIONSHELL_IDSHORT, serviceProvider.ServiceDescriptor.IdShort);
            for (int i = 0; i < serviceProvider.ServiceDescriptor.Endpoints.Count(); i++)
            {
                var endpoint = serviceProvider.ServiceDescriptor.Endpoints.ElementAt(i);
                discoveryClient.AddProperty(ASSETADMINISTRATIONSHELL_ENDPOINT + "." + endpoint.ProtocolInformation.EndpointProtocol + "[" + i + "]", endpoint.ProtocolInformation.EndpointAddress);
            }
   
            discoveryClient.Start();
            
        }
        public static void StopDiscovery(this IAssetAdministrationShellServiceProvider serviceProvider)
        {
            discoveryClient.Stop();
        }                   
    }
}
