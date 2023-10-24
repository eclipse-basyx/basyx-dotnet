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
using BaSyx.Registry.Client.Http;
using CommandLine;
using System;
using System.Linq;

namespace BaSyx.Discovery.mDNS.Forwarder
{
    class Program
    {
        private static RegistryClientSettings registryClientSettings;

        public class Options
        {
            [Option('s', "settings", Required = false, HelpText = "Path to the RegistryClientSettings.xml")]
            public string SettingsFilePath { get; set; }

            [Option('u', "url", Required = false, HelpText = "Target registry URL, e.g. http://myServerRegistry.com:4999")]
            public string Url { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (!string.IsNullOrEmpty(o.SettingsFilePath))
                           registryClientSettings = RegistryClientSettings.LoadSettingsFromFile(o.SettingsFilePath);
                       else
                           registryClientSettings = RegistryClientSettings.LoadSettings();

                       if (!string.IsNullOrEmpty(o.Url))
                           registryClientSettings.RegistryConfig.RegistryUrl = o.Url;
                   });

            if (args.Contains("--help") || args.Contains("--version"))
                return;

            RegistryHttpClient client = new RegistryHttpClient(registryClientSettings);
            client.StartDiscovery();

            Console.WriteLine($"mDNS-Forwarder started with target registry {registryClientSettings.RegistryConfig.RegistryUrl}");

            Console.WriteLine("Press any key to quit");
            Console.ReadKey();

            client.StopDiscovery();
        }
    }
}
