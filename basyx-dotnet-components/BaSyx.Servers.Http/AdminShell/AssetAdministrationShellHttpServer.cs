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
using BaSyx.API.Http.Controllers;
using BaSyx.API.ServiceProvider;
using BaSyx.Components.Common;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BaSyx.Servers.AdminShell.Http
{
    public class AssetAdministrationShellHttpServer : ServerApplication
    {
        public AssetAdministrationShellHttpServer() : this(null, null)
        { }

        public AssetAdministrationShellHttpServer(ServerSettings serverSettings) : this(serverSettings, null)
        { }

        public AssetAdministrationShellHttpServer(ServerSettings serverSettings, string[] webHostBuilderArgs)
            : base(serverSettings , webHostBuilderArgs)
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            WebHostBuilder.UseSetting(WebHostDefaults.ApplicationKey, entryAssembly.FullName);
        }

        public void SetServiceProvider(IAssetAdministrationShellServiceProvider aasServiceProvider)
        {
            WebHostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IAssetAdministrationShellServiceProvider>(aasServiceProvider);
                services.AddSingleton<IServiceProvider>(aasServiceProvider);
                services.AddSingleton<IServiceDescriptor>(aasServiceProvider.ServiceDescriptor);
                services.AddMvc((options) =>
                {
                    options.Conventions.Add(new ControllerConvention(this)
                        .Include(typeof(AssetAdministrationShellController))
                        .Include(typeof(DescriptorController)));
                });
            });
        }
    }
}
