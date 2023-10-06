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
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BaSyx.Models.AdminShell
{
    ///<inheritdoc cref="IProperty"/>
    [DataContract]
    public class Property : SubmodelElement<PropertyValue>, IProperty
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.Property;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueType")]
        public virtual DataType ValueType { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueId")]
        public IReference ValueId { get; set; }

        /// <summary>
        /// Only internal temporary storage of the current value. 
        /// Get and Set operations shall only be processed via its respective handler.
        /// </summary>
        protected object _value;

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
                return new PropertyValue(new ElementValue(_value, ValueType)); 
            };

            Set = (element, iValue) => 
            { 
                _value = iValue.Value;
                OnValueChanged(new ValueChangedArgs(IdShort, _value, ValueType));
                return Task.CompletedTask;
            };
        }
    }
    ///<inheritdoc cref="IProperty"/>
    [DataContract]
    public class Property<TInnerType> : Property
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.Property;
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueType")]
        public new DataType ValueType => typeof(TInnerType);

        private GetTypedValueHandler<TInnerType> _get;
        private SetTypedValueHandler<TInnerType> _set;

        [JsonIgnore, IgnoreDataMember]
        public new GetTypedValueHandler<TInnerType> Get 
        {
            get => _get;
            set
            {
                _get = value;
                if (value != null)
                    base.Get = new GetValueScopeHandler<PropertyValue>(async element => new PropertyValue<TInnerType>(await _get.Invoke(element)));
                else
                    base.Get = null;
            }
        }
        [JsonIgnore, IgnoreDataMember]
        public new SetTypedValueHandler<TInnerType> Set 
        {
            get => _set;
            set
            {
                _set = value;
                if (value != null)
                    base.Set = new SetValueScopeHandler<PropertyValue>(async (element, iValue) =>
                    {
                        TInnerType typedValue = iValue.Value.ToObject<TInnerType>();
                        await _set.Invoke(element, typedValue);
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
            _get = async element =>
            {
                if (base.Get != null)
                {
                    var result = await base.Get.Invoke(element);
                    return result.Value.ToObject<TInnerType>();
                }
                else
                    return default;
            };
            _set = async (element, iValue) => { await base.Set?.Invoke(element, new PropertyValue(new ElementValue<TInnerType>(iValue))); };
        }
    }
}
