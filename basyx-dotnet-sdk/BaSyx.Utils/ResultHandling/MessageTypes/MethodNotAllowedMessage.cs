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
    public class MethodNotAllowedMessage : Message
    {
        public MethodNotAllowedMessage() : base(MessageType.Information, "MethodNotAllowed", "405")
        { }

        public MethodNotAllowedMessage(string method, string onWhat) : base(MessageType.Information, $"{method} on {onWhat} is not allowed", "405")
        { }
    }
}
