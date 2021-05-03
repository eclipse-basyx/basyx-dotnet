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
using System.Runtime.Serialization;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
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
