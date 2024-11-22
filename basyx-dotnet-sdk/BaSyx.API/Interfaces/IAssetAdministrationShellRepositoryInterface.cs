/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.ResultHandling.ResultTypes;
using System.Collections.Generic;
using System.Text.Json;

namespace BaSyx.API.Interfaces
{
    public interface IAssetAdministrationShellRepositoryInterface
    {
        IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell aas);

        IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(Identifier id);

        IResult<PagedResult<IElementContainer<IAssetAdministrationShell>>> RetrieveAssetAdministrationShells(int limit = 100, string cursor = "", string assetIds = "", string idShort = "");

        IResult<PagedResult<IEnumerable<IReference<IAssetAdministrationShell>>>> RetrieveAssetAdministrationShellsReference(int limit = 100, string cursor = "", string assetIds = "", string idShort = "");

        IResult UpdateAssetAdministrationShell(Identifier id, IAssetAdministrationShell aas);

        IResult DeleteAssetAdministrationShell(Identifier id);
    }
}
