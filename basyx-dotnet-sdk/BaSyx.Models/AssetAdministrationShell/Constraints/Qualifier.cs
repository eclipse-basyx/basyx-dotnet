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
    [DataContract]
    public class Qualifier : Constraint, IQualifier
    {
        public string Type { get; set; }
        public object Value { get; set; }
        public DataType ValueType { get; set; }
        public IReference SemanticId { get; set; }
        public override ModelType ModelType => ModelType.Qualifier;
        public IReference ValueId { get; set; }

        public T ToObject<T>()
        {
            return new ElementValue(Value, ValueType).ToObject<T>();
        }

        public object ToObject(Type type)
        {
            return new ElementValue(Value, ValueType).ToObject(type);
        }

    }
}
