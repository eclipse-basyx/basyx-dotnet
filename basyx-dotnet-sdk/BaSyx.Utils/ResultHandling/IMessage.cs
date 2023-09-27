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

namespace BaSyx.Utils.ResultHandling
{
    public interface IMessage
    {
        [DataMember(Name = "messageType", EmitDefaultValue = false, IsRequired = false)]
        MessageType MessageType { get; set; }
        
        [DataMember(Name = "code", EmitDefaultValue = false, IsRequired = false)]
        string Code { get; set; }
        
        [DataMember(Name = "text", EmitDefaultValue = false, IsRequired = false)]
        string Text { get; set; }
        
        string ToString();
    }
}
