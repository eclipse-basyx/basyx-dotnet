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
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;

namespace BaSyx.API.Interfaces
{
    public interface IAssetAdministrationShellInterface
    {
        IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(RequestContent content);

        IResult UpdateAssetAdministrationShell(IAssetAdministrationShell aas);

        IResult<IAssetInformation> RetrieveAssetInformation();

        IResult UpdateAssetInformation(IAssetInformation assetInformation);

        IResult<IEnumerable<IReference<ISubmodel>>> RetrieveAllSubmodelReferences();

        IResult<IReference> CreateSubmodelReference(IReference submodelRef);

        IResult DeleteSubmodelReference(string submodelIdentifier);
    }
}
