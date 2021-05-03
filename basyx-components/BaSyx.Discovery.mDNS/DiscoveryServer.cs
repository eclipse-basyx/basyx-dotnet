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
using Makaretu.Dns;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace BaSyx.Discovery.mDNS
{
    public class DiscoveryServer
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private const int DISCOVER_THREAD_DELAY = 10000;

        public event EventHandler<ServiceDiscoveredEventArgs> ServiceDiscovered;
        public event EventHandler<ServiceInstanceEventArgs> ServiceInstanceDiscovered;
        public event EventHandler<ServiceInstanceEventArgs> ServiceInstanceShutdown;

        public string ServiceType { get; }

        private CancellationTokenSource cancellationToken;
        private readonly Thread discoverThread;
        private readonly MulticastService mdns;

        public DiscoveryServer(string serviceType)
        {
            ServiceType = serviceType;
            discoverThread = new Thread(Discover);
            
            mdns = new MulticastService();
        }

        private void Discover()
        {
            ServiceDiscovery sd = new ServiceDiscovery(mdns);

            mdns.NetworkInterfaceDiscovered += (s, e) =>
            {
                foreach (var networkInterface in e.NetworkInterfaces)
                {
                    logger.Info($"Network-Interface: '{networkInterface.Name}' found");
                }
            };

            sd.ServiceDiscovered += Sd_ServiceDiscovered;
            sd.ServiceInstanceDiscovered += Sd_ServiceInstanceDiscovered;
            sd.ServiceInstanceShutdown += Sd_ServiceInstanceShutdown;

            try
            {
                mdns.Start();

                cancellationToken = new CancellationTokenSource();
                while (!cancellationToken.IsCancellationRequested)
                {
                    sd.QueryAllServices();
                    Thread.Sleep(DISCOVER_THREAD_DELAY);
                }
            }                
            finally
            {
                sd.Dispose();
                mdns.Stop();

            }
        }

       
        private void Sd_ServiceDiscovered(object sender, DomainName serviceName)
        {
            logger.Info($"service '{serviceName}' discovered");

            ServiceDiscovered?.Invoke(sender, new ServiceDiscoveredEventArgs() { ServiceName = serviceName.ToString() });
            
            if(serviceName.ToString().Contains(ServiceType))
                mdns.SendQuery(serviceName, type: DnsType.PTR);
        }

        private void Sd_ServiceInstanceDiscovered(object sender, ServiceInstanceDiscoveryEventArgs e)
        {
            logger.Info($"service instance '{e.ServiceInstanceName}' discovered");

            var args = GetServiceInstanceEventArgs(e);
            ServiceInstanceDiscovered?.Invoke(sender, args);
        }

        private void Sd_ServiceInstanceShutdown(object sender, ServiceInstanceShutdownEventArgs e)
        {
            logger.Info($"service instance '{e.ServiceInstanceName}' is shutting down");

            var args = GetServiceInstanceEventArgs(e);
            ServiceInstanceShutdown?.Invoke(sender, args);
        }

        private ServiceInstanceEventArgs GetServiceInstanceEventArgs(MessageEventArgs e)
        {
            var servers = e.Message.AdditionalRecords.OfType<SRVRecord>();
            var addresses = e.Message.AdditionalRecords.OfType<AddressRecord>();
            var txtRecords = e.Message.AdditionalRecords.OfType<TXTRecord>()?.SelectMany(s => s.Strings);

            ServiceInstanceEventArgs args = new ServiceInstanceEventArgs();
            if (txtRecords?.Count() > 0)
                args.TxtRecords.AddRange(txtRecords);

            if (servers?.Count() > 0 && addresses?.Count() > 0)
            {
                foreach (var server in servers)
                {
                    logger.Info($"host '{server.Target}' for '{server.Name}' at port '{server.Port}'");
                    var serverAddresses = addresses.Where(w => w.Name == server.Target);
                    if (serverAddresses?.Count() > 0)
                    {
                        foreach (var serverAddress in serverAddresses)
                        {
                            logger.Info($"host '{serverAddress.Name}' at {serverAddress.Address}");
                            args.Servers.Add(new Server()
                            {
                                Name = server.Name.ToString(),
                                Target = server.Target.ToString(),
                                Port = server.Port,
                                Address = serverAddress.Address
                            });
                        }
                        return args;
                    }
                }
            }
            return args;
        }
        
        public void Start()
        {
            logger.Info("Discover thread starting...");
            discoverThread.Start();
            logger.Info("Discover thread started successfully" );
        }


        public void Stop()
        {
            logger.Info("Discover thread stopping...");
            cancellationToken?.Cancel();
            bool success = discoverThread.Join(DISCOVER_THREAD_DELAY + 500);
            logger.Info("Discover thread stopped successfully:" + success);
        }
    }

    public class ServiceInstanceEventArgs
    {
        public List<Server> Servers { get; set; }
        public List<string> TxtRecords { get; set; }

        public ServiceInstanceEventArgs()
        {
            Servers = new List<Server>();
            TxtRecords = new List<string>();
        }
    }

    public class Server
    {
        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public string Target { get; set; }
        public string Name { get; set; }
    }

    public class ServiceDiscoveredEventArgs
    {
        public string ServiceName { get; internal set; }
    }
}
