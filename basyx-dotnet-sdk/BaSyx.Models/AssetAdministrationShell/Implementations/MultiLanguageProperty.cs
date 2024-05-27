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
    public class MultiLanguageProperty : SubmodelElement<MultiLanguagePropertyValue>, IMultiLanguageProperty
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.MultiLanguageProperty;
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueId")]
        public IReference ValueId { get; set; }
        public MultiLanguageProperty(string idShort) : base(idShort)
        {

        }
    }
}
