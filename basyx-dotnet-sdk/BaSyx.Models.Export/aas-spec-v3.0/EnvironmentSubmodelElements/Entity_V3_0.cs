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
using BaSyx.Models.AdminShell;
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class Entity_V3_0 : SubmodelElementType_V3_0
    {
        [JsonProperty("entityType")]
        [XmlElement("entityType")]
        public EnvironmentEntityType_V3_0 EntityType { get; set; }

        [JsonProperty("globalAssetId")]
        [XmlElement("globalAssetId")]
        public string GlobalAssetId { get; set; }

        [JsonProperty("specificAssetIds")]
        [XmlArray("specificAssetIds")]
        [XmlArrayItem("specificAssetId")]
        public List<EnvironmentSpecificAssetId_V3_0> SpecificAssetIds { get; set; } = new List<EnvironmentSpecificAssetId_V3_0>();

        [JsonProperty("statements"), JsonConverter(typeof(JsonSubmodelElementConverter_V3_0))]
        [XmlArray("statements")]
		[XmlArrayItem(ElementName = "property", Type = typeof(Property_V3_0))]
		[XmlArrayItem(ElementName = "multiLanguageProperty", Type = typeof(MultiLanguageProperty_V3_0))]
		[XmlArrayItem(ElementName = "file", Type = typeof(File_V3_0))]
		[XmlArrayItem(ElementName = "blob", Type = typeof(Blob_V3_0))]
		[XmlArrayItem(ElementName = "capability", Type = typeof(Capability_V3_0))]
		[XmlArrayItem(ElementName = "eventElement", Type = typeof(EventElement_V3_0))]
		[XmlArrayItem(ElementName = "basicEventElement", Type = typeof(BasicEventElement_V3_0))]
		[XmlArrayItem(ElementName = "range", Type = typeof(Range_V3_0))]
		[XmlArrayItem(ElementName = "entity", Type = typeof(Entity_V3_0))]
		[XmlArrayItem(ElementName = "referenceElement", Type = typeof(ReferenceElement_V3_0))]
		[XmlArrayItem(ElementName = "annotatedRelationshipElement", Type = typeof(AnnotatedRelationshipElement_V3_0))]
		[XmlArrayItem(ElementName = "relationshipElement", Type = typeof(RelationshipElement_V3_0))]
		[XmlArrayItem(ElementName = "submodelElementCollection", Type = typeof(SubmodelElementCollection_V3_0))]
		[XmlArrayItem(ElementName = "submodelElementList", Type = typeof(SubmodelElementList_V3_0))]
		[XmlArrayItem(ElementName = "operation", Type = typeof(Operation_V3_0))]
		public List<SubmodelElementType_V3_0> Statements { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.Entity;

        public Entity_V3_0() { }
        public Entity_V3_0(SubmodelElementType_V3_0 submodelElementType) : base(submodelElementType) { }
    }

    public enum EnvironmentEntityType_V3_0
    {
        [EnumMember(Value = "CoManagedEntity")]
        CoManagedEntity,
        [EnumMember(Value = "SelfManagedEntity")]
        SelfManagedEntity
    }
}
