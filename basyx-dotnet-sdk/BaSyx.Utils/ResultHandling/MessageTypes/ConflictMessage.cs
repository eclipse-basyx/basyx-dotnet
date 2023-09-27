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

namespace BaSyx.Utils.ResultHandling
{
    public class ConflictMessage : Message
    {
        public ConflictMessage() : base(MessageType.Information, "Conflict", "409")
        { }

        public ConflictMessage(string what) : base(MessageType.Information, what + " already exists", "409")
        { }
    }
}
