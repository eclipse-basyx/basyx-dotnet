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

using BaSyx.Models.AdminShell;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
	[DataContract]
	public class EnvironmentSpecificAssetId_V3_0
	{
		[DataMember(EmitDefaultValue = false, IsRequired = false, Name = "name")]
		[XmlElement("name")]
		public string Name { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
		[XmlElement("value")]
		public string Value { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Name = "externalSubjectId")]
		[XmlElement("externalSubjectId")]
		public EnvironmentReference_V3_0 ExternalSubjectId { get; set; }

		[XmlElement("semanticId")]
		public EnvironmentReference_V3_0 SemanticId { get; set; }

		[XmlArray("supplementalSemanticIds")]
		[XmlArrayItem("supplementalSemanticId")]
		public List<EnvironmentReference_V3_0> SupplementalSemanticIds { get; set; } = new List<EnvironmentReference_V3_0>();

		public bool ShouldSerializeSupplementalSemanticIds()
		{
			if (SupplementalSemanticIds?.Count > 0)
				return true;
			else
				return false;
		}
	}
}
