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
    public class FileElement : SubmodelElement, IFileElement
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.File;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "contentType")]
        public string ContentType { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public new string Value { get; set; }

        public FileElement(string idShort) : base(idShort)
        {

        }
    }
}
