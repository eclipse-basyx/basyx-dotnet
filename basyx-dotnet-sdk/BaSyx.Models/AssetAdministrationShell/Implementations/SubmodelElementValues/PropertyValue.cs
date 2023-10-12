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

using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BaSyx.Models.AdminShell
{
    public class PropertyValue : ValueScope
    {
        public override ModelType ModelType => ModelType.Property;

        public IValue Value { get; set; }

        public PropertyValue(IValue value)
        {
            Value = value;
        }

        public static implicit operator Task<PropertyValue>(PropertyValue value)
        {
            return Task.FromResult(value);
        }

        public override JsonValue ToJson()
        {
            return JsonValue.Create(Value.Value);
        }
    }

    public class PropertyValue<T> : PropertyValue
    {
        public PropertyValue(T value) : base(new ElementValue<T>(value))
        {
        }
    }
}
