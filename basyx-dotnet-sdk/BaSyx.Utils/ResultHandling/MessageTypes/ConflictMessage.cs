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

using System.Text.Json.Serialization;

namespace BaSyx.Utils.ResultHandling
{
    public class ConflictMessage : Message
    {
        [JsonConstructor]
        public ConflictMessage() : base(MessageType.Information, "Conflict", "409")
        { }

        public ConflictMessage(string what) : base(MessageType.Information, what + " already exists", "409")
        { }
    }
}
