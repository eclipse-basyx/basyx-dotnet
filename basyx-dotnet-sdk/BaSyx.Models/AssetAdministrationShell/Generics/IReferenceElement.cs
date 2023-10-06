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
    /// <summary>
    /// A reference element is a data element that defines a logical reference to another element within the same or another AAS or a reference to an external object or entity. 
    /// </summary>
    public interface IReferenceElement : ISubmodelElement
    {
        /// <summary>
        /// Reference to any other referable element of the same of any other AAS or a reference to an external object or entity. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        IReference Value { get; set; }
    }

    public interface IReferenceElement<T> where T : IReferable
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        IReference<T> Value { get; set; }
    }
}

