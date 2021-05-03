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
using BaSyx.Utils.JsonHandling;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BaSyx.Utils.DependencyInjection
{
    public class DependencyInjectionJsonSerializerSettings : DefaultJsonSerializerSettings
    {
        public IServiceProvider ServicePovider { get; }
        public IServiceCollection Services { get; }

        public DependencyInjectionJsonSerializerSettings() :
            this(new ServiceCollection().AddStandardImplementation())
        { }

        public DependencyInjectionJsonSerializerSettings(IServiceCollection services) : base()
        {
            Services = services;
            DefaultServiceProviderFactory serviceProviderFactory = new DefaultServiceProviderFactory();
            ServicePovider = serviceProviderFactory.CreateServiceProvider(Services);
            ContractResolver = new DependencyInjectionContractResolver(new DependencyInjectionExtension(Services));
        }        
    }
}
