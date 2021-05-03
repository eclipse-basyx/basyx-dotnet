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
using BaSyx.Models.Core.AssetAdministrationShell.Views;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract]
    public class AssetAdministrationShell : Identifiable, IAssetAdministrationShell
    {
        public IAsset Asset { get; set; }
        public IElementContainer<ISubmodel> Submodels { get; set; }   
        public IReference<IAssetAdministrationShell> DerivedFrom { get; set; }     
        public IElementContainer<IView> Views { get; set; }
        public ModelType ModelType => ModelType.AssetAdministrationShell;
        public IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications { get; }
        public IElementContainer<IConceptDictionary> ConceptDictionaries { get; set; }
        public IConceptDescription ConceptDescription { get; set; }

        [JsonConstructor]
        public AssetAdministrationShell(string idShort, Identifier identification) : base(idShort, identification)
        {
            Submodels = new ElementContainer<ISubmodel>(this);
            Views = new ElementContainer<IView>(this);
            MetaData = new Dictionary<string, string>();
            ConceptDictionaries = new ElementContainer<IConceptDictionary>(this);
            EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>();
        }
    }
}
