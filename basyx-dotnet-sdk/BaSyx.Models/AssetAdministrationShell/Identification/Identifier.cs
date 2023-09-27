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
using BaSyx.Models.AdminShell;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// Used to uniquely identify an entity by using an identifier.
    /// </summary>
    [DataContract]
    public class Identifier
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
        public KeyType IdType { get; set; }

        internal Identifier() { }

        public Identifier(string id, KeyType idType)
        {
            Id = id;
            IdType = idType;
        }        
    }

}
