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
using BaSyx.Models.Connectivity;

namespace BaSyx.API.ServiceProvider
{
    public interface IServiceProvider { }

    public interface IServiceProvider<TModelElement, TServiceDescriptor> : IServiceProvider
        where TServiceDescriptor : IServiceDescriptor
    {
        TServiceDescriptor ServiceDescriptor { get; }
        void BindTo(TModelElement element);
        TModelElement GetBinding();
    }
}
