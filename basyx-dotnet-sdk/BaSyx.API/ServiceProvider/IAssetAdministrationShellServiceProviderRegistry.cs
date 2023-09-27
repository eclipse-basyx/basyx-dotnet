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
using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;

namespace BaSyx.API.ServiceProvider
{
    public interface IAssetAdministrationShellServiceProviderRegistry
    {
        IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(string aasIdentifier, IAssetAdministrationShellServiceProvider aasServiceProvider);
        IResult UnregisterAssetAdministrationShellServiceProvider(string aasIdentifier);
        IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(string aasIdentifier);
        IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders();
    }
}
