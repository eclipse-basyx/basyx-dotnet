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
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Identification
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
