/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
    public class AssetAdministrationShellRepositoryDescriptor : Descriptor, IAssetAdministrationShellRepositoryDescriptor
    {
        public IEnumerable<IAssetAdministrationShellDescriptor> AssetAdministrationShellDescriptors => _assetAdministrationShellDescriptors;
        public override ModelType ModelType => ModelType.AssetAdministrationShellRepositoryDescriptor;

        private List<IAssetAdministrationShellDescriptor> _assetAdministrationShellDescriptors;

        public AssetAdministrationShellRepositoryDescriptor(IEnumerable<IEndpoint> endpoints) : base (endpoints)
        {
            _assetAdministrationShellDescriptors = new List<IAssetAdministrationShellDescriptor>();
        }
     
        public AssetAdministrationShellRepositoryDescriptor(IEnumerable<IAssetAdministrationShell> shells, IEnumerable<IEndpoint> endpoints) : this(endpoints)
        {
            if (shells?.Count() > 0)
                foreach (var shell in shells)
                {
                    AddAssetAdministrationShell(shell);
                }
        }

        public void AddAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            AssetAdministrationShellDescriptor assetAdministrationShellDescriptor = new AssetAdministrationShellDescriptor(aas, Endpoints.ToList());
            if (aas.Submodels?.Count() > 0)
                foreach (var submodel in aas.Submodels.Values)
                {
                    assetAdministrationShellDescriptor.AddSubmodel(submodel, Endpoints.ToList());
                }

            _assetAdministrationShellDescriptors.Add(assetAdministrationShellDescriptor);
        }

        public void AddAssetAdministrationShellDescriptor(IAssetAdministrationShellDescriptor aasDescriptor)
        {
            _assetAdministrationShellDescriptors.Add(aasDescriptor);
        }
    }
}
