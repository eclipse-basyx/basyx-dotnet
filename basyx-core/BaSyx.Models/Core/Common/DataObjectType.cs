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
using BaSyx.Utils.StringOperations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.Common
{
    public enum DataObjectTypes
    {
        None,
        AnyType,
        AnySimpleType,
        UInt8,
        UInt16,
        UInt32,
        UInt64,
        Int8,
        Int16,
        Int32,
        Int64,
        String,
        LangString,
        Integer,
        NonPositiveInteger,
        NonNegativeInteger,
        NegativeInteger,
        PositiveInteger,
        Decimal,
        Double,
        Float,
        Bool,
        Duration,
        DayTimeDuration,
        YearMonthDuration,
        DateTime,
        DateTimeStamp,
        AnyURI,
        Base64Binary,
        HexBinary
    }

    [DataContract]
    public class DataObjectType : IEquatable<DataObjectType>
    {
        public static readonly DataObjectType None = new DataObjectType("none");

        public static readonly DataObjectType AnyType = new DataObjectType("anyType");
        public static readonly DataObjectType AnySimpleType = new DataObjectType("anySimpleType");

        public static readonly DataObjectType UInt8 = new DataObjectType("unsignedByte");
        public static readonly DataObjectType UInt16 = new DataObjectType("unsignedShort");
        public static readonly DataObjectType UInt32 = new DataObjectType("unsignedInt");
        public static readonly DataObjectType UInt64 = new DataObjectType("unsignedLong");

        public static readonly DataObjectType Int8 = new DataObjectType("byte");
        public static readonly DataObjectType Int16 = new DataObjectType("short");
        public static readonly DataObjectType Int32 = new DataObjectType("int");
        public static readonly DataObjectType Int64 = new DataObjectType("long");

        public static readonly DataObjectType String = new DataObjectType("string");
        public static readonly DataObjectType LangString = new DataObjectType("langString");

        public static readonly DataObjectType Integer = new DataObjectType("integer");
        public static readonly DataObjectType NonPositiveInteger = new DataObjectType("nonPositiveInteger");
        public static readonly DataObjectType NonNegativeInteger = new DataObjectType("nonNegativeInteger");
        public static readonly DataObjectType NegativeInteger = new DataObjectType("negativeInteger");
        public static readonly DataObjectType PositiveInteger = new DataObjectType("positiveInteger");

        public static readonly DataObjectType Decimal = new DataObjectType("decimal");
        public static readonly DataObjectType Double = new DataObjectType("double");
        public static readonly DataObjectType Float = new DataObjectType("float");
        public static readonly DataObjectType Bool = new DataObjectType("boolean");

        public static readonly DataObjectType Duration = new DataObjectType("duration");
        public static readonly DataObjectType DayTimeDuration = new DataObjectType("dayTimeDuration");
        public static readonly DataObjectType YearMonthDuration = new DataObjectType("yearMonthDuration");

        public static readonly DataObjectType DateTime = new DataObjectType("dateTime");
        public static readonly DataObjectType DateTimeStamp = new DataObjectType("dateTimeStamp");

        public static readonly DataObjectType AnyURI = new DataObjectType("anyURI");
        public static readonly DataObjectType Base64Binary = new DataObjectType("base64binary");
        public static readonly DataObjectType HexBinary = new DataObjectType("hexBinary");

        private static readonly Dictionary<string, DataObjectType> _dataObjectTypes;
        private static readonly Dictionary<DataObjectTypes, DataObjectType> _enumDataObjectTypes;
        static DataObjectType()
        {
            var fields = typeof(DataObjectType).GetFields(BindingFlags.Public | BindingFlags.Static);
            _dataObjectTypes = fields.ToDictionary(k => ((DataObjectType)k.GetValue(null)).Name, v => ((DataObjectType)v.GetValue(null)));
            _enumDataObjectTypes = fields.ToDictionary(k => (DataObjectTypes)Enum.Parse(typeof(DataObjectTypes), k.Name), v => ((DataObjectType)v.GetValue(null)));
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "name")]
        public string Name { get; }
       
        [JsonConstructor]
        internal DataObjectType(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public static DataObjectType GetDataObjectType(DataObjectTypes dataObjectTypeEnum)
        {
            if (_enumDataObjectTypes.TryGetValue(dataObjectTypeEnum, out DataObjectType dataObjectType))
                return dataObjectType;
            else
                return None;
        }

        public static bool TryParse(string dataObjectTypeString, out DataObjectType dataObjectType)
        {
            dataObjectTypeString = dataObjectTypeString.LowercaseFirst();
            if (_dataObjectTypes.TryGetValue(dataObjectTypeString, out dataObjectType))
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(DataObjectType other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Name.Equals(other.Name);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((DataObjectType)obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ Name.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(DataObjectType x, DataObjectType y)
        {

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }
            if (ReferenceEquals(y, null))
            {
                return false;
            }

            return x.Name == y.Name;
        }
        public static bool operator !=(DataObjectType x, DataObjectType y)
        {
            return !(x == y);
        }

        public static implicit operator Type(DataObjectType dataObjectType)
        {
            return DataType.GetSystemTypeFromDataType(dataObjectType);
        }

        public static implicit operator DataObjectType(Type type)
        {
            return DataType.GetDataTypeFromSystemType(type).DataObjectType;
        }

        public static implicit operator string(DataObjectType dataObjectType) => dataObjectType.ToString();
        public static implicit operator DataObjectType(string dataObjectType) => _dataObjectTypes[dataObjectType.LowercaseFirst()];
    }
}
