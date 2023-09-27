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

namespace BaSyx.Models.Connectivity
{
    public enum InterfaceName
    {
        [EnumMember(Value = "AAS")]
        AssetAdministrationShellInterface,
        [EnumMember(Value = "AAS-DISCOVERY")]
        AssetAdministrationShellBasicDiscoveryInterface,
        [EnumMember(Value = "AAS-REPOSITORY")]
        AssetAdministrationShellRepositoryInterface, 
        [EnumMember(Value = "AAS-SERIALIZE")]
        AssetAdministrationShellSerializationInterface,
        [EnumMember(Value = "AAS-REGISTRY")]
        AssetAdministrationRegistryInterface,
        [EnumMember(Value = "AASX-FILE")]
        AASXFileServerInterface,
        [EnumMember(Value = "SUBMODEL-REPOSITORY")]
        SubmodelRepositoryInterface,
        [EnumMember(Value = "SUBMODEL")]
        SubmodelInterface,
        [EnumMember(Value = "SUBMODEL-REGISTRY")]
        SubmodelRegistryInterface,
        [EnumMember(Value = "CD-REPOSITORY")]
        ConceptDescriptionRepositoryInterface,

    }
}
