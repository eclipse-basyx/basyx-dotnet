/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.Models.AdminShell;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;

namespace BaSyx.API.ServiceProvider
{
    public interface ISubmodelServiceProviderRegistry
    {
        IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(Identifier id, ISubmodelServiceProvider submodelServiceProvider);
        IResult UnregisterSubmodelServiceProvider(Identifier id);
        IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(Identifier id);
        IResult<IEnumerable<ISubmodelServiceProvider>> GetSubmodelServiceProviders();
    }
}
