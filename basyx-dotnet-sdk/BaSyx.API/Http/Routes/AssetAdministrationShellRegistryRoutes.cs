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
    /// The collection of a all Asset Adminstration Shell Registry routes
    /// </summary>
    public static class AssetAdministrationShellRegistryRoutes
    {
        /// <summary>
        /// Root route
        /// </summary>
        public const string SHELL_DESCRIPTORS = "/registry/shell-descriptors";
        /// <summary>
        /// Specific Asset Administration Shell Descriptor
        /// </summary>
        public const string SHELL_DESCRIPTOR_ID = "/registry/shell-descriptors/{aasIdentifier}";
        /// <summary>
        /// Submodel Descriptors
        /// </summary>
        public const string SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTORS = "/registry/shell-descriptors/{aasIdentifier}/submodel-descriptors";
        /// <summary>
        /// Specific Submodel Descriptor
        /// </summary>
        public const string SHELL_DESCRIPTOR_ID_SUBMODEL_DESCRIPTOR_ID = "/registry/shell-descriptors/{aasIdentifier}/submodel-descriptors/{submodelIdentifier}";
    }
}
