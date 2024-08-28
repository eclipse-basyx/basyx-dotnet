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

using BaSyx.Models.AdminShell;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentAssetInformation_V3_0
    {
        [JsonProperty("assetKind")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("assetKind")]
        public AssetKind AssetKind { get; set; }

		[JsonProperty("assetType")]
		[XmlElement("assetType")]
		public Identifier AssetType { get; set; }

		[JsonProperty("globalAssetId")]
		[XmlElement("globalAssetId")]
		public string GlobalAssetId { get; set; }

		[JsonProperty("specificAssetIds")]
		[XmlArray("specificAssetIds")]
		[XmlArrayItem("specificAssetId")]
		public List<EnvironmentSpecificAssetId_V3_0> SpecificAssetIds { get; set; } = new List<EnvironmentSpecificAssetId_V3_0>();

		[JsonProperty("defaultThumbnail")]
		[XmlElement("defaultThumbnail")]
		public EnvironmentResource_V3_0 DefaultThumbnail { get; set; }

		public bool ShouldSerializeSpecificAssetIds()
		{
			if (SpecificAssetIds?.Count > 0)
				return true;
			else
				return false;
		}
	}
}
