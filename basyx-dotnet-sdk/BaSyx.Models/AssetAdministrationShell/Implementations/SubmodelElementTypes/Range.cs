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
using BaSyx.Models.Extensions;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    [JsonConverter(typeof(RangeConverter))]
    public class Range : SubmodelElement, IRange
    {
        public override ModelType ModelType => ModelType.Range;
        public IReference ValueId { get; set; }
        public IValue Min { get; set; }
        public IValue Max { get; set; }
        public DataType ValueType { get; set; }

        public Range(string idShort) : this(idShort, null) 
        { }

        [JsonConstructor]
        public Range(string idShort, DataType valueType) : base(idShort) 
        {
            ValueType = valueType;

            Get = element => { return new ElementValue(new { Min = Min?.Value, Max = Max?.Value}, new DataType(DataObjectType.AnyType)); };
            Set = (element, value) => { dynamic dVal = value?.Value; Min = new ElementValue(dVal?.Min, ValueType); Max = new ElementValue(dVal?.Max, ValueType); };
        }
    }
}
