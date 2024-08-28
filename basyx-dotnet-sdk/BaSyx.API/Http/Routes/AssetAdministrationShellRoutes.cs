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

namespace BaSyx.API.Http
{
    /// <summary>
    /// The collection of a all Asset Adminstration Shell routes
    /// </summary>
    public static class AssetAdministrationShellRoutes
    {
        /// <summary>
        /// Root route
        /// </summary>
        public const string AAS = "/aas";
        /// <summary>
        /// Asset Information
        /// </summary>
        public const string AAS_ASSET_INFORMATION = "/asset-information";
        /// <summary>
        /// Asset Information Thumbnail
        /// </summary>
        public const string AAS_ASSET_INFORMATION_THUMBNAIL = "/asset-information/thumbnail";
        /// <summary>
        /// Submodels
        /// </summary>
        public const string AAS_SUBMODELS = "/submodels";
        /// <summary>
        /// Submodels References
        /// </summary>
        public const string AAS_SUBMODEL_REFS = "/submodel-refs";
        /// <summary>
        /// Submodels References by id
        /// </summary>
        public const string AAS_SUBMODEL_REFS_BYID = "/submodel-refs/{submodelIdentifier}";
        /// <summary>
        /// Submodels by id
        /// </summary>
        public const string AAS_SUBMODELS_BYID = "/submodels/{submodelIdentifier}";

    }
}
