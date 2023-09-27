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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
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
