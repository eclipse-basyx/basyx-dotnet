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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Identification
{
    /// <summary>
    /// Administrative metainformation for an element like version information. 
    /// </summary>
    [DataContract]
    public class AdministrativeInformation
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
