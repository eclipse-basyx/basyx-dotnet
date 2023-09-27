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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class ElementValue : IValue
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<ElementValue>();

        internal object _value;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        public object Value
        {
            get => _value;
            set
            {
                if (value != null)
                {
                    if (ValueType == null)
                    {
                        _value = value;
                        ValueType = new DataType(DataObjectType.None);
                    }
                    else if (value.GetType() == ValueType.SystemType)
                        _value = value;
                    else
                        _value = ToObject(value, ValueType.SystemType);

                    ValueChanged?.Invoke(this, new ValueChangedArgs(null, value, ValueType));
                }
            }
        }
        public DataType ValueType { get; protected set; }

        public event EventHandler<ValueChangedArgs> ValueChanged;

        public ElementValue(object value) 
            : this(value, DataType.GetDataTypeFromSystemType(value.GetType()))
        { }

        [JsonConstructor]
        public ElementValue(object value, DataType valueType)
        {
            ValueType = valueType;
            Value = value;
        }

        public static object ToObject(object value, Type type)
        {
            if (value == null || type == null || (value is string s && string.IsNullOrEmpty(s)))
                return null;
            else if (value.GetType() == type)
                return value;
            else if (type == typeof(Uri))
                try { return new Uri(value.ToString()); } catch (Exception uriExc) { 
                    logger.LogError(uriExc, $"Cannot convert from {value?.GetType()} to {type.Name} | value: {value?.ToString()}");
                    throw new InvalidOperationException($"Cannot convert from {value?.GetType()} to {type.Name} | value: {value?.ToString()}", uriExc);
                }
            else
            {
                try
                {
                    value = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
                    return value;
                }
                catch (Exception e1)
                {
                    logger.LogWarning(e1, $"Cannot change type from {value?.GetType()} to {type.Name} | value: {value?.ToString()}");

                    try
                    {
                        JToken jVal = JToken.Parse(value.ToString());
                        object convertedVal = jVal.ToObject(type);
                        return convertedVal;
                    }
                    catch (Exception e2)
                    {
                        logger.LogError(e2, $"Cannot convert from {value?.GetType()} to {type.Name} | value: {value?.ToString()}");
                        throw new InvalidCastException($"Cannot convert from {value?.GetType()} to {type.Name} | value: {value?.ToString()}", e2);
                    }
                }
            }
        }

        public static T ToObject<T>(object value)
        {
            if (value == null || (value is string s && string.IsNullOrEmpty(s)))
                return default;
            else if (value is T tValue)
                return tValue;
            else if (typeof(T) == typeof(Uri))
                try { return (T)Activator.CreateInstance(typeof(Uri), value.ToString()); }
                catch (Exception uriExc)
                {
                    logger.LogError(uriExc, $"Cannot convert from {value?.GetType()} to {typeof(T)} | value: {value?.ToString()}");
                    throw new InvalidOperationException($"Cannot convert from {value?.GetType()} to {typeof(T)} | value: {value?.ToString()}", uriExc);
                }
            else
            {
                try
                {
                    value = Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                    return (T)value;
                }
                catch (Exception e1)
                {
                    logger.LogWarning(e1, $"Cannot change type from {value?.GetType()} to {typeof(T)} | value: {value?.ToString()}");

                    try
                    {
                        JToken jVal = JToken.Parse(value.ToString());
                        T convertedVal = jVal.ToObject<T>();
                        return convertedVal;
                    }
                    catch (Exception e2)
                    {
                        logger.LogError(e2, $"Cannot convert from {value?.GetType()} to {typeof(T)} | value: {value?.ToString()}");
                        throw new InvalidCastException($"Cannot convert from {value?.GetType()} to {typeof(T)} | value: {value?.ToString()}", e2);
                    }
                }
            }
        }

        public object ToObject(Type type)
        {
            return ToObject(Value, type);
        }

        public T ToObject<T>()
        {
            return ToObject<T>(Value);
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DataContract]
    public class ElementValue<TValue> : ElementValue, IValue<TValue>
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public new TValue Value { get; set; }

        public ElementValue() : this(default, DataType.GetDataTypeFromSystemType(typeof(TValue)))
        { }

        public ElementValue(TValue value) : this(value, DataType.GetDataTypeFromSystemType(typeof(TValue)))
        { }

        public ElementValue(TValue value, DataType valueType) : base(value, valueType)
        {
            Value = value;
            ValueType = valueType;
        }

        public static implicit operator ElementValue<TValue>(TValue value)
        {
            return new ElementValue<TValue>(value);
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
