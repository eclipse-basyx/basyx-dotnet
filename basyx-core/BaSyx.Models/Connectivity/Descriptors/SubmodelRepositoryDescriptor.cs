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
using System.Linq;
using System.Collections;
using BaSyx.Models.Core.AssetAdministrationShell;

namespace BaSyx.Models.Connectivity.Descriptors
{
    [DataContract]
    public class SubmodelRepositoryDescriptor : ISubmodelRepositoryDescriptor
    {
        [IgnoreDataMember]
        public Identifier Identification { get; set; }
        [IgnoreDataMember]
        public AdministrativeInformation Administration { get; set; }
        [IgnoreDataMember]
        public string IdShort { get; set; }
        [IgnoreDataMember]
        public LangStringSet Description { get; set; }
        public IEnumerable<IEndpoint> Endpoints { get; internal set; }

        [IgnoreDataMember]
        public IReferable Parent { get; set; }
        [IgnoreDataMember]
        public string Category => null;

        public ModelType ModelType => ModelType.AssetAdministrationShellRepositoryDescriptor;

        public IElementContainer<ISubmodelDescriptor> SubmodelDescriptors { get; set; }

        public SubmodelRepositoryDescriptor(IEnumerable<IEndpoint> endpoints) 
        {
            Endpoints = endpoints ?? new List<IEndpoint>();
            SubmodelDescriptors = new ElementContainer<ISubmodelDescriptor>(this);
        }
     
        [JsonConstructor]
        public SubmodelRepositoryDescriptor(IEnumerable<ISubmodel> submodels, IEnumerable<IEndpoint> endpoints) : this(endpoints)
        {
            if (submodels?.Count() > 0)
                foreach (var submodel in submodels)
                {
                    AddSubmodel(submodel);
                }
        }

        public void AddSubmodel(ISubmodel submodel)
        {
            SubmodelDescriptor submodelDescriptor = new SubmodelDescriptor(submodel, Endpoints.ToList());
            SubmodelDescriptors.Create(submodelDescriptor);
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
