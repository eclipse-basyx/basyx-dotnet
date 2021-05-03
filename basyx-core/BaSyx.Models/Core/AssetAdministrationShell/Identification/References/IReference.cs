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

namespace BaSyx.Models.Core.AssetAdministrationShell.Identification
{
    /// <summary>
    /// Reference to either a model element of the same or another AAs or to an external entity. 
    /// A reference is an ordered list of keys, each key referencing an element.
    /// The complete list of keys may for example be concatenated to a path that then gives unique access to an element or entity.
    /// </summary>
    public interface IReference
    {
        [IgnoreDataMember]
        IKey First { get; }

        /// <summary>
        /// Unique reference in its name space. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "keys")]
        List<IKey> Keys { get; }

        /// <summary>
        /// Returns the official string representation of a Reference
        /// </summary>
        /// <returns></returns>
        string ToStandardizedString();
    }

    public interface IReference<out T> : IReference where T : IReferable
    { }
}
