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

using System.Globalization;
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

        public override string ToString()
        {
            if(Value?.Value is double dbl)
                return dbl.ToString(_nfi);
            else if (Value?.Value is float flt)
                return flt.ToString(_nfi);
            else
                return Value?.Value?.ToString();
        }
    }

    public class PropertyValue<T> : PropertyValue
    {
        public PropertyValue(T value) : base(new ElementValue<T>(value))
        {
        }
    }
}