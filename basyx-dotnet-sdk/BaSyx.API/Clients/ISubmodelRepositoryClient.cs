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

using BaSyx.API.Interfaces;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.ResultHandling.ResultTypes;
using System.Threading.Tasks;

namespace BaSyx.API.Clients
{
    public interface ISubmodelRepositoryClient : ISubmodelRepositoryInterface, IClient
    {
        Task<IResult<ISubmodel>> CreateSubmodelAsync(ISubmodel submodel);

        Task<IResult<ISubmodel>> RetrieveSubmodelAsync(Identifier id);

        Task<IResult<PagedResult<IElementContainer<ISubmodel>>>> RetrieveSubmodelsAsync(int limit = 100, string cursor = "", string semanticId = "", string idShort = "");

        Task<IResult> UpdateSubmodelAsync(Identifier id, ISubmodel submodel);

        Task<IResult> DeleteSubmodelAsync(Identifier id);
    }
}
