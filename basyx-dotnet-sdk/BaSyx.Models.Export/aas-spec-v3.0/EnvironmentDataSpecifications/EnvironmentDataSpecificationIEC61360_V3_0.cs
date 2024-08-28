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
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export.EnvironmentDataSpecifications
{
    [XmlType("dataSpecificationIec61360")]
    public class EnvironmentDataSpecificationIEC61360_V3_0
    {
        [JsonProperty("preferredName")]
        [XmlArray("preferredName")]
        [XmlArrayItem("langStringPreferredNameTypeIec61360")]
        public List<EnvironmentLangString_V3_0> PreferredName { get; set; }

        [JsonProperty("shortName")]
        [XmlArray("shortName")]
        [XmlArrayItem("langStringShortNameTypeIec61360")]
        public List<EnvironmentLangString_V3_0> ShortName { get; set; }

        [JsonProperty("unit")]
        [XmlElement("unit")]
        public string Unit { get; set; }

        [JsonProperty("unitId")]
        [XmlElement("unitId")]
        public EnvironmentReference_V3_0 UnitId { get; set; }      

        [JsonProperty("sourceOfDefinition")]
        [XmlElement("sourceOfDefinition")]
        public string SourceOfDefinition { get; set; }

        [JsonProperty("symbol")]
        [XmlElement("symbol")]
        public string Symbol { get; set; }

        private EnvironmentDataTypeIEC61360_V3_0 _dataType;

        [JsonProperty("dataType")]
        [XmlElement("dataType")]
        public string DataTypeAsString
        {
            get
            {
                if (DataType == EnvironmentDataTypeIEC61360_V3_0.UNDEFINED)
                    return null;
                else
                    return DataType.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DataType = EnvironmentDataTypeIEC61360_V3_0.UNDEFINED;
                }
                else
                {
                    if (Enum.TryParse<EnvironmentDataTypeIEC61360_V3_0>(value, out EnvironmentDataTypeIEC61360_V3_0 dataType))
                        DataType = dataType;
                    else
                        DataType = EnvironmentDataTypeIEC61360_V3_0.UNDEFINED;
                }
            }
        }


        [JsonIgnore]
        [XmlIgnore]
        public EnvironmentDataTypeIEC61360_V3_0 DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        [JsonProperty("definition")]
        [XmlArray("definition")]
        [XmlArrayItem("langStringDefinitionTypeIec61360")]
        public List<EnvironmentLangString_V3_0> Definition { get; set; }

        [JsonProperty("valueFormat")]
        [XmlElement("valueFormat")]
        public string ValueFormat { get; set; }

        [JsonProperty("valueList"), JsonConverter(typeof(JsonValueListConverter_V3_0))]
        [XmlArray("valueList")]
        [XmlArrayItem("valueReferencePair")]
        public List<ValueReferencePair_V3_0> ValueList { get; set; }

        [JsonProperty("value")]
        [XmlElement("value")]
        public object Value { get; set; }

        [JsonProperty("levelType")]
        [XmlElement("levelType")]
        public EnvironmentLevelType_V3_0 LevelType { get; set; }

        public bool ShouldSerializeUnitId()
        {
            if (UnitId == null || UnitId.Keys?.Count == 0)
                return false;
            else
                return true;
        }

        public bool ShouldSerializeValueList()
        {
            if (ValueList == null || ValueList.Count == 0)
                return false;
            else
                return true;
        }

        public bool ShouldSerializeShortName()
        {
            if (ShortName == null || ShortName.Count == 0)
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

    public class ValueReferencePair_V3_0
    {
        [JsonProperty("value")]
        [XmlElement("value")]
        public object Value { get; set; }

        [JsonProperty("valueId")]
        [XmlElement("valueId")]
        public EnvironmentReference_V3_0 ValueId { get; set; }
    }
   
    public class EnvironmentLevelType_V3_0
    {
		[JsonProperty("min")]
		[XmlElement("min")]
		public bool Min { get; set; }

		[JsonProperty("nom")]
		[XmlElement("nom")]
		public bool Nom { get; set; }

		[JsonProperty("typ")]
		[XmlElement("typ")]
		public bool Typ { get; set; }

		[JsonProperty("max")]
		[XmlElement("max")]
		public bool Max { get; set; }
	}

    public enum EnvironmentDataTypeIEC61360_V3_0
    {
        [EnumMember(Value = "UNDEFINED")]
        UNDEFINED,
        [EnumMember(Value = "DATE")]
        DATE,
        [EnumMember(Value = "STRING")]
        STRING,
        [EnumMember(Value = "STRING_TRANSLATABLE")]
        STRING_TRANSLATABLE,
		[EnumMember(Value = "INTEGER_MEASURE")]
		INTEGER_MEASURE,
		[EnumMember(Value = "INTEGER_COUNT")]
		INTEGER_COUNT,
		[EnumMember(Value = "INTEGER_CURRENCY")]
		INTEGER_CURRENCY,
		[EnumMember(Value = "REAL_MEASURE")]
        REAL_MEASURE,
        [EnumMember(Value = "REAL_COUNT")]
        REAL_COUNT,
        [EnumMember(Value = "REAL_CURRENCY")]
        REAL_CURRENCY,
        [EnumMember(Value = "BOOLEAN")]
        BOOLEAN,
        [EnumMember(Value = "IRI")]
        IRI,
		[EnumMember(Value = "IRDI")]
		IRDI,
		[EnumMember(Value = "RATIONAL")]
        RATIONAL,
        [EnumMember(Value = "RATIONAL_MEASURE")]
        RATIONAL_MEASURE,
        [EnumMember(Value = "TIME")]
        TIME,
        [EnumMember(Value = "TIME_STAMP")]
        TIME_STAMP,
		[EnumMember(Value = "HTML")]
		HTML,
		[EnumMember(Value = "BLOB")]
		BLOB,
		[EnumMember(Value = "FILE")]
		FILE
	}
}
