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
using System;
using System.Collections.Generic;

namespace BaSyx.Utils.ResultHandling
{
    public class MessageCollection : List<IMessage>
    {
        public new void Add(IMessage message)
        {
            if(message != null)
                base.Add(message);
        }       

        public override string ToString()
        {
            string serializedMessageCollection = string.Empty;
            if (Count > 0)
                foreach (var message in this)
                    if (message != null)
                        serializedMessageCollection += message.ToString() + Environment.NewLine;

            return serializedMessageCollection;
        }
    }
}
