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
using BaSyx.Utils.Assembly;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings.Types
{
    public class DependencySettings : Settings<DependencySettings>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<DependencySettings>();

        public DependencyConfiguration DependencyCollection { get; set; } = new DependencyConfiguration();

        public class DependencyConfiguration
        {
            [XmlArrayItem("Dependency")]
            public List<Dependency> Dependencies { get; set; } = new List<Dependency>();
        }

        public class Dependency
        {
            [XmlElement]
            public string DllPath { get; set; }
            [XmlElement]
            public string InterfaceType { get; set; }
            [XmlElement]
            public string ImplementationType { get; set; }
            [XmlElement]
            public ServiceLifetime ServiceLifetime { get; set; }
        }
      
        public IServiceCollection GetServiceCollection(List<System.Reflection.Assembly> externalAssemblies = null)
        {
            List<System.Reflection.Assembly> assemblies = externalAssemblies ?? AssemblyUtils.GetLoadedAssemblies();
           
            if (DependencyCollection?.Dependencies?.Count > 0)
            {
                ServiceCollection serviceCollection = new ServiceCollection();
                foreach (var dependency in DependencyCollection.Dependencies)
                {
                    string dllPath = Path.Combine(ExecutionPath, dependency.DllPath);
                    if(!string.IsNullOrEmpty(dependency.DllPath) && File.Exists(dllPath))
                    {
                        try
                        {
                            System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(dllPath);
                            if (!assemblies.Contains(assembly))
                                assemblies.Add(assembly);
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, "Failed to load assembly " + dllPath);
                        }
                     
                    }


                    try
                    {
                        Type interfaceType = assemblies.Find(a => a.GetType(dependency.InterfaceType, false) != null)?.GetType(dependency.InterfaceType);
                        if (interfaceType == null)
                            throw new DllNotFoundException("Dll not found for interfaceType");

                        Type implementationType = assemblies.Find(a => a.GetType(dependency.ImplementationType, false) != null)?.GetType(dependency.ImplementationType);
                        if (implementationType == null)
                            throw new DllNotFoundException("Dll not found for implementationType");
                        ServiceDescriptor serviceDescriptor = new ServiceDescriptor(interfaceType, implementationType, dependency.ServiceLifetime);
                        serviceCollection.Add(serviceDescriptor);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Failed to load dependency " + dependency.ImplementationType + " for interface: " + dependency.InterfaceType);
                        continue;
                    }
                }
                return serviceCollection;
            }
            return null;
        }

    }
}
