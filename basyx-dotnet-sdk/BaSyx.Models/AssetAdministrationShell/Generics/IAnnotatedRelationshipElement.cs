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
    /// <summary>
    /// An annotated relationship element is a relationship element that can be annotated with additional data elements. 
    /// </summary>
    public interface IAnnotatedRelationshipElement : IRelationshipElement
    {
        /// <summary>
        /// Annotations that hold for the relationships between the two elements.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "annotation")]
        IElementContainer<ISubmodelElement> Annotations { get; set; }
    }
}
