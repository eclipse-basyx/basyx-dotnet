/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
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
using System.Text;

namespace BaSyx.Utils.ResultHandling
{
    public class MessageCollection : List<IMessage>
    {
        public override string ToString()
        {
            string serializedMessageCollection = string.Empty;
            if (this.Count > 0)
                foreach (var item in this)
                    serializedMessageCollection += item.ToString() + Environment.NewLine;

            return serializedMessageCollection;
        }
    }
}
