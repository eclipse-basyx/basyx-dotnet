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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// An entity is a submodel element that is used to model entities. 
    /// </summary>
    public interface IEntity : ISubmodelElement<EntityValue>
    {       
        /// <summary>
        /// Describes whether the entity is a comanaged entity or a self-managed entity. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "entityType")]
        EntityType EntityType { get; }    
    }

    /// <summary>
    /// Enumeration for denoting whether an entity is a self-managed entity or a comanaged entity. 
    /// </summary>
    [DataContract]
    public enum EntityType
    {
        [EnumMember(Value = "None")]
        None,
        [EnumMember(Value = "CoManagedEntity")]
        CoManagedEntity,
        [EnumMember(Value = "SelfManagedEntity")]
        SelfManagedEntity
    }
}
