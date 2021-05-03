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

using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.Common;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentAsset_V2_0 : EnvironmentIdentifiable_V2_0, IModelType
    {
        [JsonProperty("kind")]
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
