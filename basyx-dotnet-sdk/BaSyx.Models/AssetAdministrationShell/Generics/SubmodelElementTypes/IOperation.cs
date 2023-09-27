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
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.Models.AdminShell
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
