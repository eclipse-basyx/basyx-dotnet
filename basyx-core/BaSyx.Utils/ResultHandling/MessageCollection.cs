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
