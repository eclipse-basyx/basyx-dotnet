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
using System.Threading.Tasks;

namespace BaSyx.API.Clients
{
    public interface ISubmodelRepositoryClient : ISubmodelRepositoryInterface, IClient
    {
        Task<IResult<ISubmodel>> CreateSubmodelAsync(ISubmodel submodel);

        Task<IResult<ISubmodel>> RetrieveSubmodelAsync(string submodelIdentifier);

        Task<IResult<IElementContainer<ISubmodel>>> RetrieveSubmodelsAsync();

        Task<IResult> UpdateSubmodelAsync(string submodelIdentifier, ISubmodel submodel);

        Task<IResult> DeleteSubmodelAsync(string submodelIdentifier);
    }
}
