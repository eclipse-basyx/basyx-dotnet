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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Core.AssetAdministrationShell;

namespace BaSyx.Models.Connectivity.Descriptors
{

    [DataContract]
    public class SubmodelDescriptor : ISubmodelDescriptor
    {
        public Identifier Identification { get; set; }
        public AdministrativeInformation Administration { get; set; }
        public string IdShort { get; set; }
        
        public LangStringSet Description { get; set; }
        public IReference SemanticId { get; set; }
        public IEnumerable<IEndpoint> Endpoints { get; private set; }

        [IgnoreDataMember]
        public IReferable Parent { get; set; }
        [IgnoreDataMember]
        public string Category => null;

        public ModelType ModelType => ModelType.SubmodelDescriptor;

        [JsonConstructor]
        public SubmodelDescriptor(string idShort, IEnumerable<IEndpoint> endpoints)
        {
            IdShort = idShort;
            Endpoints = endpoints ?? new List<IEndpoint>();
        }

        public SubmodelDescriptor(ISubmodel submodel, IEnumerable<IEndpoint> endpoints) : this(submodel.IdShort, endpoints)
        {
            if (submodel == null)
                return;

            Identification = submodel.Identification;
            Administration = submodel.Administration;
            IdShort = submodel.IdShort;
            Description = submodel.Description;
            SemanticId = submodel.SemanticId;
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
