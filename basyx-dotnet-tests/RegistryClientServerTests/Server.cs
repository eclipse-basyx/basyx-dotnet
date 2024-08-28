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
using BaSyx.Registry.ReferenceImpl.InMemory;
using BaSyx.Registry.Server.Http;
using BaSyx.Utils.Settings;
using System.Collections.Generic;

namespace RegistryClientServerTests
{
    class Server
    {
        public static string ServerUrl = "http://localhost:4999";
        public static void Run()
        {
            ServerSettings settings = new ServerSettings()
            {
               ServerConfig = new ServerConfiguration()
               {
                   Hosting = new HostingConfiguration()
                   {
                       Urls = new List<string>() { ServerUrl }
                   }
               }
            };
            RegistryHttpServer registryServer = new RegistryHttpServer(settings);
            InMemoryRegistry inMemoryRegistry = new InMemoryRegistry();
            registryServer.SetRegistryProvider(inMemoryRegistry);
            _ = registryServer.RunAsync();
        }
    }
}
