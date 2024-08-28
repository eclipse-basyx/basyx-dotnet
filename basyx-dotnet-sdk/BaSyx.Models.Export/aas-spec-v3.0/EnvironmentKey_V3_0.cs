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
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    [XmlType("key")]
    public class EnvironmentKey_V3_0
    {
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("type")]
        public KeyElements_V3_0 Type { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "value")]
		[XmlElement("value")]
		public string Value { get; set; }

    }

    public enum KeyElements_V3_0
    {
        Undefined,
        GlobalReference,
        FragmentReference,
        AnnotatedRelationshipElement,
        BasicEventElement,
        Blob,
        Capability,
        ConceptDictionary,
        DataElement,
        File,
        Entity,
        Event,
        MultiLanguageProperty,
        Operation,
        Property,
        Range,
        ReferenceElement,
        RelationshipElement,
        SubmodelElement,
        SubmodelElementCollection,
        SubmodelElementList,
        AssetAdministrationShell,
        ConceptDescription,
        Submodel
    }
}
