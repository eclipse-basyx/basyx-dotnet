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
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export.EnvironmentDataSpecifications
{
    [XmlType("dataSpecificationIEC61360", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
    public class EnvironmentDataSpecificationIEC61360_V2_0
    {
        [JsonProperty("preferredName")]
        [XmlArray("preferredName")]
        [XmlArrayItem("langString", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public LangStringSet PreferredName { get; set; }

        [JsonProperty("shortName")]
        [XmlArray("shortName")]
        [XmlArrayItem("langString", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public LangStringSet ShortName { get; set; }

        [JsonProperty("unit")]
        [XmlElement("unit", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public string Unit { get; set; }

        [JsonProperty("unitId")]
        [XmlElement("unitId", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public EnvironmentReference_V2_0 UnitId { get; set; }      

        [JsonProperty("sourceOfDefinition")]
        [XmlElement("sourceOfDefinition", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public string SourceOfDefinition { get; set; }

        [JsonProperty("symbol")]
        [XmlElement("symbol", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public string Symbol { get; set; }

        private EnvironmentDataTypeIEC61360 _dataType;

        [JsonProperty("dataType")]
        [XmlElement("dataType", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public string DataTypeAsString
        {
            get
            {
                if (DataType == EnvironmentDataTypeIEC61360.UNDEFINED)
                    return null;
                else
                    return DataType.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DataType = EnvironmentDataTypeIEC61360.UNDEFINED;
                }
                else
                {
                    if (Enum.TryParse<EnvironmentDataTypeIEC61360>(value, out EnvironmentDataTypeIEC61360 dataType))
                        DataType = dataType;
                    else
                        DataType = EnvironmentDataTypeIEC61360.UNDEFINED;
                }
            }
        }


        [JsonIgnore]
        [XmlIgnore]
        public EnvironmentDataTypeIEC61360 DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        [JsonProperty("definition")]
        [XmlArray("definition")]
        [XmlArrayItem("langString", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public LangStringSet Definition { get; set; }

        [JsonProperty("valueFormat")]
        [XmlElement("valueFormat", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public string ValueFormat { get; set; }

        [JsonProperty("valueList"), JsonConverter(typeof(JsonValueListConverter_V2_0))]
        [XmlArray("valueList")]
        [XmlArrayItem("valueReferencePair", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public List<ValueReferencePair> ValueList { get; set; }

        [JsonProperty("value")]
        [XmlElement("value", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public object Value { get; set; }

        [JsonProperty("valueId")]
        [XmlElement("valueId", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public EnvironmentReference_V2_0 ValueId { get; set; }

        [JsonProperty("levelType")]
        [XmlElement("levelType", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public List<EnvironmentLevelType> LevelTypes { get; set; }

        public bool ShouldSerializeLevelTypes()
        {
            if (LevelTypes == null || LevelTypes.Count == 0)
                return false;
            else
                return true;
        }

        public bool ShouldSerializeValueId()
        {
            if (ValueId == null)
                return false;
            else
                return true;
        }

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

    public class ValueReferencePair
    {
        [JsonProperty("value")]
        [XmlElement("value", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public object Value { get; set; }

        [JsonProperty("valueId")]
        [XmlElement("valueId", Namespace = AssetAdministrationShellEnvironment_V2_0.IEC61360_NAMESPACE)]
        public EnvironmentReference_V2_0 ValueId { get; set; }

        [JsonProperty("valueType")]
        [XmlIgnore]
        public string ValueType { get; set; }
    }
   
    public enum EnvironmentLevelType
    {
        [EnumMember(Value = "Min")]
        Min,
        [EnumMember(Value = "Max")]
        Max,
        [EnumMember(Value = "Nom")]
        Nom,
        [EnumMember(Value = "Typ")]
        Typ
    }

    public enum EnvironmentDataTypeIEC61360
    {
        [EnumMember(Value = "UNDEFINED")]
        UNDEFINED,
        [EnumMember(Value = "DATE")]
        DATE,
        [EnumMember(Value = "STRING")]
        STRING,
        [EnumMember(Value = "STRING_TRANSLATABLE")]
        STRING_TRANSLATABLE,
        [EnumMember(Value = "REAL_MEASURE")]
        REAL_MEASURE,
        [EnumMember(Value = "REAL_COUNT")]
        REAL_COUNT,
        [EnumMember(Value = "REAL_CURRENCY")]
        REAL_CURRENCY,
        [EnumMember(Value = "BOOLEAN")]
        BOOLEAN,
        [EnumMember(Value = "URL")]
        URL,
        [EnumMember(Value = "RATIONAL")]
        RATIONAL,
        [EnumMember(Value = "RATIONAL_MEASURE")]
        RATIONAL_MEASURE,
        [EnumMember(Value = "TIME")]
        TIME,
        [EnumMember(Value = "TIME_STAMP")]
        TIME_STAMP
    }
}
