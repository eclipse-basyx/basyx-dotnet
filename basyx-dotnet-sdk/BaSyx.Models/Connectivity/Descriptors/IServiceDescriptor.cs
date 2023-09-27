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
using System.Runtime.Serialization;

namespace BaSyx.Models.Connectivity
{
    public interface IServiceDescriptor : IDescriptor
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "idShort")]
        string IdShort { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "identification")]
        Identifier Identification { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "administration")]
        AdministrativeInformation Administration { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "description")]
        LangStringSet Description { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "displayName")]
        LangStringSet DisplayName { get; }
    }
}
