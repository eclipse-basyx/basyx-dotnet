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
using BaSyx.Models.Connectivity;
using BaSyx.Models.AdminShell;
using System.Collections.Generic;
using BaSyx.Utils.ResultHandling.ResultTypes;
using BaSyx.Utils.ResultHandling;

namespace BaSyx.API.ServiceProvider
{
    public interface ISubmodelRepositoryServiceProvider : IServiceProvider<IEnumerable<ISubmodel>, ISubmodelRepositoryDescriptor>, ISubmodelRepositoryInterface
    {
        ISubmodelServiceProviderRegistry SubmodelProviderRegistry { get; }

        IResult<PagedResult<IElementContainer<ISubmodel>>> RetrieveSubmodelsMetadata(int limit = 100, string cursor = "");

        IResult<PagedResult<IEnumerable<IReference>>> RetrieveSubmodelsReference(int limit = 100, string cursor = "");

    }
}
