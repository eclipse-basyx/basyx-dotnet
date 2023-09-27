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
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class Entity_V2_0 : SubmodelElementType_V2_0
    {
        [JsonProperty("entityType")]
        [XmlElement("entityType")]
        public EnvironmentEntityType EntityType { get; set; }

        [JsonProperty("statements"), JsonConverter(typeof(JsonSubmodelElementConverter_V2_0))]
        [XmlArray("statements")]
        [XmlArrayItem("submodelElement")]
        public List<EnvironmentSubmodelElement_V2_0> Statements { get; set; }

        [JsonProperty("asset")]
        [XmlElement("assetRef")]
        public EnvironmentReference_V2_0 AssetReference { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.Entity;

        public Entity_V2_0() { }
        public Entity_V2_0(SubmodelElementType_V2_0 submodelElementType) : base(submodelElementType) { }
    }

    public enum EnvironmentEntityType
    {
        [EnumMember(Value = "CoManagedEntity")]
        CoManagedEntity,
        [EnumMember(Value = "SelfManagedEntity")]
        SelfManagedEntity
    }
}
