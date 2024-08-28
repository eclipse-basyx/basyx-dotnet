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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Extensions
{
    public class PackageDescription
    {
        [DataMember(Name = "aasIds"), XmlIgnore]
        public List<string> AdminShellIds { get; set; } = new List<string>();

        [DataMember(Name = "packageId"), XmlIgnore]
        public string PackageId { get; set; }

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public string FileName { get; set; }

    }
}
