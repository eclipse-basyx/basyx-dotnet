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
    /// The collection of a all Description routes
    /// </summary>
    public static class DescriptionRoutes
    {
        /// <summary>
        /// Root route
        /// </summary>
        public const string DESCRIPTION = "/description";

        /// <summary>
        /// Root route
        /// </summary>
        public const string DESCRIPTOR = "/descriptor";
    }

    public static class Profiles
    {
        public const string AAS_SERVICE_FULL_SSP_001 = "https://admin-shell.io/aas/API/3/0/AssetAdministrationShellServiceSpecification/SSP-001";
        public const string AAS_SERVICE_READ_SSP_002 = "https://admin-shell.io/aas/API/3/0/AssetAdministrationShellServiceSpecification/SSP-002";
        public const string SUBMODEL_SERVICE_FULL_SSP_001 = "https://admin-shell.io/aas/API/3/0/SubmodelServiceSpecification/SSP-001";
        public const string SUBMODEL_SERVICE_VALUE_SSP_002 = "https://admin-shell.io/aas/API/3/0/SubmodelServiceSpecification/SSP-002";
        public const string SUBMODEL_SERVICE_READ_SSP_003 = "https://admin-shell.io/aas/API/3/0/SubmodelServiceSpecification/SSP-003";
        public const string AASX_SERVICE_FULL_SSP_001 = "https://admin-shell.io/aas/API/3/0/AasxFileServerServiceSpecification/SSP-001";
        public const string AAS_REGISTRY_SERVICE_FULL_SSP_001 = "https://admin-shell.io/aas/API/3/0/AssetAdministrationShellRegistryServiceSpecification/SSP-001";
        public const string AAS_REGISTRY_SERVICE_READ_SSP_002 = "https://admin-shell.io/aas/API/3/0/AssetAdministrationShellRegistryServiceSpecification/SSP-002";
        public const string SUBMODEL_REGISTRY_SERVICE_FULL_SSP_001 = "https://admin-shell.io/aas/API/3/0/SubmodelRegistryServiceSpecification/SSP-001";
        public const string SUBMODEL_REGISTRY_SERVICE_READ_SSP_002 = "https://admin-shell.io/aas/API/3/0/SubmodelRegistryServiceSpecification/SSP-002";
        public const string DISCOVERY_SERVICE_FULL_SSP_001 = "https://admin-shell.io/aas/API/3/0/DiscoveryServiceSpecification/SSP-001";
        public const string AAS_REPO_SERVICE_FULL_SSP_001 = "https://admin-shell.io/aas/API/3/0/AssetAdministrationShellRepositoryServiceSpecification/SSP-001";
        public const string AAS_REPO_SERVICE_READ_SSP_002 = "https://admin-shell.io/aas/API/3/0/AssetAdministrationShellRepositoryServiceSpecification/SSP-002";
        public const string SUBMODEL_REPO_SERVICE_FULL_SSP_001 = "https://admin-shell.io/aas/API/3/0/SubmodelRepositoryServiceSpecification/SSP-001";
        public const string SUBMODEL_REPO_SERVICE_READ_SSP_002 = "https://admin-shell.io/aas/API/3/0/SubmodelRepositoryServiceSpecification/SSP-002";
        public const string SUBMODEL_REPO_TEMPLATE_SERVICE_FULL_SSP_003 = "https://admin-shell.io/aas/API/3/0/SubmodelRepositoryServiceSpecification/SSP-003";
        public const string SUBMODEL_REPO_TEMPLATE_SERVICE_READ_SSP_004 = "https://admin-shell.io/aas/API/3/0/SubmodelRepositoryServiceSpecification/SSP-004";
        public const string CONCEPTDESCRIPTION_REPO_SERVICE_FULL_SSP_001 = "https://admin-shell.io/aas/API/3/0/ConceptDescriptionServiceSpecification/SSP-001";
    }
}
