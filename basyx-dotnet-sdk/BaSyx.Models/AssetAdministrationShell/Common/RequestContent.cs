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
    public enum RequestContent : int
    {
        [EnumMember(Value = "normal")]
        Normal = 0,
        [EnumMember(Value = "trimmed")]
        Trimmed = 1,
        [EnumMember(Value = "value")]
        Value = 2,
        [EnumMember(Value = "reference")]
        Reference = 3,
        [EnumMember(Value = "path")]
        Path = 4
    }
}
