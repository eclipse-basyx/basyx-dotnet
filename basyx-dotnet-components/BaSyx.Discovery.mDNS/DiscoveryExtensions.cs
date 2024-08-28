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
using BaSyx.Clients.AdminShell.Http;
using BaSyx.API.ServiceProvider;
using BaSyx.Models.Connectivity;
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
        private static List<DiscoveryClient> discoveryClients = new List<DiscoveryClient>();
        private static IAssetAdministrationShellRegistryInterface assetAdministrationShellRegistry;

        public const string ASSETADMINISTRATIONSHELL_ID = "aas.id";
        public const string ASSETADMINISTRATIONSHELL_IDSHORT = "aas.idShort";
        public const string ASSETADMINISTRATIONSHELL_ENDPOINT = "aas.endpoint";
        public const char KEY_VALUE_SEPERATOR = '=';

        private static readonly ILogger logger = LoggingExtentions.CreateLogger("DiscoveryExtensions");

        public static void StartDiscovery(this IAssetAdministrationShellRegistryInterface registry)
        {
            assetAdministrationShellRegistry = registry;

            discoveryServer = new DiscoveryServer(ServiceTypes.AAS_SERVICE_TYPE);
            discoveryServer.ServiceInstanceDiscovered += DiscoveryServer_ServiceInstanceDiscovered;
            discoveryServer.ServiceInstanceShutdown += DiscoveryServer_ServiceInstanceShutdown;
            discoveryServer.Start();
        }

        private static void DiscoveryServer_ServiceInstanceDiscovered(object sender, ServiceInstanceEventArgs e)
        {
            try
            {
                IAssetAdministrationShellDescriptor aasDescriptor = null;
                if (e?.TxtRecords?.Count > 0)
                {
                    string aasIdRecord = e.TxtRecords.Where(s => s.StartsWith(ASSETADMINISTRATIONSHELL_ID))?.FirstOrDefault();
                    if (!string.IsNullOrEmpty(aasIdRecord))
                    {
                        string[] splittedRecord = aasIdRecord.Split(new char[] { KEY_VALUE_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
                        if (splittedRecord.Length == 2)
                        {
                            var retrieved = assetAdministrationShellRegistry.RetrieveAssetAdministrationShellRegistration(splittedRecord[1]);
                            if (retrieved.SuccessAndContent)
                                return;
                            else
                            {
                                List<string> endpointRecords = e.TxtRecords.Where(s => s.StartsWith(ASSETADMINISTRATIONSHELL_ENDPOINT))?.ToList();
                                if(endpointRecords?.Count > 0)
                                    AssembleDescriptor(ref aasDescriptor, endpointRecords, e.Servers);
                            }
                        }
                    }
                    else
                        return;
                }
                if (aasDescriptor != null && assetAdministrationShellRegistry != null)
                {
                    var registeredResult = assetAdministrationShellRegistry.CreateAssetAdministrationShellRegistration(aasDescriptor);
                    if (registeredResult.Success)
                        logger.LogInformation($"Successfully registered Asset Administration Shell with {aasDescriptor.Id} at registry");
                    else
                        registeredResult.LogResult(logger, LogLevel.Error, $"Could not register Asset Administration Shell with {aasDescriptor.Id} at registry");
                }
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Error accessing discovered service instance");
            }
        }

        private static void AssembleDescriptor(ref IAssetAdministrationShellDescriptor aasDescriptor, List<string> endpointRecords, List<Server> servers)
        {
            //Get AAS descriptor initially if its first discovered
            if (aasDescriptor == null)
            {
                foreach (string endpointRecord in endpointRecords)
                {
                    string[] splittedEndpoint = endpointRecord.Split(new char[] { KEY_VALUE_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
                    if (splittedEndpoint.Length == 2 && (splittedEndpoint[0].ToLower().Contains("http") || splittedEndpoint[0].ToLower().Contains("https")))
                    {
                        Uri endpoint = new Uri(splittedEndpoint[1]);
                        bool endpointValid = CheckEndpoint(endpoint, servers);
                        if (endpointValid)
                        {
                            using (var client = new AssetAdministrationShellHttpClient(endpoint, SimpleHttpClient.DEFAULT_HTTP_CLIENT_HANDLER))
                            {
                                var retrieveDescriptor = client.RetrieveAssetAdministrationShellDescriptor();
                                if (retrieveDescriptor.SuccessAndContent)
                                {
                                    aasDescriptor = retrieveDescriptor.Entity;
                                    break;
                                }
                                else
                                    continue;
                            }
                        }
                    }
                }
            }
            //Remove endpoints from the AAS descriptor that are not in the mDNS endpoint list
            if(aasDescriptor != null)
            {
                foreach (var aasEndpoint in aasDescriptor.Endpoints.ToList())
                {
                    Uri endpoint = new Uri(aasEndpoint.ProtocolInformation.EndpointAddress);
                    bool endpointValid = CheckEndpoint(endpoint, servers);
                    if (!endpointValid)
                        aasDescriptor.DeleteEndpoint(aasEndpoint);
                }
                foreach (var smDescriptor in aasDescriptor.SubmodelDescriptors)
                {
                    foreach (var smEndpoint in smDescriptor.Endpoints.ToList())
                    {
                        Uri endpoint = new Uri(smEndpoint.ProtocolInformation.EndpointAddress);
                        bool endpointValid = CheckEndpoint(endpoint, servers);
                        if (!endpointValid)
                            smDescriptor.DeleteEndpoint(smEndpoint);
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the advertised endpoint is part of the mDNS published endpoints
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="servers"></param>
        /// <returns></returns>
        private static bool CheckEndpoint(Uri endpoint, List<Server> servers)
        {
            if (endpoint.HostNameType == UriHostNameType.IPv4)
            {
                IPAddress iPAddress = IPAddress.Parse(endpoint.Host);
                bool contained = servers.Select(s => s.Address).Contains(iPAddress);
                return contained;
            }
            else if (endpoint.HostNameType == UriHostNameType.Dns)
                return true;
            else
                return false;
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
                        string[] splittedItem = aasIdKeyValue.Split(new char[] { KEY_VALUE_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
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
            discoveryClient.AddProperty(ASSETADMINISTRATIONSHELL_ID, serviceProvider.ServiceDescriptor.Id);
            discoveryClient.AddProperty(ASSETADMINISTRATIONSHELL_IDSHORT, serviceProvider.ServiceDescriptor.IdShort);
            for (int i = 0; i < serviceProvider.ServiceDescriptor.Endpoints.Count(); i++)
            {
                var endpoint = serviceProvider.ServiceDescriptor.Endpoints.ElementAt(i);
                discoveryClient.AddProperty(ASSETADMINISTRATIONSHELL_ENDPOINT + "." + endpoint.ProtocolInformation.EndpointProtocol + "." + i, endpoint.ProtocolInformation.EndpointAddress);
            }
   
            discoveryClient.Start();
            
        }

        /// <summary>
        /// Starts mDNS dicovery for an Asset Administration Shell Repository Service Provider with included endpoints in its Service Descriptor
        /// </summary>
        /// <param name="serviceProvider">The Asset Administration Shell Repository Service Provider</param>
        public static void StartDiscovery(this IAssetAdministrationShellRepositoryServiceProvider serviceProvider)
        {
            List<IPAddress> ipAddresses = new List<IPAddress>();
            int port = -1;
            foreach (var endpoint in serviceProvider.ServiceDescriptor.Endpoints)
            {
                Uri uriEndpoint = new Uri(endpoint.ProtocolInformation.EndpointAddress);
                if (port == -1)
                    port = uriEndpoint.Port;

                if (IPAddress.TryParse(uriEndpoint.Host, out IPAddress address))
                    ipAddresses.Add(address);
            }
            StartDiscovery(serviceProvider, port, ipAddresses);
        }

        /// <summary>
        /// Starts mDNS discovery for an Asset Administration Shell Repository Service Provider with a list of given IP-addresses and a port
        /// </summary>
        /// <param name="serviceProvider">The Asset Administration Shell Service Provider</param>
        /// <param name="port">The port to advertise</param>
        /// <param name="iPAddresses">A list of IP-addresses to advertise, if empty uses locally dicoverable multicast IP addresses</param>
        public static void StartDiscovery(this IAssetAdministrationShellRepositoryServiceProvider serviceProvider, int port, IEnumerable<IPAddress> iPAddresses)
        {
            foreach (var aasDescriptor in serviceProvider.ServiceDescriptor.AssetAdministrationShellDescriptors)
            {
                var discoveryClient = new DiscoveryClient(aasDescriptor.IdShort, (ushort)port, ServiceTypes.AAS_SERVICE_TYPE, iPAddresses);
                discoveryClient.AddProperty(ASSETADMINISTRATIONSHELL_ID, aasDescriptor.Id);
                discoveryClient.AddProperty(ASSETADMINISTRATIONSHELL_IDSHORT, aasDescriptor.IdShort);
                for (int i = 0; i < aasDescriptor.Endpoints.Count(); i++)
                {
                    var endpoint = aasDescriptor.Endpoints.ElementAt(i);
                    discoveryClient.AddProperty(ASSETADMINISTRATIONSHELL_ENDPOINT + "." + endpoint.ProtocolInformation.EndpointProtocol + "." + i, endpoint.ProtocolInformation.EndpointAddress);
                }
                discoveryClients.Add(discoveryClient);
                discoveryClient.Start();
            }           
        }

        public static void StopDiscovery(this IAssetAdministrationShellServiceProvider serviceProvider)
        {
            discoveryClient.Stop();
        }

        public static void StopDiscovery(this IAssetAdministrationShellRepositoryServiceProvider serviceProvider)
        {
            foreach (var discoveryClient in discoveryClients)
            {
                discoveryClient.Stop();
            }
        }
    }
}
