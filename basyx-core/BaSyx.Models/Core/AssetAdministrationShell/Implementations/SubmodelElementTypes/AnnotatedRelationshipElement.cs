/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    public class AnnotatedRelationshipElement : RelationshipElement, IAnnotatedRelationshipElement
    {
        public override ModelType ModelType => ModelType.AnnotatedRelationshipElement;
        public IReference<ISubmodelElement> Annotation { get; set; }

        public AnnotatedRelationshipElement(string idShort) : base(idShort) 
        {
            Get = element => { return new ElementValue(Annotation, new DataType(DataObjectType.AnyType)); };
            Set = (element, value) => { Annotation = value.Value as IReference<ISubmodelElement>; };
        }     
    }
}
