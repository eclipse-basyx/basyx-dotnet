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
using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using System.Linq;

namespace BaSyx.API.AssetAdministrationShell.Extensions
{
    public static class AssetAdministrationShellExtensions
    {
        public static IAssetAdministrationShellServiceProvider CreateServiceProvider(this IAssetAdministrationShell aas, bool includeSubmodels)
        {
            InternalAssetAdministrationShellServiceProvider sp = new InternalAssetAdministrationShellServiceProvider(aas);            

            if(includeSubmodels && aas.Submodels?.Count() > 0)
                foreach (var submodel in aas.Submodels.Values)
                {
                    var submodelSp = submodel.CreateServiceProvider();
                    sp.RegisterSubmodelServiceProvider(submodel.IdShort, submodelSp);
                }

            return sp;
        }
    }
}
