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
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class OperationVariable_V3_0
    {
        [JsonProperty("value")]
        [XmlElement(ElementName = "value")]
        public OperationVariableValue_V3_0 Value { get; set; }
    }

    public class OperationVariableValue_V3_0
    {
        [XmlElement(ElementName = "property", Type = typeof(Property_V3_0))]
        [XmlElement(ElementName = "multiLanguageProperty", Type = typeof(MultiLanguageProperty_V3_0))]
        [XmlElement(ElementName = "file", Type = typeof(File_V3_0))]
        [XmlElement(ElementName = "blob", Type = typeof(Blob_V3_0))]
        [XmlElement(ElementName = "capability", Type = typeof(Capability_V3_0))]
        [XmlElement(ElementName = "eventElement", Type = typeof(EventElement_V3_0))]
        [XmlElement(ElementName = "basicEventElement", Type = typeof(BasicEventElement_V3_0))]
        [XmlElement(ElementName = "range", Type = typeof(Range_V3_0))]
        [XmlElement(ElementName = "entity", Type = typeof(Entity_V3_0))]
        [XmlElement(ElementName = "referenceElement", Type = typeof(ReferenceElement_V3_0))]
        [XmlElement(ElementName = "annotatedRelationshipElement", Type = typeof(AnnotatedRelationshipElement_V3_0))]
        [XmlElement(ElementName = "relationshipElement", Type = typeof(RelationshipElement_V3_0))]
        [XmlElement(ElementName = "submodelElementCollection", Type = typeof(SubmodelElementCollection_V3_0))]
        [XmlElement(ElementName = "submodelElementList", Type = typeof(SubmodelElementList_V3_0))]
        [XmlElement(ElementName = "operation", Type = typeof(Operation_V3_0))]
        public SubmodelElementType_V3_0 Value { get; set; }
    }
}
