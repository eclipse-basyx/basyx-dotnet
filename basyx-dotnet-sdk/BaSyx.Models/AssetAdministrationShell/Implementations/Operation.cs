/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.Operation;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "inputVariables")]
        public IOperationVariableSet InputVariables { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "outputVariables")]
        public IOperationVariableSet OutputVariables { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "inOutputVariables")]
        public IOperationVariableSet InOutputVariables { get; set; }

        [IgnoreDataMember]
        public MethodCalledHandler OnMethodCalled { get; set; }

        public Operation(string idShort) : base(idShort) 
        {
            InputVariables = new OperationVariableSet();
            OutputVariables = new OperationVariableSet();
            InOutputVariables = new OperationVariableSet();
        }
    }
}
