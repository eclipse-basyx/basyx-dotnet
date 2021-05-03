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

namespace BaSyx.Models.Core.AssetAdministrationShell.Generics
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
