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
using BaSyx.Models.Core.Common;
using System.Runtime.Serialization;

namespace BaSyx.Models.Communication
{
    [DataContract]
    public class InvocationRequest
    {
        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "requestId")]
        public string RequestId { get; private set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "inputArguments")]
        public IOperationVariableSet InputArguments { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "inoutputArguments")]
        public IOperationVariableSet InOutputArguments { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "timeout")]
        public int? Timeout { get; set; }

        public InvocationRequest(string requestId)
        {
            RequestId = requestId;
            InputArguments = new OperationVariableSet();
            InOutputArguments = new OperationVariableSet();
        }
    }
}
