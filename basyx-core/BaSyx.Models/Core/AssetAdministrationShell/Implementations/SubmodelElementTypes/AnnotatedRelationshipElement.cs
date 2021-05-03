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
using BaSyx.Models.Core.Common;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    public class AnnotatedRelationshipElement : RelationshipElement, IAnnotatedRelationshipElement
    {
        public override ModelType ModelType => ModelType.AnnotatedRelationshipElement;
        public IElementContainer<ISubmodelElement> Annotations { get; set; }

        public AnnotatedRelationshipElement(string idShort) : base(idShort) 
        {
            Annotations = new ElementContainer<ISubmodelElement>(this);

            Get = element => { return new ElementValue(Annotations, new DataType(DataObjectType.AnyType)); };
            Set = (element, value) => { Annotations = value.Value as IElementContainer<ISubmodelElement>; };
        }     
    }
}
