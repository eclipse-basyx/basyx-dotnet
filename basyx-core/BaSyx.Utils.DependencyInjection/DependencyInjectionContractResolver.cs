/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
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
                JsonObjectContract contract = DIResolveContract(objectType);
                contract.DefaultCreator = () => ServiceProvider.GetService(objectType);
                return contract;
            }

            return base.CreateObjectContract(objectType);
        }
        private JsonObjectContract DIResolveContract(Type objectType)
        {
            var registeredType = DependencyInjectionExtension.GetRegisteredTypeFor(objectType);
            if (registeredType != null)
                return base.CreateObjectContract(registeredType);
            else
                return CreateObjectContract(objectType);
        }
    }
}
