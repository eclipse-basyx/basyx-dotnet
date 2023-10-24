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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// An entity is a submodel element that is used to model entities. 
    /// </summary>
    public interface IEntity : ISubmodelElement
    {
        /// <summary>
        /// Describes statements applicable to the entity by a set of submodel elements, typically with a qualified value.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "statements")]
        IElementContainer<ISubmodelElement> Statements { get; }

        /// <summary>
        /// Describes whether the entity is a comanaged entity or a self-managed entity. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "entityType")]
        EntityType EntityType { get; }

        /// <summary>
        /// Identifier of the asset the Asset Administration Shell is
        /// representing. This attribute is required as soon as the Asset
        /// Administration Shell is exchanged via partners in the life
        /// cycle of the asset. In a first phase of the life cycle, the
        /// asset might not yet have a global asset ID but already an
        /// internal identifier.The internal identifier would be
        /// modelled via "specificAssetId".
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "globalAssetId")]
        Identifier GlobalAssetId { get; }

        /// <summary>
        /// Additional domain-specific, typically proprietary identifier
        /// for the asset like serial number, manufacturer part ID,
        /// customer part IDs, etc.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "specificAssetIds")]
        IEnumerable<SpecificAssetId> SpecificAssetIds { get; }
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
