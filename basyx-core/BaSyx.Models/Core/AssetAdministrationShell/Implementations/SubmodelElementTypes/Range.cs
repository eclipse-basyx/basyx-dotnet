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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
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
