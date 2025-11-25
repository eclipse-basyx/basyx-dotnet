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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// A submodel element collection is a set or list of submodel elements.
    /// </summary>
    public interface ISubmodelElementCollection : ISubmodelElement<SubmodelElementCollectionValue>, IGetSet
    {
        [IgnoreDataMember, JsonIgnore]
        ISubmodelElement this[string idShort] { get; set; }

        [IgnoreDataMember, JsonIgnore]
        ISubmodelElement this[int i] { get; set; }
    }
}
