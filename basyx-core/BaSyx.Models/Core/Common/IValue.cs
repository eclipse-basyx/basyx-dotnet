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
using System;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.Common
{
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
