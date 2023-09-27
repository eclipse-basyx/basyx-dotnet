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
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaSyx.API.Clients
{
    public interface IAssetAdministrationShellClient : IAssetAdministrationShellInterface, IClient
    {
        Task<IResult<IAssetAdministrationShell>> RetrieveAssetAdministrationShellAsync(RequestContent content);

        Task<IResult> UpdateAssetAdministrationShellAsync(IAssetAdministrationShell aas);

        Task<IResult<IAssetInformation>> RetrieveAssetInformationAsync();

        Task<IResult> UpdateAssetInformationAsync(IAssetInformation assetInformation);

        Task<IResult<IEnumerable<IReference<ISubmodel>>>> RetrieveAllSubmodelReferencesAsync();

        Task<IResult<IReference>> CreateSubmodelReferenceAsync(IReference submodelRef);

        Task<IResult> DeleteSubmodelReferenceAsync(string submodelIdentifier);
    }
}
