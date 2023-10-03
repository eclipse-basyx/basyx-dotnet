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
    public class RelationshipElement : SubmodelElement, IRelationshipElement
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.RelationshipElement;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "first")]
        public IReference First { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "second")]
        public IReference Second { get; set; }      

        public RelationshipElement(string idShort) : base(idShort) 
        {
        }     
    }
}
