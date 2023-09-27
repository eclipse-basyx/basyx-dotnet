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
using System;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    public interface IValueId
    {
        /// <summary>
        /// Reference to the global unique ID of a coded value
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueId")]
        IReference ValueId { get; set; }
    }
    public interface IValue
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        object Value { get; set; }
        [IgnoreDataMember]
        DataType ValueType { get; }       
        T ToObject<T>();
        object ToObject(Type type);
        /// <summary>
        /// Returns the Value as string representation
        /// </summary>
        /// <returns></returns>
        string ToString();
    }

    public interface IValueChanged
    {
        event EventHandler<ValueChangedArgs> ValueChanged;
    }

    public interface IValue<out T> : IValue
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        new T Value { get; }
    }
    public class ValueChangedArgs
    {
        public string IdShort { get; }

        public object Value { get; }

        public DataType ValueType { get; }

        public ValueChangedArgs(string idShort, object value, DataType valueType)
        {
            IdShort = idShort;
            Value = value;
            ValueType = valueType;
        }
    }

}
