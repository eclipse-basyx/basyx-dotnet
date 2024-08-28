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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// Element that can have a semantic definition plus some supplemental semantic definitions.
    /// 
    /// Constraint AASd-118: If a supplemental semantic ID
    /// (HasSemantics/supplementalSemanticId) is defined, there
    /// </summary>
    public interface IHasSemantics
    {
        /// <summary>
        /// Identifier of the semantic definition of the element called semantic ID or also main semantic ID of the element.
        /// 
        /// Note: it is recommended to use an external reference.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "semanticId")]
        IReference SemanticId { get; }

        /// <summary>
        /// Identifier of a supplemental semantic definition of the element called supplemental semantic ID of the element.
        /// 
        /// Note: it is recommended to use an external reference.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "supplementalSemanticIds")]
        IEnumerable<IReference> SupplementalSemanticIds { get; }
    }
}
