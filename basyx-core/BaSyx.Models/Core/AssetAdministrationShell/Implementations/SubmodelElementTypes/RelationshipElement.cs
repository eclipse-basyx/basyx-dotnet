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
    public class RelationshipElement : SubmodelElement, IRelationshipElement
    {
        public override ModelType ModelType => ModelType.RelationshipElement;

        public IReference First { get; set; }

        public IReference Second { get; set; }      

        public RelationshipElement(string idShort) : base(idShort) 
        {
            Get = element => { return new ElementValue(new { First, Second }, new DataType(DataObjectType.AnyType)); };
            Set = (element, value) => { dynamic dVal = value?.Value; First = dVal?.First; Second = dVal?.Second; };
        }     
    }
}
