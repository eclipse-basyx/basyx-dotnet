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

namespace BaSyx.Models.Connectivity
{

    [DataContract]
    public class SubmodelDescriptor : Descriptor, ISubmodelDescriptor
    {
        public IReference SemanticId { get; set; }    
        public override ModelType ModelType => ModelType.SubmodelDescriptor;

        [JsonConstructor]
        public SubmodelDescriptor(IEnumerable<IEndpoint> endpoints) : base (endpoints)
        { }

        public SubmodelDescriptor(ISubmodel submodel, IEnumerable<IEndpoint> endpoints) : this(endpoints)
        {
            IdShort = submodel.IdShort;
            Identification = submodel.Identification;
            Administration = submodel.Administration;
            Description = submodel.Description;
            DisplayName = submodel.DisplayName;
            SemanticId = submodel.SemanticId;
        }
    }
}
