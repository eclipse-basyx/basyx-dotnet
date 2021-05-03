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
    /// An annotated relationship element is a relationship element that can be annotated with additional data elements. 
    /// </summary>
    public interface IAnnotatedRelationshipElement : IRelationshipElement
    {
        /// <summary>
        /// Annotations that hold for the relationships between the two elements.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "annotation")]
        IElementContainer<ISubmodelElement> Annotations { get; set; }
    }
}
