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
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    ///<inheritdoc cref="IProperty"/>
    [DataContract]
    public class Property : SubmodelElement, IProperty
    {
        public override ModelType ModelType => ModelType.Property;

        /// <summary>
        /// Only internal temporary storage of the current value. 
        /// Get and Set operations shall only be processed via its respective handler.
        /// </summary>
        protected object _value;

        public virtual object Value
        {
            get
            {
                return Get?.Invoke(this)?.Value;
            }            
            set
            {
                if (value != null)
                {
                    if (value is IValue iValue)
                        Set?.Invoke(this, iValue);
                    else
                        Set?.Invoke(this, new ElementValue(value, ValueType));                   
                }
            }
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueType")]
        public virtual DataType ValueType { get; set; }
        public IReference ValueId { get; set; }        

        public Property(string idShort) : this(idShort, null, null)
        { }

        public Property(string idShort, DataType valueType) : this(idShort, valueType, null)
        { }

        [JsonConstructor]
        public Property(string idShort, DataType valueType, object value) : base(idShort)
        {
            ValueType = valueType;

            if (value != null)
            {
                if (ValueType == null)
                {
                    _value = value;
                    ValueType = new DataType(DataObjectType.None);
                }
                else if (value.GetType() == valueType.SystemType)
                    _value = value;
                else
                    _value = ElementValue.ToObject(value, valueType.SystemType);
            }

            Get = element  => 
            { 
                return new ElementValue(_value, ValueType); 
            };

            Set = (element, iValue) => 
            { 
                _value = iValue.Value;
                OnValueChanged(new ValueChangedArgs(IdShort, _value, ValueType));
            };
        }

        public T ToObject<T>()
        {
            return new ElementValue(Value, ValueType).ToObject<T>();
        }

        public object ToObject(Type type)
        {
            return new ElementValue(Value, ValueType).ToObject(type);
        }        
    }
    ///<inheritdoc cref="IProperty"/>
    [DataContract]
    public class Property<TInnerType> : Property, IProperty<TInnerType>
    {
        public override ModelType ModelType => ModelType.Property;
        public override DataType ValueType => typeof(TInnerType);
       
        [JsonIgnore, IgnoreDataMember]
        public virtual new TInnerType Value
        {
            get
            {
                if (Get != null)
                    return Get.Invoke(this);
                else
                    return default;
            }
            set
            {
                if (value != null)
                {
                    Set?.Invoke(this, value);                                       
                }
            }
        }

        private GetValueHandler<TInnerType> _get;
        private SetValueHandler<TInnerType> _set;

        [JsonIgnore, IgnoreDataMember]
        public new GetValueHandler<TInnerType> Get 
        {
            get => _get;
            set
            {
                _get = value;
                if (value != null)
                    base.Get = new GetValueHandler(element => new ElementValue<TInnerType>(_get.Invoke(element)));
                else
                    base.Get = null;
            }
        }
        [JsonIgnore, IgnoreDataMember]
        public new SetValueHandler<TInnerType> Set 
        {
            get => _set;
            set
            {
                _set = value;
                if (value != null)
                    base.Set = new SetValueHandler((element, iValue) =>
                    {
                        TInnerType typedValue = iValue.ToObject<TInnerType>();
                        _set.Invoke(element, typedValue);
                        OnValueChanged(new ValueChangedArgs(IdShort, typedValue, ValueType));
                    });
                else
                    base.Set = null;
            }
        }

        public Property(string idShort) : this(idShort, default) { }

        [JsonConstructor]
        public Property(string idShort, TInnerType value) : base(idShort, typeof(TInnerType), value) 
        {
            _get = element =>
            {
                if (base.Get != null)
                    return base.Get.Invoke(element).ToObject<TInnerType>();
                else
                    return default;
            };
            _set = (element, iValue) => { base.Set?.Invoke(element, new ElementValue<TInnerType>(iValue)); };
        }

    }
}
