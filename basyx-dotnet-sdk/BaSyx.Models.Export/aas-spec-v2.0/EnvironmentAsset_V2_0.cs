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
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentAsset_V2_0 : EnvironmentIdentifiable_V2_0, IModelType
    {
        [JsonProperty("kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("kind")]
        public AssetKind Kind { get; set; }

        [JsonProperty("assetIdentificationModel")]
        [XmlElement("assetIdentificationModelRef")]
        public EnvironmentReference_V2_0 AssetIdentificationModelReference { get; set; }

        [JsonProperty("billOfMaterial")]
        [XmlElement("billOfMaterialRef")]
        public EnvironmentReference_V2_0 BillOfMaterial { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public ModelType ModelType => ModelType.Asset;
    }
}
