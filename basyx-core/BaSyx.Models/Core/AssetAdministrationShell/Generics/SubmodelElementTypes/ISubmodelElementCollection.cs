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
    /// A submodel element collection is a set or list of submodel elements.
    /// </summary>
    public interface ISubmodelElementCollection : ISubmodelElement
    {
        /// <summary>
        /// If allowDuplicates=true then it is allowed that the collection contains the same element several times. Default = false 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "allowDuplicates")]
        bool AllowDuplicates { get; set; }

        /// <summary>
        /// If ordered=false then the elements in the property collection are not ordered. If ordered=true then the elements in the collection are ordered.Default = false 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "ordered")]
        bool Ordered { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        IElementContainer<ISubmodelElement> Value { get; set; }
    }
}
