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
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class SubmodelElementCollection_V3_0 : SubmodelElementType_V3_0, IModelType
    {
        [JsonProperty("value"), JsonConverter(typeof(JsonSubmodelElementConverter_V3_0))]
        [XmlArray("value")]
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
		public List<SubmodelElementType_V3_0> Value { get; set; }
             
        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.SubmodelElementCollection;

        public SubmodelElementCollection_V3_0() { }
        public SubmodelElementCollection_V3_0(SubmodelElementType_V3_0 submodelElementType) : base(submodelElementType) { }

        /*
        public bool ShouldSerializeValue()
        {
            if (Value == null || Value.Count == 0)
                return false;
            else
                return true;
        }
        */
    }
}
