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

namespace BaSyx.API.Interfaces
{
    public interface ISubmodelRepositoryInterface
    {
        IResult<ISubmodel> CreateSubmodel(ISubmodel submodel);

        IResult<ISubmodel> RetrieveSubmodel(string submodelIdentifier);

        IResult<IElementContainer<ISubmodel>> RetrieveSubmodels();

        IResult UpdateSubmodel(string submodelIdentifier, ISubmodel submodel);

        IResult DeleteSubmodel(string submodelIdentifier);
    }
}
