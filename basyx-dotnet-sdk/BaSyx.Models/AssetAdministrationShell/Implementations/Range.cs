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

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class Range : SubmodelElement<RangeValue>, IRange
    {
        [IgnoreDataMember]
        public new RangeValue Value { get => base.Value; set => base.Value = value; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.Range;
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueId")]
        public IReference ValueId { get; set; }       
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueType")]
        public DataType ValueType { get; set; }

        public Range(string idShort) : this(idShort, null) 
        { }

        public Range(string idShort, DataType valueType) : base(idShort) 
        {
            ValueType = valueType;           
        }
    }
}
