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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class Entity : SubmodelElement, IEntity
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.Entity;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "statements")]
        public IElementContainer<ISubmodelElement> Statements { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "entityType")]
        public EntityType EntityType { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "globalAssetId")]
        public Identifier GlobalAssetId { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "specificAssetIds")]
        public IEnumerable<SpecificAssetId> SpecificAssetIds { get; set; }

        public Entity(string idShort) : base(idShort)
        {
            SpecificAssetIds = new List<SpecificAssetId>();
            Statements = new ElementContainer<ISubmodelElement>(this);
        }
    }
}
