/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.Attributes;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.AssetAdministrationShell;

namespace BaSyx.Models.Extensions.Semantics.DataSpecifications
{
    [DataContract, DataSpecification("http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0")]
    public class DataSpecificationIEC61360 : IEmbeddedDataSpecification
    {
        public IReference HasDataSpecification => new Reference(
            new GlobalKey(KeyElements.GlobalReference, KeyType.IRI, "http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0"));
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
        public List<ValueReferencePair> ValueList { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueId")]
        public IReference ValueId { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public object Value { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "levelTypes")]
        public List<LevelType> LevelTypes { get; set; }

        public DataSpecificationIEC61360Content()
        {
            PreferredName = new LangStringSet();
            Definition = new LangStringSet();
            ShortName = new LangStringSet();
            ValueList = new List<ValueReferencePair>();
            LevelTypes = new List<LevelType>();
        }
    }

    [DataContract]
    public enum LevelType
    {
        [EnumMember(Value = "Undefined")]
        Undefined,
        [EnumMember(Value = "Min")]
        Min,
        [EnumMember(Value = "Max")]
        Max,
        [EnumMember(Value = "Nom")]
        Nom,
        [EnumMember(Value = "Typ")]
        Typ
    }

    [DataContract]
    public enum DataTypeIEC61360
    {
        [EnumMember(Value = "UNDEFINED")]
        UNDEFINED,
        [EnumMember(Value = "DATE")]
        DATE,
        [EnumMember(Value = "STRING")]
        STRING,
        [EnumMember(Value = "STRING_TRANSLATABLE")]
        STRING_TRANSLATABLE,
        [EnumMember(Value = "INTEGER")]
        INTEGER,
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
