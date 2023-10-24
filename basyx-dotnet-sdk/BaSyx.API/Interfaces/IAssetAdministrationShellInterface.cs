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
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.ResultHandling.ResultTypes;
using System.Collections.Generic;

namespace BaSyx.API.Interfaces
{
    public interface IAssetAdministrationShellInterface
    {
        IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell();

        IResult UpdateAssetAdministrationShell(IAssetAdministrationShell aas);

        IResult<IAssetInformation> RetrieveAssetInformation();

        IResult UpdateAssetInformation(IAssetInformation assetInformation);

        IResult<PagedResult<IEnumerable<IReference<ISubmodel>>>> RetrieveAllSubmodelReferences();

        IResult<IReference> CreateSubmodelReference(IReference submodelRef);

        IResult DeleteSubmodelReference(Identifier id);
    }
}
