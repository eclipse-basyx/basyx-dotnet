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
using BaSyx.Utils.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace BaSyx.Utils.DependencyInjection
{
    public class DependencyInjectionExtension : IDependencyInjectionExtension
    {
        readonly IDictionary<Type, Type> typeDictionary = new Dictionary<Type, Type>();
        public IServiceCollection ServiceCollection { get; }

        public DependencyInjectionExtension(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            foreach (var service in serviceCollection)
            {
                typeDictionary[service.ServiceType] = service.ImplementationType;
            }
        }

        
        public Type GetRegisteredTypeFor(Type t)
        {
            return typeDictionary[t];
        }
        public bool IsTypeRegistered(Type t)
        {
            return typeDictionary.ContainsKey(t);
        }
    }
}
