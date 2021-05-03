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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Semantics
{
    /// <summary>
    /// Element that can be extended by using data specification templates. A data specification template defines the additional attributes an element may or shall have. 
    /// The data specifications used are explicitly specified with their global id. 
    /// </summary>
    public interface IHasDataSpecification
    {
        [IgnoreDataMember]
        IConceptDescription ConceptDescription { get; }

        /// <summary>
        /// Global reference to the data specification template used by the element. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "embeddedDataSpecifications")]
        IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications { get; }
    }
}
