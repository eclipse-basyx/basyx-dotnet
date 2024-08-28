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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
	[DataContract]
    public class EnvironmentLangString_V3_0
    {
		[DataMember(EmitDefaultValue = false, IsRequired = false, Name = "language")]
		[XmlElement("language")]
		public string Language { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Name = "text")]
		[XmlElement("text")]
		public string Text { get; set; }
	}
}
