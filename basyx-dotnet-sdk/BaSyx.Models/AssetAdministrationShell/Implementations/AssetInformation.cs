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
using System.Linq;

namespace BaSyx.Models.AdminShell
{
    public class AssetInformation : IAssetInformation
    {
        public AssetKind AssetKind { get; set; }

        public Identifier AssetType { get; set; }

        public Identifier GlobalAssetId { get; set; }

        public IEnumerable<SpecificAssetId> SpecificAssetIds { get; set; }

        public Resource DefaultThumbnail { get; set; }               

        public AssetInformation()
        {
            SpecificAssetIds = new List<SpecificAssetId>();
        }

        public bool ShouldSerializeSpecificAssetIds()
        {
            if (SpecificAssetIds?.Count() > 0)
                return true;
            else
                return false;
        }
    }
}
