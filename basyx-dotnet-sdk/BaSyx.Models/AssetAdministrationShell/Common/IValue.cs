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
using System;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    public interface IValue
    {
        [IgnoreDataMember]
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
        [IgnoreDataMember]
        new T Value { get; }
    }
    public class ValueChangedArgs
    {
        public string IdShort { get; }

        public ValueScope ValueScope { get; }

        public ValueChangedArgs(string idShort, ValueScope valueScope)
        {
            IdShort = idShort;
            ValueScope = valueScope;
        }
    }

}
