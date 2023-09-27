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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class Entity : SubmodelElement, IEntity
    {
        public override ModelType ModelType => ModelType.Entity;

        public IElementContainer<ISubmodelElement> Statements { get; set; }

        public EntityType EntityType { get; set; }

        public IReference<IAsset> Asset { get; set; }

        public Entity(string idShort) : base(idShort)
        {
            Statements = new ElementContainer<ISubmodelElement>(this);

            Get = element => { return new ElementValue(new { Statements, EntityType, Asset }, new DataType(DataObjectType.AnyType)); };
            Set = (element, value) => { dynamic dVal = value?.Value; Statements = dVal?.Statements; EntityType = dVal?.EntityType; Asset = dVal?.Asset; };
        }
    }
}
