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

namespace BaSyx.API.Interfaces
{
    public interface ISubmodelRepositoryInterface
    {
        IResult<ISubmodel> CreateSubmodel(ISubmodel submodel);

        IResult<ISubmodel> RetrieveSubmodel(Identifier id);

        IResult<PagedResult<IElementContainer<ISubmodel>>> RetrieveSubmodels(int limit = 100, string cursor = "", string semanticId = "", string idShort = "");

        IResult UpdateSubmodel(Identifier id, ISubmodel submodel);

        IResult DeleteSubmodel(Identifier id);
    }
}
