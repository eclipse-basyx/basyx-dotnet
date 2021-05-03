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
