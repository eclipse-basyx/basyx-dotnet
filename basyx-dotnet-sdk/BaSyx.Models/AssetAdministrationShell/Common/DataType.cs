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
using BaSyx.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract, JsonConverter(typeof(DataTypeConverter))]
    public class DataType : IEquatable<DataType>
    {
        [IgnoreDataMember]
        public DataObjectType DataObjectType { get; internal set; }

        [IgnoreDataMember]
        public Type SystemType { get; internal set; }

        [IgnoreDataMember]
        public bool IsCollection { get; internal set; }

        internal DataType() { }

        [JsonConstructor]
        public DataType(DataObjectType dataObjectType) : this(dataObjectType, false)
        { }

        public DataType(DataObjectType dataObjectType, bool isCollection)
        {
            DataObjectType = dataObjectType ?? throw new ArgumentNullException(nameof(dataObjectType));
            SystemType = GetSystemTypeFromDataType(dataObjectType) ??
                throw new ArgumentOutOfRangeException(nameof(dataObjectType), $"Unable to get system type from DataObjectType '{dataObjectType}'");

            IsCollection = isCollection;         
        }

        public override string ToString()
        {
            return DataObjectType?.ToString();
        }

        public static DataType GetDataTypeFromSystemType(Type type)
        {
            DataType dataType = new DataType();
            Type innerType;
            if (IsGenericList(type))
            {
                dataType.IsCollection = true;
                innerType = type.GetGenericArguments()[0];
            }
            else if (IsArray(type))
            {
                dataType.IsCollection = true;
                innerType = type.GetElementType();
            }
            else if (type.IsEnum)
            {
                dataType.IsCollection = false;
                innerType = typeof(int);
            }
            else if(IsNullable(type))
            {
                dataType.IsCollection = false;
                innerType = Nullable.GetUnderlyingType(type);
            }
            else
            {
                dataType.IsCollection = false;
                innerType = type;
            }
            dataType.SystemType = innerType;

            switch (innerType.FullName)
            {
                case "System.Decimal": dataType.DataObjectType = DataObjectType.Decimal; break;
                case "System.String": dataType.DataObjectType = DataObjectType.String; break;
                case "System.SByte": dataType.DataObjectType = DataObjectType.Int8; break;
                case "System.Int16": dataType.DataObjectType = DataObjectType.Int16; break;
                case "System.Int32": dataType.DataObjectType = DataObjectType.Int32; break;
                case "System.Int64": dataType.DataObjectType = DataObjectType.Int64; break;
                case "System.Byte": dataType.DataObjectType = DataObjectType.UInt8; break;
                case "System.UInt16": dataType.DataObjectType = DataObjectType.UInt16; break;
                case "System.UInt32": dataType.DataObjectType = DataObjectType.UInt32; break;
                case "System.UInt64": dataType.DataObjectType = DataObjectType.UInt64; break;
                case "System.Boolean": dataType.DataObjectType = DataObjectType.Bool; break;
                case "System.Single": dataType.DataObjectType = DataObjectType.Float; break;
                case "System.Double": dataType.DataObjectType = DataObjectType.Double; break;
                case "System.DateTime": dataType.DataObjectType = DataObjectType.DateTime; break;
                case "System.Uri": dataType.DataObjectType = DataObjectType.AnyURI; break;
                default:
                    if (!IsSimpleType(innerType))
                    {
                        dataType.DataObjectType = DataObjectType.AnyType;
                        return dataType;
                    }
                    else
                        return null;
            }
            return dataType;
        }

        public static bool IsGenericList(Type type)
        {
            return (type.IsGenericType && (
				type.GetGenericTypeDefinition() == typeof(List<>) || 
				type.GetGenericTypeDefinition() == typeof(IEnumerable<>)));
        }

        public static bool IsDictionary(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>));
        }

        public static bool IsNullable(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static bool IsArray(Type type)
        {
            return type.IsArray;
        }

        public static Type GetSystemTypeFromDataType(DataObjectType dataObjectType)
        {
            if (dataObjectType == DataObjectType.String)
                return typeof(string);
            if (dataObjectType == DataObjectType.LangString)
                return typeof(string);
            else if (dataObjectType == DataObjectType.Bool)
                return typeof(bool);
            else if (dataObjectType == DataObjectType.Float)
                return typeof(float);
            else if (dataObjectType == DataObjectType.Double)
                return typeof(double);
            else if (dataObjectType == DataObjectType.Decimal)
                return typeof(decimal);
            else if (dataObjectType == DataObjectType.UInt8)
                return typeof(byte);
            else if (dataObjectType == DataObjectType.UInt16)
                return typeof(UInt16);
            else if (dataObjectType == DataObjectType.UInt32)
                return typeof(UInt32);
            else if (dataObjectType == DataObjectType.UInt64)
                return typeof(UInt64);
            else if (dataObjectType == DataObjectType.Int8)
                return typeof(sbyte);
            else if (dataObjectType == DataObjectType.Int16)
                return typeof(Int16);
            else if (dataObjectType == DataObjectType.Int32)
                return typeof(Int32);
            else if (dataObjectType == DataObjectType.Int64)
                return typeof(Int64);
            else if (dataObjectType == DataObjectType.Integer)
                return typeof(decimal);
            else if (dataObjectType == DataObjectType.NegativeInteger)
                return typeof(decimal);
            else if (dataObjectType == DataObjectType.PositiveInteger)
                return typeof(decimal);
            else if (dataObjectType == DataObjectType.NonNegativeInteger)
                return typeof(decimal);
            else if (dataObjectType == DataObjectType.NonPositiveInteger)
                return typeof(decimal);
            else if (dataObjectType == DataObjectType.AnyType)
                return typeof(object);
            else if (dataObjectType == DataObjectType.AnySimpleType)
                return typeof(string);
            else if (dataObjectType == DataObjectType.DateTime)
                return typeof(DateTime);
            else if (dataObjectType == DataObjectType.DateTimeStamp)
                return typeof(DateTime);
            else if (dataObjectType == DataObjectType.AnyURI)
                return typeof(Uri);
            else if (dataObjectType == DataObjectType.Base64Binary)
                return typeof(byte[]);
            else if (dataObjectType == DataObjectType.HexBinary)
                return typeof(byte[]);
            else if (dataObjectType == DataObjectType.Duration)
                return typeof(TimeSpan);
            else if (dataObjectType == DataObjectType.DayTimeDuration)
                return typeof(TimeSpan);
            else if (dataObjectType == DataObjectType.YearMonthDuration)
                return typeof(TimeSpan);
            else if (dataObjectType == DataObjectType.None)
                return typeof(object);
            else
                return null;
        }

        public static JsonValueKind GetJsonValueKind(DataObjectType dataObjectType)
        {
            if (dataObjectType == DataObjectType.String)
                return JsonValueKind.String;
            if (dataObjectType == DataObjectType.LangString)
                return JsonValueKind.String;
            else if (dataObjectType == DataObjectType.Bool)
                return JsonValueKind.True;
            else if (dataObjectType == DataObjectType.Float)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.Double)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.Decimal)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.UInt8)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.UInt16)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.UInt32)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.UInt64)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.Int8)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.Int16)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.Int32)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.Int64)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.Integer)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.NegativeInteger)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.PositiveInteger)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.NonNegativeInteger)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.NonPositiveInteger)
                return JsonValueKind.Number;
            else if (dataObjectType == DataObjectType.AnyType)
                return JsonValueKind.Object;
            else if (dataObjectType == DataObjectType.AnySimpleType)
                return JsonValueKind.Object;
            else if (dataObjectType == DataObjectType.DateTime)
                return JsonValueKind.String;
            else if (dataObjectType == DataObjectType.DateTimeStamp)
                return JsonValueKind.String;
            else if (dataObjectType == DataObjectType.AnyURI)
                return JsonValueKind.String;
            else if (dataObjectType == DataObjectType.Base64Binary)
                return JsonValueKind.String;
            else if (dataObjectType == DataObjectType.HexBinary)
                return JsonValueKind.String;
            else if (dataObjectType == DataObjectType.Duration)
                return JsonValueKind.String;
            else if (dataObjectType == DataObjectType.DayTimeDuration)
                return JsonValueKind.String;
            else if (dataObjectType == DataObjectType.YearMonthDuration)
                return JsonValueKind.String;
            else if (dataObjectType == DataObjectType.None)
                return JsonValueKind.String;
            else
                return JsonValueKind.Null;
        }

        public static Type GetSystemTypeFromDataType(DataType dataType)
        {
            if(dataType.IsCollection)
            {
                Type outerTypeDefinition = typeof(List<>).GetGenericTypeDefinition();
                Type containerType = outerTypeDefinition.MakeGenericType(dataType.SystemType);
                return containerType;
            }
            else
                return GetSystemTypeFromDataType(dataType.DataObjectType);
        }

        public static bool IsComplexType(Type type) => !IsSimpleType(type);
        public static bool IsSimpleType(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsSimpleType(typeInfo.GetGenericArguments()[0]);

            return typeInfo.IsEnum || typeInfo.IsPrimitive || type.Equals(typeof(string)) || type.Equals(typeof(decimal));
        }
        #region IEquatable Interface Implementation
        public bool Equals(DataType other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.DataObjectType.Equals(other.DataObjectType)
                   && this.IsCollection.Equals(other.IsCollection);
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

            return obj.GetType() == GetType() && Equals((DataType)obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ DataObjectType.GetHashCode();
                result = (result * 397) ^ (IsCollection ? 1 : 0);
                return result;
            }
        }

        public static bool operator ==(DataType x, DataType y)
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

            return x.DataObjectType == y.DataObjectType && x.IsCollection == y.IsCollection;
        }
        public static bool operator !=(DataType x, DataType y)
        {
            return !(x == y);
        }
        #endregion

        public static implicit operator Type(DataType dataType)
        {
            if(dataType.IsCollection)
            {
                Type outerTypeDefinition = typeof(List<>).GetGenericTypeDefinition();
                Type containerType = outerTypeDefinition.MakeGenericType(dataType.SystemType);
                return containerType;
            }
            return dataType.SystemType;
        }

        public static implicit operator DataType(Type type)
        {
            return GetDataTypeFromSystemType(type);
        }

        public static implicit operator string(DataType dataType) => dataType.ToString();

        public static implicit operator DataType(string dataObjectType) => new DataType(dataObjectType);
    }
}
