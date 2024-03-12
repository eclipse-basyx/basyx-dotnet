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
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class SubmodelElementType_V3_0 : EnvironmentReferable_V3_0, IModelType
    {
        [JsonProperty("kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("kind")]
        public ModelingKind Kind { get; set; }

        [JsonProperty("semanticId")]
        [XmlElement("semanticId")]
        public EnvironmentReference_V3_0 SemanticId { get; set; }

        [JsonProperty("qualifiers"), JsonConverter(typeof(JsonQualifierConverter_V3_0))]
        [XmlElement(ElementName = "qualifier")]
        public List<EnvironmentConstraint_V3_0> Qualifier { get; set; } = new List<EnvironmentConstraint_V3_0>();

        [XmlIgnore]
        public virtual ModelType ModelType { get; }

        public SubmodelElementType_V3_0() { }
        public SubmodelElementType_V3_0(SubmodelElementType_V3_0 submodelElementType)
        {
            this.Category = submodelElementType.Category;
            this.Description = submodelElementType.Description;
            this.IdShort = submodelElementType.IdShort;
            this.Kind = submodelElementType.Kind;
            this.Parent = submodelElementType.Parent;
            this.Qualifier = submodelElementType.Qualifier;
            this.SemanticId = submodelElementType.SemanticId;
        }

        public bool ShouldSerializeSemanticId()
        {
            if (SemanticId == null || SemanticId.Keys?.Count == 0)
                return false;
            else
                return true;
        }

        public bool ShouldSerializeQualifier()
        {
            if (Qualifier == null || Qualifier.Count == 0)
                return false;
            else
                return true;
        }
    }
}
