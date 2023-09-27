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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// The semantics of a property or other elements that may have a semantic description is defined by a concept description. 
    /// The description of the concept should follow a standardized schema (realized as data specification template). 
    /// </summary>
    public interface IConceptDescription : IIdentifiable, IHasDataSpecification, IModelElement
    {
        /// <summary>
        /// Global reference to an external definition the concept is compatible to or was derived from.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "isCaseOf")]
        IEnumerable<IReference> IsCaseOf { get; }
    }
}
