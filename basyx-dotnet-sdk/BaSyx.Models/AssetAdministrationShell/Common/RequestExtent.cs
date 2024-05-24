﻿/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
    public enum RequestExtent : int
    {
        [EnumMember(Value = "withoutBlobValue")]
        WithoutBlobValue = 0,
        [EnumMember(Value = "withBlobValue")]
        WithBlobValue = 1
    }
}