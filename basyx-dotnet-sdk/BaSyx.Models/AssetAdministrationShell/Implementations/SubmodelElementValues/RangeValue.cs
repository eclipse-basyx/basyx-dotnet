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
using System.Runtime.Serialization;
using System.Text.Json.Nodes;

namespace BaSyx.Models.AdminShell
{
    public class RangeValue : ValueScope
    {
        public override ModelType ModelType => ModelType.Range;

        /// <summary>
        /// The minimum value of the range. If the min value is missing then the value is assumed to be negative infinite.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "min")]
        public IValue Min { get; set; }

        /// <summary>
        /// The maximum value of the range. If the max value is missing then the value is assumed to be positive infinite.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "max")]
        public IValue Max { get; set; }

        public RangeValue() { }
        public RangeValue(IValue min, IValue max)
        {
            Min = min;
            Max = max;
        }

        public override JsonValue ToJson()
        {
            return JsonValue.Create(new { Min = Min.Value, Max = Max.Value });
        }
    }
}
