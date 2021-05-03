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
using System.Runtime.Serialization;

namespace BaSyx.Models.Communication
{
    [DataContract]
    public class CallbackResponse
    {
        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "requestId")]
        public string RequestId { get; private set; }

        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "callbackUrl")]
        public Uri CallbackUrl { get; set; }

        public CallbackResponse(string requestId)
        {
            RequestId = requestId;
        }
    }
}
