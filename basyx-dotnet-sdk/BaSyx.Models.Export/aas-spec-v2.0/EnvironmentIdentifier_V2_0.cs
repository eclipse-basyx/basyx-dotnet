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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    /// <summary>
    /// Used to uniquely identify an entity by using an identifier.
    /// </summary>
    [DataContract]
    public class EnvironmentIdentifier_V2_0
    {
        /// <summary>
        /// Identifier of the element. Its type is defined in idType.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "id")]
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlText]
        public string Id { get; set; }

        /// <summary>
        /// Type of the Identifierr, e.g. IRI, IRDI etc. The supported Identifier types are defined in the enumeration “IdentifierType”. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "idType")]
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlAttribute("idType")]
        public KeyType_V2_0 IdType { get; set; }

        internal EnvironmentIdentifier_V2_0() { }

        public EnvironmentIdentifier_V2_0(string id, KeyType_V2_0 idType)
        {
            Id = id;
            IdType = idType;
        }
    }
}
