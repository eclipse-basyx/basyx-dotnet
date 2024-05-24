﻿/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
namespace BaSyx.Discovery.mDNS
{
    public static class ServiceTypes
    {
        public const string AAS_SERVICE_TYPE = "_aas_service._tcp";
        public const string REGISTRY_SERVICE_TYPE = "_registry_service._tcp";
    }
}