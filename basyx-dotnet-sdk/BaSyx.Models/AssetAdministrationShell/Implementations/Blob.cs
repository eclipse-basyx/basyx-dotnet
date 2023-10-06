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
using BaSyx.Utils.Extensions;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class Blob : SubmodelElement, IBlob
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.Blob;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "contentType")]
        public string ContentType { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public new string Value { get; set; }

        public Blob(string idShort) : base(idShort) 
        {
        }

        public void SetValue(byte[] bytes)
        {
            Value = StringOperations.Base64Encode(bytes);
        }

        public void SetValue(string value)
        {
            if (StringOperations.IsBase64String(value))
                Value = value;
            else
                Value = StringOperations.Base64Encode(value);
        }

        public byte[] GetBytes()
        {
            return StringOperations.GetBytesFromBase64String(Value);
        }
    }
}
