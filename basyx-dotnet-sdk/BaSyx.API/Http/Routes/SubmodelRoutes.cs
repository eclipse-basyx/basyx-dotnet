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
    /// The collection of a all Submodel routes
    /// </summary>
    public static class SubmodelRoutes
    {
        /// <summary>
        /// Root route
        /// </summary>
        public const string SUBMODEL = "/submodel";
        /// <summary>
        /// Submodel table format route
        /// </summary>
        public const string SUBMODEL_TABLE = "/submodel/table";
        /// <summary>
        /// Submodel elements route
        /// </summary>
        public const string SUBMODEL_ELEMENTS = "/submodel/submodel-elements";
        /// <summary>
        /// Submodel elements idShortPath route
        /// </summary>
        public const string SUBMODEL_ELEMENTS_IDSHORTPATH = "/submodel/submodel-elements/{idShortPath}";
        /// <summary>
        /// Submodel operation idShortPath route
        /// </summary>
        public const string SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE = "/submodel/submodel-elements/{idShortPath}/invoke";
        /// <summary>
        /// Submodel file element upload route
        /// </summary>
        public const string SUBMODEL_ELEMENTS_IDSHORTPATH_UPLOAD = "/submodel/submodel-elements/{idShortPath}/upload";
        /// <summary>
        /// Submodel asyncronous operation result route
        /// </summary>
        public const string SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS = "/submodel/submodel-elements/{idShortPath}/operation-results/{handleId}";
    }
}
