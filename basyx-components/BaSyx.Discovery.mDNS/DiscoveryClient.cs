/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
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
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace BaSyx.Discovery.mDNS
{
    public class DiscoveryClient
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly object locker = new object();

        private ServiceProfile serviceProfile;
        private Thread advertiseThread;
        private bool IsRunning;
        private readonly MulticastService mdns;

        public ushort Port { get; }
        public string ServiceName { get; }
        public string ServiceType { get; }
        public IEnumerable<IPAddress> IPAddresses { get; }

        public DiscoveryClient(string serviceName, ushort port, string serviceType) : 
            this(serviceName, port, serviceType, null)
        { }

        public DiscoveryClient(string serviceName, ushort port, string serviceType, IEnumerable<IPAddress> ipAddresses)
        {
            ServiceName = serviceName;
            ServiceType = serviceType;
            Port = port;

            mdns = new MulticastService();
            IPAddresses = ipAddresses ?? MulticastService.GetIPAddresses();

            logger.Info("Creating service profile with" +
                "\tServiceName=" + serviceName + 
                "\tServiceType=" + serviceType +
                "\tPort=" + port +
                "\tIPAddresses=" + string.Join(";", IPAddresses));

            serviceProfile = new ServiceProfile(ServiceName, ServiceType, Port, IPAddresses);

            advertiseThread = new Thread(Advertise);
            IsRunning = false;
        }

        /// <summary>
        /// Add information (key-value-pair) to TXT-record of advertise message
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddProperty(string key, string value)
        {
            serviceProfile.AddProperty(key, value);
        }

        private void Advertise()
        {
            ServiceDiscovery serviceDiscovery = new ServiceDiscovery(mdns);

            serviceDiscovery.Advertise(serviceProfile);

            mdns.Start();

            lock (locker)
                while (IsRunning)
                    Monitor.Wait(locker);

            serviceDiscovery.Unadvertise(serviceProfile);
        }

        public void Start()
        {
            logger.Info("Advertise thread starting...");
            lock (locker)
                IsRunning = true;
            advertiseThread.Start();
            logger.Info("Advertise thread started successfully");
        }


        public void Stop()
        {
            logger.Info("Advertise thread stopping...");
            lock (locker)
            {
                IsRunning = false;
                Monitor.Pulse(locker);
            }
            bool success = advertiseThread.Join(5000);
            logger.Info("Advertise thread stopped successfully:" + success);
        }
    }
}
