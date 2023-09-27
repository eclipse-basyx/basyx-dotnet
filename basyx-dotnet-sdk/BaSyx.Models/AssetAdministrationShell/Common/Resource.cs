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
    public class Resource
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "path")]
        public string Path { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "contentType")]
        public string ContentType { get; set; }
    }
}
