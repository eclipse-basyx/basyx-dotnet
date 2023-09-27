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
using Microsoft.Extensions.Logging;
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
        private static readonly ILogger logger = LoggingExtentions.CreateLogger("NetworkUtils");

        /// <summary>
        /// This method returns the closest source IP address relative to the the target IP address. 
        /// The probality of being able to call the target IP hence increases.
        /// </summary>
        /// <param name="target">Target IP address to call</param>
        /// <param name="sources">Source IP address from where to call the target</param>
        /// <returns>The closest source IP address to the target IP address or the loopback address</returns>
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
        /// <param name="includeIPv6">Include IPv6 IP addresses as well</param>
        /// <returns>Sequence of IP addresses</returns>
        public static IEnumerable<IPAddress> GetIPAddresses(bool includeIPv6)
        {
            IEnumerable<NetworkInterface> networkInterfaces = GetOperationalNetworkInterfaces();
            var addresses = networkInterfaces?.SelectMany(n => n.GetIPProperties().UnicastAddresses)?.Select(s => s.Address);
            if (includeIPv6)
                return addresses;
            else
                return addresses?.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Returns all link local IP addresses
        /// </summary>
        /// <param name="includeIPv6">Include IPv6 IP addresses as well</param>
        /// <returns>Sequence of link local IP addresses</returns>
        public static IEnumerable<IPAddress> GetLinkLocalIPAddresses(bool includeIPv6)
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> ipAddresses = new List<IPAddress>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork || (includeIPv6 && ip.AddressFamily == AddressFamily.InterNetworkV6 && ip.IsIPv6LinkLocal))
                    ipAddresses.Add(ip);
            }
            return ipAddresses;
        }

        /// <summary>
        /// Pings a host and returns true if ping was successfull otherwise false (default timeout = 5000ms)
        /// </summary>
        /// <param name="hostNameOrAddress">IP-address or host name</param>
        /// <param name="timeout">Timeout for ping</param>
        /// <returns>true if pingable, false otherwise</returns>
        public static async Task<bool> PingHostAsync(string hostNameOrAddress, int timeout = 5000)
        {
            if (string.IsNullOrEmpty(hostNameOrAddress))
                throw new ArgumentNullException(nameof(hostNameOrAddress));

            try
            {
                using (Ping pinger = new Ping())
                {
                    logger.LogInformation($"Pinging {hostNameOrAddress}...");
                    PingReply reply = await pinger.SendPingAsync(hostNameOrAddress, timeout).ConfigureAwait(false);
                    if (reply.Status == IPStatus.Success)
                    {
                        logger.LogInformation($"Ponged from {hostNameOrAddress} successfully");
                        return true;
                    }
                    else
                    {
                        logger.LogWarning($"Pinging {hostNameOrAddress} -> PingReply-Status: " + Enum.GetName(typeof(IPStatus), reply.Status));
                        return false;
                    }
                }
            }
            catch (PingException e)
            {
                logger.LogError(e, "Ping-Exception");
                return false;
            }
        }
    }
}
