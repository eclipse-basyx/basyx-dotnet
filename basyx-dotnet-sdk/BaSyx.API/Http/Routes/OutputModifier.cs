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

namespace BaSyx.API.Http
{
    /// <summary>
    /// The collection of a all output modifier appendix
    /// </summary>
    public static class OutputModifier
    {
        public const string METADATA = "/$metadata";
        public const string VALUE = "/$value";
        public const string REFERENCE = "/$reference";
        public const string PATH = "/$path";
    }
}
