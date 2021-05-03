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
using BaSyx.Utils.ResultHandling;
using System.Runtime.Serialization;

namespace BaSyx.Models.Communication
{
    [DataContract]
    public class InvocationResponse
    {
        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "requestId")]
        public string RequestId { get; private set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "inoutputArguments")]
        public IOperationVariableSet InOutputArguments { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "outputArguments")]
        public IOperationVariableSet OutputArguments { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "operationResult")]
        public OperationResult OperationResult { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "executionState")]
        public ExecutionState ExecutionState { get; set; }

        public InvocationResponse(string requestId)
        {
            RequestId = requestId;
            InOutputArguments = new OperationVariableSet();
            OutputArguments = new OperationVariableSet();
            ExecutionState = ExecutionState.Initiated;
        }
    }
}
