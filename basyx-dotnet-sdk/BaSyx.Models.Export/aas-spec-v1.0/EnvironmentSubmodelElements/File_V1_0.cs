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
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class File_V1_0 : SubmodelElementType_V1_0
    {
        [JsonProperty("mimeType")]
        [XmlElement("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("value")]
        [XmlElement("value")]
        public string Value { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.File;

        public File_V1_0() { }
        public File_V1_0(SubmodelElementType_V1_0 submodelElementType) : base(submodelElementType) { }
    }
}