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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.Semantics
{
    [DataContract, DataSpecification("http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0")]
    public class DataSpecificationIEC61360 : IEmbeddedDataSpecification
    {
        public IReference DataSpecification => new Reference(
            new Key(KeyType.GlobalReference, "http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0"));
        [DataSpecificationContent(typeof(DataSpecificationIEC61360Content), "IEC61360")]
        public IDataSpecificationContent DataSpecificationContent { get; set; }

        public DataSpecificationIEC61360(DataSpecificationIEC61360Content content)
        {
            DataSpecificationContent = content;
        }
    }

    [DataContract]
    public class DataSpecificationIEC61360Content : IDataSpecificationContent
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "dataType")]
        public DataTypeIEC61360 DataType { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "definition")]
        public LangStringSet Definition { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "preferredName")]
        public LangStringSet PreferredName { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "shortName")]
        public LangStringSet ShortName { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "sourceOfDefinition")]
        public string SourceOfDefinition { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "symbol")]
        public string Symbol { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "unit")]
        public string Unit { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "unitId")]
        public IReference UnitId { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueFormat")]
        public string ValueFormat { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueList")]
        public ValueList ValueList { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueId")]
        public IReference ValueId { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public object Value { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "levelType")]
        public LevelType LevelType { get; set; }

        public ModelType ModelType => ModelType.DataSpecificationIec61360;

        public DataSpecificationIEC61360Content()
        {
            PreferredName = new LangStringSet();
            Definition = new LangStringSet();
            ShortName = new LangStringSet();
        }
    }

    [DataContract]
    public class ValueList
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueReferencePairs")]
        public List<ValueReferencePair> ValueReferencePairs { get; set; } = new List<ValueReferencePair>();
    }

    [DataContract]
    public class LevelType
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "min")]
        public bool Min { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "max")]
        public bool Max { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "nom")]
        public bool Nom { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "typ")]
        public bool Typ { get; set; }
    }

    [DataContract]
    public enum DataTypeIEC61360
    {
        [EnumMember(Value = "UNDEFINED")]
        UNDEFINED,
        [EnumMember(Value = "DATE")]
        DATE,
        [EnumMember(Value = "FILE")]
        FILE,
        [EnumMember(Value = "HTML")]
        HTML,
        [EnumMember(Value = "IRDI")]
        IRDI,
        [EnumMember(Value = "IRI")]
        IRI,
        [EnumMember(Value = "STRING")]
        STRING,
        [EnumMember(Value = "STRING_TRANSLATABLE")]
        STRING_TRANSLATABLE,
        [EnumMember(Value = "INTEGER_COUNT")]
        INTEGER_COUNT,
        [EnumMember(Value = "INTEGER_MEASURE")]
        INTEGER_MEASURE,
        [EnumMember(Value = "INTEGER_CURRENCY")]
        INTEGER_CURRENCY,
        [EnumMember(Value = "REAL_MEASURE")]
        REAL_MEASURE,
        [EnumMember(Value = "REAL_COUNT")]
        REAL_COUNT,
        [EnumMember(Value = "REAL_CURRENCY")]
        REAL_CURRENCY,
        [EnumMember(Value = "BLOB")]
        BLOB,
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
        [EnumMember(Value = "TIMESTAMP")]
        TIMESTAMP
    }

    /// <summary>
    /// A value reference pair within a value list. Each value has a global unique id defining its semantic.
    /// </summary>
    [DataContract]
    public class ValueReferencePair
    {
        /// <summary>
        /// the value of the referenced concept definition of the value in valueId. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public object Value { get; set; }

        /// <summary>
        /// Global unique id of the value.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueId")]
        public IReference ValueId { get; set; }
    }

}
