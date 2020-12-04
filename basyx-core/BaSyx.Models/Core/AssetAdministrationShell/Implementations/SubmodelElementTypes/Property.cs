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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.Common;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
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
                return Get.Invoke(this)?.Value;
            }            
            set
            {
                if (value != null)
                {
                    if (value is IValue iValue)
                        Set.Invoke(this, iValue);
                    else
                        Set.Invoke(this, new ElementValue(value, value.GetType()));

                    ValueChanged?.Invoke(this, new ValueChangedArgs(this.IdShort, value, value.GetType()));
                }
            }
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueType")]
        public virtual DataType ValueType { get; set; }
        public IReference ValueId { get; set; }

        public event EventHandler<ValueChangedArgs> ValueChanged;

        public Property(string idShort) : this(idShort, null, null)
        { }

        public Property(string idShort, DataType valueType) : this(idShort, valueType, null)
        { }

        [JsonConstructor]
        public Property(string idShort, DataType valueType, object value) : base(idShort)
        {
            ValueType = valueType;

            _value = value;

            Get = element  => { return new ElementValue(_value, ValueType); };
            Set = (element, iValue) => { _value = iValue.Value; };
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

        public new event EventHandler<ValueChangedArgs> ValueChanged;
        
        [JsonIgnore, IgnoreDataMember]
        public virtual new TInnerType Value
        {
            get
            {
                return Get.Invoke(this);
            }
            set
            {
                if (value != null)
                {
                    Set.Invoke(this, value);
                    
                    ValueChanged?.Invoke(this, new ValueChangedArgs(this.IdShort, value, value.GetType()));
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
                base.Get = new GetValueHandler(element => new ElementValue<TInnerType>(_get.Invoke(element)));
            }
        }
        [JsonIgnore, IgnoreDataMember]
        public new SetValueHandler<TInnerType> Set 
        {
            get => _set;
            set
            {
                _set = value;
                base.Set = new SetValueHandler((element, iValue) => _set.Invoke(element, iValue.ToObject<TInnerType>()));
            }
        }

        public Property(string idShort) : this(idShort, default) { }

        [JsonConstructor]
        public Property(string idShort, TInnerType value) : base(idShort, typeof(TInnerType), value) 
        {
            _get = element => { return base.Get.Invoke(element).ToObject<TInnerType>(); };
            _set = (element, iValue) => { base.Set.Invoke(element, new ElementValue<TInnerType>(iValue)); };
        }

    }
}
