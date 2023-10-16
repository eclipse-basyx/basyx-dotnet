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
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Connectivity
{
    [DataContract]
    public class AssetAdministrationShellDescriptor : Descriptor, IAssetAdministrationShellDescriptor
    {
        public AssetKind AssetKind { get; set; }
        public string AssetType { get; set; }
        public Identifier GlobalAssetId { get; set; }
        public IEnumerable<SpecificAssetId> SpecificAssetIds { get; set; }
        public IEnumerable<ISubmodelDescriptor> SubmodelDescriptors { get => _submodelDescriptors; }
        public override ModelType ModelType => ModelType.AssetAdministrationShellDescriptor;

        private List<ISubmodelDescriptor> _submodelDescriptors;

        [JsonConstructor]
        public AssetAdministrationShellDescriptor(IEnumerable<IEndpoint> endpoints) : base (endpoints)
        {
            _submodelDescriptors = new List<ISubmodelDescriptor>();
            SpecificAssetIds = new List<SpecificAssetId>();
        }

        public AssetAdministrationShellDescriptor(IAssetAdministrationShell aas, IEnumerable<IEndpoint> endpoints) : this(endpoints)
        {
            IdShort = aas.IdShort;
            Id = aas.Id;
            Administration = aas.Administration;
            Description = aas.Description;       
            DisplayName = aas.DisplayName;
            AssetKind = aas.AssetInformation != null ?  aas.AssetInformation.AssetKind : default(AssetKind);
            AssetType = aas.AssetInformation?.AssetType;
            GlobalAssetId = aas.AssetInformation?.GlobalAssetId;
            SpecificAssetIds = aas.AssetInformation?.SpecificAssetIds;
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

        public void SetSubmodelDescriptors(IEnumerable<ISubmodelDescriptor> submodelDescriptors)
        {
            _submodelDescriptors = submodelDescriptors.ToList();
        }

        public void RemoveSubmodelDescriptor(Identifier id)
        {
            int index = _submodelDescriptors.FindIndex(s => s.Id == id);
            if(index >= 0)
                _submodelDescriptors.RemoveAt(index);
        }

        public void ClearSubmodelDescriptors() => _submodelDescriptors.Clear();
    }
}
