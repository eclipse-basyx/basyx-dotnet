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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// A key is a reference to an element by its id. 
    /// </summary>
    public interface IKey
    {
        /// <summary>
        /// Denote which kind of entity is referenced. In case type = GlobalReference then the element is a global unique id.  
        /// In all other cases the key references a model element of the same or of another AAS.The name of the model element is explicitly listed.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "type")]
        KeyType Type { get; }

        /// <summary>
        /// The key value, for example an IRDI if the idType=IRDI. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        string Value { get; }

        /// <summary>
        /// Returns the official string representation of a Key according to Details of Asset Administration Shell (Chapter 5.2.1)
        /// </summary>
        /// <returns></returns>
        string ToStandardizedString();
    }
}
