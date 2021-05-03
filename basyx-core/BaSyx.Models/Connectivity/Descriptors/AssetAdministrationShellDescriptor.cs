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
using System.Collections;
using System.Linq;
using BaSyx.Models.Core.AssetAdministrationShell;

namespace BaSyx.Models.Connectivity.Descriptors
{
    [DataContract]
    public class AssetAdministrationShellDescriptor : IAssetAdministrationShellDescriptor
    {
        public Identifier Identification { get; set; }       
        public AdministrativeInformation Administration { get; set; }
        public string IdShort { get; set; }
        public LangStringSet Description { get; set; }
        public IEnumerable<IEndpoint> Endpoints { get; internal set; }

        [IgnoreDataMember]
        public IReferable Parent { get; set; }
        [IgnoreDataMember]
        public string Category => null;

        public ModelType ModelType => ModelType.AssetAdministrationShellDescriptor;

        public IAsset Asset { get; set; }

        public IElementContainer<ISubmodelDescriptor> SubmodelDescriptors { get; set; }

        [JsonConstructor]
        public AssetAdministrationShellDescriptor(IEnumerable<IEndpoint> endpoints) 
        {
            Endpoints = endpoints ?? new List<IEndpoint>();
            SubmodelDescriptors = new ElementContainer<ISubmodelDescriptor>(this);
        }

        public AssetAdministrationShellDescriptor(IAssetAdministrationShell aas, IEnumerable<IEndpoint> endpoints) : this(endpoints)
        {
            this.Identification = aas.Identification;
            this.Administration = aas.Administration;
            this.IdShort = aas.IdShort;
            this.Description = aas.Description;
            this.Asset = aas.Asset;            
        }

        public void AddSubmodel(ISubmodel submodel)
        {
            SubmodelDescriptors.Create(
                new SubmodelDescriptor(submodel, Endpoints.ToList()));
        }

        public void AddEndpoints(IEnumerable<IEndpoint> endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                (Endpoints as IList).Add(endpoint);
            }
        }

        public void SetEndpoints(IEnumerable<IEndpoint> endpoints)
        {
            Endpoints = endpoints;
        }
    }
}
