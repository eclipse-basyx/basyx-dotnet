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
using BaSyx.API.Interfaces;
using BaSyx.API.ServiceProvider;
using BaSyx.Utils.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BaSyx.Deployment.AppDataService
{
    public class AppDataContext
    {
        public string HostName { get; set; }
        public bool IsLinux { get; set; }
        public bool IsWindows { get; set; }
        public Architecture OSArchitecture { get; set; }
        public DateTime TimeStamp { get; set; }
        public Dictionary<string, Settings> Settings { get; set; }
        public Dictionary<string, string> Files { get; set; }       

        private Dictionary<Type, IAssetAdministrationShellServiceProvider> AdminShellServices { get; set; }
        private IAssetAdministrationShellRegistryInterface Registry { get; set; }

        private IServiceCollection Services { get; set; }

        public AppDataContext()
        {
            Settings = new Dictionary<string, Settings>();
            Files= new Dictionary<string, string>();
            AdminShellServices = new Dictionary<Type, IAssetAdministrationShellServiceProvider>();
            Services = new ServiceCollection();
        }

        public void AddRegistry(IAssetAdministrationShellRegistryInterface registry)
        {
            Registry = registry;
        }

        public IAssetAdministrationShellRegistryInterface GetRegistry() 
        { 
            return Registry; 
        }

        public void AddService<T>(T instance) where T : class
        {
            Services.AddSingleton<T>(instance);
        }

		public T GetService<T>() where T : class
		{
            var provider = Services.BuildServiceProvider();
            return provider.GetService<T>();
		}

		public void AddAdminShellService<T>(T service) where T : IAssetAdministrationShellServiceProvider
        {
            AdminShellServices.TryAdd(typeof(T), service);                
        }

        public T GetAdminShellService<T>() where T : IAssetAdministrationShellServiceProvider
        {
            if (AdminShellServices.TryGetValue(typeof(T), out var service))
                return (T)service;
            else
                return default;
        }


    }
}
