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

namespace BaSyx.Utils.DependencyInjection.Abstractions
{
    public interface IDependencyInjectionExtension
    {
        IServiceCollection ServiceCollection { get; }
        Type GetRegisteredTypeFor(Type t);
        bool IsTypeRegistered(Type t);
    }
}
