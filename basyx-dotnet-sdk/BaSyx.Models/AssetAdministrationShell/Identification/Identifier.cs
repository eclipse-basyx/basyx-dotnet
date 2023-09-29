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
        /// The globally unique identification of the element
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "id")]
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlText]
        public string Id { get; set; }

        internal Identifier() { }

        public Identifier(string id)
        {
            Id = id;
        }

        public static implicit operator string(Identifier identifier) => identifier.Id;

        public static implicit operator Identifier(string id) => new Identifier(id);
    }

}
