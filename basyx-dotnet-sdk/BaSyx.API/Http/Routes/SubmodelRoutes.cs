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
        public const string SUBMODEL_TABLE = "/table";
        /// <summary>
        /// Submodel elements route
        /// </summary>
        public const string SUBMODEL_ELEMENTS = "/submodel-elements";
        /// <summary>
        /// Submodel elements idShortPath route
        /// </summary>
        public const string SUBMODEL_ELEMENTS_IDSHORTPATH = "/submodel-elements/{idShortPath}";
        /// <summary>
        /// Submodel operation idShortPath route
        /// </summary>
        public const string SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE = "/submodel-elements/{idShortPath}/invoke";
        /// <summary>
        /// Submodel operation idShortPath route
        /// </summary>
        public const string SUBMODEL_ELEMENTS_IDSHORTPATH_INVOKE_ASYNC = "/submodel-elements/{idShortPath}/invoke-async";
        /// <summary>
        /// Submodel file element upload route
        /// </summary>
        public const string SUBMODEL_ELEMENTS_IDSHORTPATH_ATTACHMENT = "/submodel-elements/{idShortPath}/attachment";
        /// <summary>
        /// Submodel asyncronous operation status route
        /// </summary>
        public const string SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_STATUS = "/submodel-elements/{idShortPath}/operation-status/{handleId}";
        /// <summary>
        /// Submodel asyncronous operation result route
        /// </summary>
        public const string SUBMODEL_ELEMENTS_IDSHORTPATH_OPERATION_RESULTS = "/submodel-elements/{idShortPath}/operation-results/{handleId}";
    }
}
