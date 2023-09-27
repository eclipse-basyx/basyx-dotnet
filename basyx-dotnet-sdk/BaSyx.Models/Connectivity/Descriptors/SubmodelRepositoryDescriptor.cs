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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace BaSyx.Models.Connectivity
{
    [DataContract]
    public class SubmodelRepositoryDescriptor : Descriptor, ISubmodelRepositoryDescriptor
    {
        public IEnumerable<ISubmodelDescriptor> SubmodelDescriptors => _submodelDescriptors;
        public override ModelType ModelType => ModelType.SubmodelRepositoryDescriptor;

        private List<ISubmodelDescriptor> _submodelDescriptors;

        public SubmodelRepositoryDescriptor(IEnumerable<IEndpoint> endpoints) : base (endpoints)
        {
            _submodelDescriptors = new List<ISubmodelDescriptor>();
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
            _submodelDescriptors.Add(new SubmodelDescriptor(submodel, Endpoints.ToList()));
        }

        public void AddSubmodelDescriptor(ISubmodelDescriptor submodelDescriptor)
        {
            _submodelDescriptors.Add(submodelDescriptor);
        }
    }
}
