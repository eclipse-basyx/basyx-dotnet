/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
    /// An element that has a globally unique identifier.  
    /// </summary>
    public interface IIdentifiable : IReferable
    {
        /// <summary>
        /// The globally unique identification of the element. 
        /// </summary>
        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false, Name = "id")]
        Identifier Id { get; }

        /// <summary>
        /// Administrative information of an identifiable element.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "administration")]
        AdministrativeInformation Administration { get; }
    }
}
