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
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
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

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "executionResult")]
        public OperationResult ExecutionResult { get; set; }

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
