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
using System.Text.Json.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// Reference to either a model element of the same or another AAs or to an external entity. 
    /// A reference is an ordered list of keys, each key referencing an element.
    /// The complete list of keys may for example be concatenated to a path that then gives unique access to an element or entity.
    /// </summary>
    public interface IReference
    {
        [JsonIgnore, IgnoreDataMember]
        IKey First { get; }

        /// <summary>
        /// Unique reference in its name space. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "type")]
        ReferenceType Type { get; }

        /// <summary>
        /// Expected semantic ID of the referenced model element(Reference/type= ModelReference); there typically is no semantic ID for for the referenced object of external references (Reference/type= ExternalReference).
        /// Note 1: if Reference/referredSemanticId is
        /// defined, the semanticId of the model element referenced should have a matching semantic ID.If this is not the case, a validator should raise a warning.
        /// Note 2: it is recommended to use an
        /// external reference for the semantic ID expected from the referenced model element.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "referredSemanticId")]
        IReference ReferredSemanticId { get; }

        /// <summary>
        /// Unique reference in its name space. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "keys")]
        IEnumerable<IKey> Keys { get; }

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
