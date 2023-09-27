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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class Operation : SubmodelElement, IOperation
    {
        public IOperationVariableSet InputVariables { get; set; }
        public IOperationVariableSet OutputVariables { get; set; }
        public IOperationVariableSet InOutputVariables { get; set; }
        [IgnoreDataMember]
        public MethodCalledHandler OnMethodCalled { get; set; }
        public override ModelType ModelType => ModelType.Operation;
        public Operation(string idShort) : base(idShort) 
        {
            InputVariables = new OperationVariableSet();
            OutputVariables = new OperationVariableSet();
            InOutputVariables = new OperationVariableSet();
        }
    }
}
