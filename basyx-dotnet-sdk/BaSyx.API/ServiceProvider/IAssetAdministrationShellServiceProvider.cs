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
using BaSyx.API.Interfaces;
using BaSyx.Models.Connectivity;
using BaSyx.Models.AdminShell;
using BaSyx.API.Clients;

namespace BaSyx.API.ServiceProvider
{
    public interface IAssetAdministrationShellServiceProvider 
        : IServiceProvider<IAssetAdministrationShell, IAssetAdministrationShellDescriptor>, IAssetAdministrationShellInterface
    {
        ISubmodelServiceProviderRegistry SubmodelProviderRegistry { get; }
    }

    public static class AssetAdministrationShellServiceProviderExtensions
    {
        public static IAssetAdministrationShellServiceProvider CreateServiceProvider(this IAssetAdministrationShell aas, bool includeSubmodels)
        {
            InternalAssetAdministrationShellServiceProvider sp = new InternalAssetAdministrationShellServiceProvider(aas);

            if (includeSubmodels && aas.Submodels?.Count > 0)
                foreach (var submodel in aas.Submodels.Values)
                {
                    var submodelSp = submodel.CreateServiceProvider();
                    sp.RegisterSubmodelServiceProvider(submodel.Identification.Id, submodelSp);
                }

            return sp;
        }

        public static IAssetAdministrationShellServiceProvider CreateServiceProvider(this IAssetAdministrationShellClient client)
        {
            AssetAdministrationShellClientServiceProvider sp = new AssetAdministrationShellClientServiceProvider(client);

            return sp;
        }
    }
}
