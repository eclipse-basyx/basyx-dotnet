/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
    /// <summary>
    /// An operation variable is a submodel element that is used as input or output variable of an operation. 
    /// </summary>
    public interface IOperationVariable : IModelElement
    {
        /// <summary>
        /// Describes the needed argument for an operation via a submodel element of kind=Template.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        ISubmodelElement Value { get; }
    }
}
