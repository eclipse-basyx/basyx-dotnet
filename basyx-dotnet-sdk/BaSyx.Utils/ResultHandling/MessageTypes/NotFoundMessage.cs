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
    public class NotFoundMessage : Message
    {
        public NotFoundMessage() : base(MessageType.Information, "NotFound", "404")
        { }

        public NotFoundMessage(string what) : base(MessageType.Information, what + " not found", "404")
        { }
    }
}
