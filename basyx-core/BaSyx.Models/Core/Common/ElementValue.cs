/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract]
    public class ElementValue : IValue
    {
        internal object _value;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        public object Value
        {
            get => _value;
            set
            {
                ValueChanged?.Invoke(this, new ValueChangedArgs(null, value, ValueType));
                _value = value;
            }
        }
        public DataType ValueType { get; protected set; }

        public event EventHandler<ValueChangedArgs> ValueChanged;

        [JsonConstructor]
        public ElementValue(object value, DataType valueType)
        {
            Value = value;
            ValueType = valueType;
        }

        public object ToObject(Type type)
        {
            if (Value == null || type == null)
                return null;

            try
            {
                Value = Convert.ChangeType(Value, type, CultureInfo.InvariantCulture);
                return Value;
            }
            catch
            {
                try
                {
                    JToken jVal = JToken.Parse(Value.ToString());
                    object convertedVal = jVal.ToObject(type);
                    return convertedVal;
                }
                catch
                {
                    throw new InvalidCastException("Cannot convert " + Value?.GetType() + " to " + type.Name);
                }
            }
        }

        public T ToObject<T>()
        {
            if (Value == null)
                return default;
            else if (Value is T)
                return (T)Value;
            else
            {
                try
                {
                    Value = Convert.ChangeType(Value, typeof(T), CultureInfo.InvariantCulture);
                    return (T)Value;
                }
                catch
                {
                    try
                    {
                        JToken jVal = JToken.Parse(Value.ToString());
                        T convertedVal = jVal.ToObject<T>();
                        return convertedVal;
                    }
                    catch
                    {
                        throw new InvalidCastException("Cannot convert " + Value?.GetType() + " to " + typeof(T).Name);
                    }
                }
            }
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
    }
}
