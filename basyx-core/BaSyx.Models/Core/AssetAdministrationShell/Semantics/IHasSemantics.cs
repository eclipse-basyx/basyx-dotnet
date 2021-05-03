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
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Semantics
{
    /// <summary>
    /// Element that can have a semantic definition. 
    /// </summary>
    public interface IHasSemantics
    {
        /// <summary>
        /// Identifier of the semantic definition of the element. It is called semantic id of the element.
        /// The semantic id may either reference an external global id or it may reference a referable model element of kind = Template that defines the semantics of the element.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "semanticId")]
        IReference SemanticId { get; }
    }
}
