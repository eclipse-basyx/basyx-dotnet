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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
	public class EntityValue : ValueScope
    {
		public override ModelType ModelType => ModelType.Entity;

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
        public Identifier GlobalAssetId { get; set; }

        /// <summary>
        /// Additional domain-specific, typically proprietary identifier
        /// for the asset like serial number, manufacturer part ID,
        /// customer part IDs, etc.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "specificAssetIds")]
        public IEnumerable<SpecificAssetId> SpecificAssetIds { get; set; }

        private readonly IElementContainer<ISubmodelElement> _statements;

        /// <summary>
        /// Describes statements applicable to the entity by a set of submodel elements, typically with a qualified value.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "statements")]
        public IElementContainer<ISubmodelElement> Statements { get => _statements; set => _statements.AddRange(value); }

        public EntityValue() : base() 
        {
            SpecificAssetIds = new List<SpecificAssetId>();
            _statements = new ElementContainer<ISubmodelElement>();
        }
		public EntityValue(Identifier globalAssetId, IEnumerable<SpecificAssetId> specificAssetIds, IElementContainer<ISubmodelElement> statements)
		{
			GlobalAssetId = globalAssetId;
            SpecificAssetIds = specificAssetIds;
            _statements = statements;
		}
	}
}
