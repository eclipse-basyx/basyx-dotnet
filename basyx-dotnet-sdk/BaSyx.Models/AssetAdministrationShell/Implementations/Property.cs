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
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
    
        public Property(string idShort) : this(idShort, null, null)
        { }

        [JsonConstructor]
        public Property(string idShort, DataType valueType) : this(idShort, valueType, null)
        { }
        
        public Property(string idShort, DataType valueType, object value) : base(idShort)
        {
            ValueType = valueType;
            
            if (value != null)
            {                
                if (ValueType == null)
                {
                    var elementValue = new ElementValue(value);
                    _valueScope = new PropertyValue(elementValue);
                    ValueType = elementValue.ValueType;
                }
                else if (value.GetType() == valueType.SystemType)
                    _valueScope = new PropertyValue(new ElementValue(value, valueType));
                else
                    _valueScope = new PropertyValue(new ElementValue(ElementValue.ToObject(value, valueType.SystemType), valueType.SystemType));
            }
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
                    base.Set = new SetValueScopeHandler<PropertyValue>(async (element, propValue) =>
                    {
                        TInnerType typedValue = propValue.Value.ToObject<TInnerType>();
                        await _set.Invoke(element, typedValue);
                        OnValueChanged(new ValueChangedArgs(IdShort, propValue));
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
            _set = async (element, iValue) => 
            { 
                await base.Set?.Invoke(element, new PropertyValue(new ElementValue<TInnerType>(iValue))); 
            };
        }
    }
}
