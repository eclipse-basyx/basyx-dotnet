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
using Newtonsoft.Json.Serialization;
using System;

namespace BaSyx.Utils.DependencyInjection
{
    public class DependencyInjectionContractResolver : CamelCasePropertyNamesContractResolver, IDependencyInjectionContractResolver
    {
        public IServiceProvider ServiceProvider { get; }
        public IDependencyInjectionExtension DependencyInjectionExtension { get; }
        public DependencyInjectionContractResolver(IDependencyInjectionExtension diExtension)
        {
            DependencyInjectionExtension = diExtension;
            ServiceProvider = DependencyInjectionExtension.ServiceCollection.BuildServiceProvider();
        }
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            if (DependencyInjectionExtension.IsTypeRegistered(objectType))
            {
                JsonObjectContract contract = (JsonObjectContract)DIResolveContract(objectType);
                contract.DefaultCreator = () => ServiceProvider.GetService(objectType);
                return contract;
            }

            return base.CreateObjectContract(objectType);
        }
        private JsonContract DIResolveContract(Type objectType)
        {
            var registeredType = DependencyInjectionExtension.GetRegisteredTypeFor(objectType);
            if (registeredType != null)
                return base.CreateContract(registeredType);
            else
                return CreateContract(objectType);
        }

        protected override JsonArrayContract CreateArrayContract(Type objectType)
        {
            if (DependencyInjectionExtension.IsTypeRegistered(objectType))
            {
                JsonArrayContract contract = (JsonArrayContract)DIResolveContract(objectType);
                contract.DefaultCreator = () => ServiceProvider.GetService(objectType);
                return contract;
            }

            return base.CreateArrayContract(objectType);
        }
    }
}
