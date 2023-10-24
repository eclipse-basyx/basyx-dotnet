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
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BaSyx.Utils.DependencyInjection
{
    public static class DependencyInjectionActivator
    {
        public static object CreateInstance(IServiceProvider serviceProvider, Type interfaceType)
        {
            if (serviceProvider == null || interfaceType == null || !interfaceType.IsInterface)
                return null;

            object instance = ActivatorUtilities.CreateInstance(serviceProvider, interfaceType);
            return instance;
        }

        public static object CreateStandardInstance(Type interfaceType)
        {
            IServiceProvider serviceProvider = DefaultImplementation.GetStandardServiceProvider();
            object instance = CreateInstance(serviceProvider, interfaceType);
            return instance;
        }
        
        public static T CreateStandardInstance<T>()
        {
            object instance = CreateStandardInstance(typeof(T));
            if (instance is T castedInstance)
                return castedInstance;
            else
                return default;
        }

        public static T CreateInstance<T>(IServiceProvider serviceProvider)
        {
            object instance = CreateInstance(serviceProvider, typeof(T));
            if (instance is T castedInstance)
                return castedInstance;
            else
                return default;            
        }
    }
}
