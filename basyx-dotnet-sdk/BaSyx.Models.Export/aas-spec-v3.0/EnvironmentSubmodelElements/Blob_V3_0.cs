/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
    public class Blob_V3_0 : SubmodelElementType_V3_0
    {
        [JsonProperty("contentType")]
        [XmlElement("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("value")]
        [XmlElement("value")]
        public string Value { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.Blob;

        public Blob_V3_0() { }
        public Blob_V3_0(SubmodelElementType_V3_0 submodelElementType) : base(submodelElementType) { }
    }
}