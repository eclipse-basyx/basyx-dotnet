/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    /// <summary>
    /// Administrative metainformation for an element like version information. 
    /// </summary>
    [DataContract]
    public class EnvironmentAdministrativeInformation_V3_0
    {
        /// <summary>
        /// Version of the element. 
        /// </summary>
        [DataMember(Name = "version")]
        [XmlElement("version")]
        public string Version { get; set; }

        /// <summary>
        /// Revision of the element. 
        /// </summary>
        [DataMember(Name = "revision")]
        [XmlElement("revision")]
        public string Revision { get; set; }
    }
}
