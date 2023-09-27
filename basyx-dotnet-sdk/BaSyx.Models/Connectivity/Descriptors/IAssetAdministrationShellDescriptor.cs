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

namespace BaSyx.Models.Connectivity
{
    public interface IAssetAdministrationShellDescriptor : IServiceDescriptor, IModelElement
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "globalAssetId")]
        IReference GlobalAssetId { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "specificAssetIds")]
        IEnumerable<SpecificAssetId> SpecificAssetIds { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "submodelDescriptors")]
        IEnumerable<ISubmodelDescriptor> SubmodelDescriptors { get; set; }

        void AddSubmodelDescriptor(ISubmodelDescriptor submodelDescriptor);
    }
}
