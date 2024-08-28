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
    public interface IAssetInformation
    {
        /// <summary>
        /// Denotes whether the Asset is of kind “Type” or “Instance”. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "assetKind")]
        AssetKind AssetKind { get; }

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

        /// <summary>
        /// In case AssetInformation/assetKind is applicable the
        /// AssetInformation/assetType is the asset ID of the type
        /// asset of the asset under consideration as identified by
        /// AssetInformation/globalAssetId.
        /// 
        /// Note: in case AssetInformation/assetKind is "Instance"
        /// then the AssetInformation/assetType denotes which "Type" the asset is of.But it is also
        /// possible to have an AssetInformation/assetType of an asset of kind "Type".
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "assetType")]
        Identifier AssetType { get; }

        /// <summary>
        /// Thumbnail of the asset represented by the asset administration shell. Used as default.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "defaultThumbnail")]
        Resource DefaultThumbnail { get; }
    }
}
