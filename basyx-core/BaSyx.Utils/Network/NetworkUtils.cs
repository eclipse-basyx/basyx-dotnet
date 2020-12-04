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
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BaSyx.Utils.Network
{
    public static class NetworkUtils
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// This method returns the closest source IP address relative to the the target IP address. 
        /// The probality of being able to call the target IP hence increases.
        /// </summary>
        /// <param name="target">Target IP address to call</param>
        /// <param name="sources">Source IP address from where to cal the target</param>
        /// <returns>The closest source IP address to the the target IP address or the Loopback Address</returns>
        public static IPAddress GetClosestIPAddress(IPAddress target, List<IPAddress> sources)
        {
            Dictionary<int, IPAddress> scoredSourceIPAddresses = new Dictionary<int, IPAddress>();
            byte[] targetBytes = target.GetAddressBytes();
            foreach (var source in sources)
            {
                byte[] sourceBytes = source.GetAddressBytes();
                int score = CompareIPByteArray(targetBytes, sourceBytes);

                if(!scoredSourceIPAddresses.ContainsKey(score) && score != 0)
                    scoredSourceIPAddresses.Add(score, source);
            }

            if(scoredSourceIPAddresses.Count > 0)
                return scoredSourceIPAddresses[scoredSourceIPAddresses.Keys.Max()];

            return IPAddress.Loopback;
        }

        private static int CompareIPByteArray(byte[] target, byte[] source)
        {
            if (target.Length != source.Length)
                return 0;

            int score = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (target[i] == source[i])
                    score++;
                else
                    return score;
            }
            return score;
        }
        /// <summary>
        /// Returns a sequence of network interfaces which are up and running. 
        /// If there is no other than the loopback interface available, the loopback interface is returned
        /// </summary>
        /// <returns>Sequence of network interfaces</returns>
        public static IEnumerable<NetworkInterface> GetOperationalNetworkInterfaces()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            IEnumerable<NetworkInterface> selectedInterfaces = networkInterfaces?.Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback);
            if(selectedInterfaces?.Count() > 0)
                return selectedInterfaces;
            else
                return networkInterfaces?.Where(n => n.OperationalStatus == OperationalStatus.Up);
        }

        /// <summary>
        /// Returns all IP addresses of all network interfaces without loopback
        /// </summary>
        /// <returns>Sequence of IP addresses</returns>
        public static IEnumerable<IPAddress> GetIPAddresses()
        {
            IEnumerable<NetworkInterface> networkInterfaces = GetOperationalNetworkInterfaces();
            return networkInterfaces?.SelectMany(n => n.GetIPProperties().UnicastAddresses)?.Select(s => s.Address);
        }

        /// <summary>
        /// Returns all link local IP addresses
        /// </summary>
        /// <returns>Sequence of link local IP addresses</returns>
        public static List<IPAddress> GetLinkLocalIPAddresses()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> ipAddresses = new List<IPAddress>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6 && ip.IsIPv6LinkLocal)
                    ipAddresses.Add(ip);
            }
            return ipAddresses;
        }

        /// <summary>
        /// Pings a host and returns true if ping was successfull otherwise false
        /// </summary>
        /// <param name="hostNameOrAddress">IP-address or host name</param>
        /// <returns>true if pingable, false otherwise</returns>
        public static async Task<bool> PingHostAsync(string hostNameOrAddress)
        {
            if (string.IsNullOrEmpty(hostNameOrAddress))
                throw new ArgumentNullException(nameof(hostNameOrAddress));

            try
            {
                using (Ping pinger = new Ping())
                {
                    PingReply reply = await pinger.SendPingAsync(hostNameOrAddress);
                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                    else
                    {
                        logger.Warn($"Pinging {hostNameOrAddress} PingReply-Status: " + Enum.GetName(typeof(IPStatus), reply.Status));
                        return false;
                    }
                }
            }
            catch (PingException e)
            {
                logger.Error(e, "Ping-Exception");
                return false;
            }
        }
    }
}
