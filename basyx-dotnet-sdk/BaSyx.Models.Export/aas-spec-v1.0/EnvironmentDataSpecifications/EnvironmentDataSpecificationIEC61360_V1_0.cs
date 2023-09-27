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

namespace BaSyx.Models.Export.EnvironmentDataSpecifications
{
    [XmlType("dataSpecificationIEC61360", Namespace = AssetAdministrationShellEnvironment_V1_0.IEC61360_NAMESPACE)]
    public class EnvironmentDataSpecificationIEC61360_V1_0
    {
        [JsonProperty("preferredName")]
        [XmlArray("preferredName")]
        [XmlArrayItem("langString", Namespace = AssetAdministrationShellEnvironment_V1_0.AAS_NAMESPACE)]
        public LangStringSet PreferredName { get; set; }

        [JsonProperty("shortName")]
        [XmlElement("shortName")]
        public string ShortName { get; set; }

        [JsonProperty("unit")]
        [XmlElement("unit")]
        public string Unit { get; set; }

        [JsonProperty("unitId")]
        [XmlElement("unitId")]
        public EnvironmentReference_V1_0 UnitId { get; set; }

        [JsonProperty("valueFormat")]
        [XmlElement("valueFormat")]
        public string ValueFormat { get; set; }

        [JsonProperty("sourceOfDefinition")]
        [XmlArray("sourceOfDefinition")]
        [XmlArrayItem("langString", Namespace = AssetAdministrationShellEnvironment_V1_0.AAS_NAMESPACE)]
        public LangStringSet SourceOfDefinition { get; set; }

        [JsonProperty("symbol")]
        [XmlElement("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("dataType")]
        [XmlElement("dataType")]
        public string DataType { get; set; }

        [JsonProperty("definition")]
        [XmlArray("definition")]
        [XmlArrayItem("langString", Namespace = AssetAdministrationShellEnvironment_V1_0.AAS_NAMESPACE)]
        public LangStringSet Definition { get; set; }

        public bool ShouldSerializeSourceOfDefinition()
        {
            if (SourceOfDefinition == null || SourceOfDefinition.Count == 0)
                return false;
            else
                return true;
        }

        public bool ShouldSerializeDefinition()
        {
            if (Definition == null || Definition.Count == 0)
                return false;
            else
                return true;
        }
    }
}
