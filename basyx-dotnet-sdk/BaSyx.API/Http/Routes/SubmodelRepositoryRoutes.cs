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
    /// The collection of a all Submodel Repository routes
    /// </summary>
    public static class SubmodelRepositoryRoutes
    {
        /// <summary>
        /// Root route
        /// </summary>
        public const string SUBMODELS = "/submodels";
        /// <summary>
        /// Submodel by id
        /// </summary>
        public const string SUBMODEL_BYID = "/submodels/{submodelIdentifier}";
    }
}
