/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using System.Runtime.Serialization;

namespace BaSyx.Models.Connectivity
{
    public interface IEndpoint
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "protocolInformation")]
        ProtocolInformation ProtocolInformation { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "interface")]
        InterfaceName Interface { get; }
    }
}
