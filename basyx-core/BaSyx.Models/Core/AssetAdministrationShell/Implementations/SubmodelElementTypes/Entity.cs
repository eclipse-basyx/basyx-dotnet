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
using BaSyx.Models.Core.Common;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
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
