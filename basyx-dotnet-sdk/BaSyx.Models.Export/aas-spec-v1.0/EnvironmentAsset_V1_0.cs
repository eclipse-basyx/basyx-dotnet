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
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentAsset_V1_0 : EnvironmentIdentifiable_V1_0, IModelType, IAsset
    {
        [JsonProperty("kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("kind")]
        public AssetKind Kind { get; set; }

        [JsonProperty("assetIdentificationModel")]
        [XmlElement("assetIdentificationModelRef")]
        public EnvironmentReference_V1_0 AssetIdentificationModelReference { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public ModelType ModelType => ModelType.Asset;

        [XmlIgnore]
        public IReference<ISubmodel> AssetIdentificationModel => AssetIdentificationModelReference.ToReference_V1_0<ISubmodel>();            
        
        [XmlIgnore]
        public IConceptDescription ConceptDescription => throw new System.NotImplementedException();
        [XmlIgnore]
        public IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications => throw new System.NotImplementedException();
        [XmlIgnore]
        public IReference<ISubmodel> BillOfMaterial => throw new System.NotImplementedException();
        [XmlIgnore]
        public LangStringSet DisplayName => throw new System.NotImplementedException();

        [XmlIgnore]
        IReferable IReferable.Parent { get; set; }
    }
}