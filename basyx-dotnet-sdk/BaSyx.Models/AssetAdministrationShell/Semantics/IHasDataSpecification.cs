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
