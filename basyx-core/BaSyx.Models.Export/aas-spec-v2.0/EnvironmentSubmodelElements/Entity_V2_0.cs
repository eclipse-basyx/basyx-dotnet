/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.Common;
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

        [JsonProperty("statements")]
        [XmlElement("statement")]
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
