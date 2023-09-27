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
        public const string AAS_ASSET_INFORMATION = "/aas/asset-information";
        /// <summary>
        /// Asset Information
        /// </summary>
        public const string AAS_SUBMODELS = "/aas/submodels";
        /// <summary>
        /// Asset Information
        /// </summary>
        public const string AAS_SUBMODELS_BYID = "/aas/submodels/{submodelIdentifier}";

    }
}
