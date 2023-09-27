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
    [DataContract]
    public enum KeyType
    {
        [EnumMember(Value = "Undefined")]
        Undefined,
        [EnumMember(Value = "Custom")]
        Custom,
        [EnumMember(Value = "IRI")]
        IRI,
        [EnumMember(Value = "URI")]
        URI,
        [EnumMember(Value = "IRDI")]
        IRDI,
        [EnumMember(Value = "IdShort")]
        IdShort,
        [EnumMember(Value = "FragmentId")]
        FragmentId
    }
}
