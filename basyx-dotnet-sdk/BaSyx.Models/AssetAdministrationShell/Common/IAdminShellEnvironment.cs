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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    public interface IAdminShellEnvironment
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "assetAdministrationShells")]
        IElementContainer<IAssetAdministrationShell> AssetAdministrationShells { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "submodels")]
        IElementContainer<ISubmodel> Submodels { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "conceptDescriptions")]
        IElementContainer<IConceptDescription> ConceptDescriptions { get; }

    }
}
