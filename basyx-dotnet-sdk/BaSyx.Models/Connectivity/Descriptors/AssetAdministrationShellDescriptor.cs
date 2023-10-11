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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace BaSyx.Models.Connectivity
{
    [DataContract]
    public class AssetAdministrationShellDescriptor : Descriptor, IAssetAdministrationShellDescriptor
    {   
        public Identifier GlobalAssetId { get; set; }
        public IEnumerable<SpecificAssetId> SpecificAssetIds { get; set; }
        public IEnumerable<ISubmodelDescriptor> SubmodelDescriptors { get => _submodelDescriptors; set { _submodelDescriptors = value.ToList(); } }
        public override ModelType ModelType => ModelType.AssetAdministrationShellDescriptor;

        private List<ISubmodelDescriptor> _submodelDescriptors;

        public AssetAdministrationShellDescriptor(IEnumerable<IEndpoint> endpoints) : base (endpoints)
        {
            _submodelDescriptors = new List<ISubmodelDescriptor>();
            SpecificAssetIds = SpecificAssetIds?.Count() > 0 ? SpecificAssetIds : new List<SpecificAssetId>();
        }

        public AssetAdministrationShellDescriptor(IAssetAdministrationShell aas, IEnumerable<IEndpoint> endpoints) : this(endpoints)
        {
            IdShort = aas.IdShort;
            Identification = aas.Id;
            Administration = aas.Administration;
            Description = aas.Description;       
            DisplayName = aas.DisplayName;
            GlobalAssetId = aas.AssetInformation?.GlobalAssetId;
            SpecificAssetIds = aas.AssetInformation?.SpecificAssetIds?.Count() > 0 ? aas.AssetInformation.SpecificAssetIds : SpecificAssetIds;
        }

        public void AddSubmodel(ISubmodel submodel, IEnumerable<IEndpoint> submodelEndpoints = null)
        {
            var smEndpoints = submodelEndpoints ?? Endpoints.ToList();
            _submodelDescriptors.Add(new SubmodelDescriptor(submodel, smEndpoints));
        }

        public void AddSubmodelDescriptor(ISubmodelDescriptor submodelDescriptor)
        {
            _submodelDescriptors.Add(submodelDescriptor);
        }
    }
}
