﻿/*******************************************************************************
* Copyright (c) 2022 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
namespace BaSyx.Models.AdminShell
{
    public class BaSyxSubmodelIdentifier : UniformResourceName
    {
        public BaSyxSubmodelIdentifier(string submodelName, string version)
            : this(submodelName, version, null, null, null)
        { }

        public BaSyxSubmodelIdentifier(string submodelName, string version, string revision, string elementId, string instanceNumber) 
            : base(BaSyxUrnConstants.BASYX_NAMESPACE, BaSyxUrnConstants.BASYX_SUBMODELS, submodelName, version, revision, elementId, instanceNumber)
        { }
    }
}
