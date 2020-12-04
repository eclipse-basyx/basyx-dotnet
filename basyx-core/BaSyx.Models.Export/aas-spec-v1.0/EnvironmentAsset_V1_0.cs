/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/

using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentAsset_V1_0 : EnvironmentIdentifiable_V1_0, IModelType, IAsset
    {
        [JsonProperty("kind")]
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
        IReferable IReferable.Parent { get; set; }
    }
}