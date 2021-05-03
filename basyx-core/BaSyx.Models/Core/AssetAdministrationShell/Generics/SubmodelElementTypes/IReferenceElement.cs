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

namespace BaSyx.Models.Core.AssetAdministrationShell.Generics
{
    /// <summary>
    /// A reference element is a data element that defines a logical reference to another element within the same or another AAS or a reference to an external object or entity. 
    /// </summary>
    public interface IReferenceElement : ISubmodelElement
    {
        /// <summary>
        /// Reference to any other referable element of the same of any other AAS or a reference to an external object or entity. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        IReference Value { get; set; }
    }

    public interface IReferenceElement<T> where T : IReferable
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        IReference<T> Value { get; set; }
    }
}

