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

namespace BaSyx.API.ServiceProvider
{
    internal sealed class InternalSubmodelServiceProvider : SubmodelServiceProvider
    {
        internal InternalSubmodelServiceProvider(ISubmodel submodel) : base(submodel)
        {
            UseInMemorySubmodelElementHandler();
        }
    }
}
