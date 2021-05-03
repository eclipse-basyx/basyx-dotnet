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
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.Models.Core.AssetAdministrationShell.Generics
{
    public delegate Task<OperationResult> MethodCalledHandler(
        IOperation operation,
        IOperationVariableSet inputArguments, 
        IOperationVariableSet inoutputArguments, 
        IOperationVariableSet outputArguments, 
        CancellationToken cancellationToken);

    /// <summary>
    /// An operation is a submodel element with input and output variables. 
    /// </summary>
    public interface IOperation : ISubmodelElement
    {
        /// <summary>
        /// Input parameter of the operation.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "inputVariables")]
        IOperationVariableSet InputVariables { get; set; }

        /// <summary>
        /// Output parameter of the operation.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "outputVariables")]
        IOperationVariableSet OutputVariables { get; set; }

        /// <summary>
        ///  Parameter that is input and output of the operation. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "inoutputVariables")]
        IOperationVariableSet InOutputVariables { get; set; }


        [IgnoreDataMember]       
        MethodCalledHandler OnMethodCalled { get; }
    }   
}
