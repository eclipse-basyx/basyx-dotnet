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

        [IgnoreDataMember]
        KeyElements RefersTo { get; }

        /// <summary>
        /// Returns the official string representation of a Reference
        /// </summary>
        /// <returns></returns>
        string ToStandardizedString();
    }

    public interface IReference<out T> : IReference where T : IReferable
    { 
        
    }
}
