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
using BaSyx.Utils.Json;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BaSyx.Utils.DependencyInjection
{
    public class DependencyInjectionJsonSerializerSettings : DefaultJsonSerializerSettings
    {
        public IServiceProvider ServiceProvider { get; }
        public IServiceCollection Services { get; }

        public DependencyInjectionJsonSerializerSettings() :
            this(new ServiceCollection().AddStandardImplementation())
        { }

        public DependencyInjectionJsonSerializerSettings(IServiceCollection services) : base()
        {
            Services = services;
            DefaultServiceProviderFactory serviceProviderFactory = new DefaultServiceProviderFactory();
            ServiceProvider = serviceProviderFactory.CreateServiceProvider(Services);
            ContractResolver = new DependencyInjectionContractResolver(new DependencyInjectionExtension(Services));
        }        
    }
}
