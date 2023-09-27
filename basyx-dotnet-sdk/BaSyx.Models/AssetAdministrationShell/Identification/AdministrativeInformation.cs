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
using System.Xml.Serialization;

namespace BaSyx.Models.AdminShell
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

        /// <summary>
        /// The subject ID of the subject responsible for making the element.
        /// </summary>
        [DataMember(Name = "creator")]
        [XmlElement("creator")]
        public IReference Creator { get; set; }

        /// <summary>
        /// Identifier of the template that guided the creation of the element.
        /// Note 1: in case of a submodel, the template
        /// ID is the identifier of the submodel template
        /// that guided the creation of the submodel.
        /// Note 2: the submodel template ID is not
        /// relevant for validation. Here, the
        /// Submodel/semanticId shall be used.
        /// Note 3: usage of the template ID is not
        /// restricted to submodel instances.The
        /// creation of submodel templates can also be
        /// guided by another submodel template.
        /// </summary>
        [DataMember(Name = "templateId")]
        [XmlElement("templateId")]
        public Identifier TemplateId { get; set; }
    }
}
