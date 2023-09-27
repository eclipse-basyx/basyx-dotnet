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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        [JsonConverter(typeof(StringEnumConverter))]
        KeyElements Type { get; }

        /// <summary>
        /// Type of the key value.In case of idType = idShort local shall be true. 
        /// In case type=GlobalReference idType shall not be IdShort. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "idType")]
        [JsonConverter(typeof(StringEnumConverter))]
        KeyType IdType { get; }

        /// <summary>
        /// The key value, for example an IRDI if the idType=IRDI. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        string Value { get; }

        /// <summary>
        /// Denotes if the key references a model element of the same AAS (=true) or not (=false).
        /// In case of local = false the key may reference a model element of another AAS or an entity outside any AAS that has a global unique id. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "local")]
        bool Local { get; }

        /// <summary>
        /// Returns the official string representation of a Key according to Details of Asset Administration Shell (Chapter 5.2.1)
        /// </summary>
        /// <returns></returns>
        string ToStandardizedString();
    }
}
