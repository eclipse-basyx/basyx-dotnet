/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
	public class AnnotatedRelationshipElementValue : RelationshipElementValue
    {
		public override ModelType ModelType => ModelType.AnnotatedRelationshipElement;

        /// <summary>
        /// Annotations that hold for the relationships between the two elements.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "annotation")]
        public IElementContainer<ISubmodelElement> Annotations { get; set; }

        public AnnotatedRelationshipElementValue() : base() 
        {
            Annotations = new ElementContainer<ISubmodelElement>();
        }
		public AnnotatedRelationshipElementValue(IReference first, IReference second, IElementContainer<ISubmodelElement> annotations) : base(first, second)
		{
			Annotations = annotations;
		}
	}
}
