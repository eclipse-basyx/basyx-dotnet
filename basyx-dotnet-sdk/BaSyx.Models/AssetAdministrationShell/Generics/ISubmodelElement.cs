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
    public interface ISubmodelElement : IHasSemantics, IQualifiable, IReferable, IHasKind, IModelElement, IHasDataSpecification, IValueChanged, IGetSet
    {
        [JsonIgnore, IgnoreDataMember]
        ValueScope Value { get; }
    }

    public interface ISubmodelElement<TValueScope> : ISubmodelElement where TValueScope: ValueScope
    {
        [JsonIgnore, IgnoreDataMember]
        new TValueScope Value { get; }
    }
}
