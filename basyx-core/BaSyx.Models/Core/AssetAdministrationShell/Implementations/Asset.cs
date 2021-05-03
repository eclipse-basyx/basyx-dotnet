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

using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract]
    public class Asset : Identifiable, IAsset
    {
        public AssetKind Kind { get; set; }
        public IReference<ISubmodel> AssetIdentificationModel { get; set; }
        public IReference<ISubmodel> BillOfMaterial { get; set; }
        public IReference SemanticId { get; set; }
        public IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications { get; }
        public IConceptDescription ConceptDescription { get; set; }
        public ModelType ModelType => ModelType.Asset;

        [JsonConstructor]
        public Asset(string idShort, Identifier identification) : base(idShort, identification)
        {
            Kind = AssetKind.Instance;
            EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>();
        }
    }
}
