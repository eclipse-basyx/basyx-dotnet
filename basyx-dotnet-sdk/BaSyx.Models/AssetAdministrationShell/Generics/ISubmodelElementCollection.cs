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

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// A submodel element collection is a set or list of submodel elements.
    /// </summary>
    public interface ISubmodelElementCollection : ISubmodelElement<SubmodelElementCollectionValue>, IGetSet
    {

    }
}
