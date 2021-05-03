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
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BaSyx.Models.Core.Common;

namespace BaSyx.Models.Core.AssetAdministrationShell.Semantics
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
