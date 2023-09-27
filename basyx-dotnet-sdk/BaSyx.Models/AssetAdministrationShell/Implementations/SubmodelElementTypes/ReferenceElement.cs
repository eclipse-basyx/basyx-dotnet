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

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class ReferenceElement : SubmodelElement, IReferenceElement
    {
        public override ModelType ModelType => ModelType.ReferenceElement;
        public IReference Value { get; set; }

        public ReferenceElement(string idShort) : base(idShort) 
        {
            Get = element => { return new ElementValue(Value, new DataType(DataObjectType.AnyType)); };
            Set = (element, value) => { Value = value?.Value as IReference; };
        }
    }
}
