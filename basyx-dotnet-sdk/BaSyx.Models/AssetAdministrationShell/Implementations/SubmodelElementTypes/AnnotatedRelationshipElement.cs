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

namespace BaSyx.Models.AdminShell
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
