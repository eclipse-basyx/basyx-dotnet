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
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// A specific asset ID describes a generic supplementary identifying attribute of the asset.
    /// The specific asset ID is not necessarily globally unique.
    /// 
    /// Constraint AASd-133: SpecificAssetId/externalSubjectId shall be a global reference, i.e.
    /// Reference/type = ExternalReference.
    /// </summary>
    [DataContract]
    public class SpecificAssetId : IHasSemantics
    {
        /// <summary>
        /// Name of the asset identifier.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The value of the specific asset identifier with the corresponding name.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public string Value { get; set; }

        /// <summary>
        /// The unique ID of the (external) subject the specific asset ID value belongs to or has meaning to
        /// 
        /// Note: this is an external reference.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "externalSubjectId")]
        public IReference ExternalSubjectId { get; set; }

        public IReference SemanticId { get; set; }
    }
}
