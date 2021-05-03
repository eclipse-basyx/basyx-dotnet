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
