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
namespace BaSyx.Models.AdminShell
{
    public class BaSyxAssetIdentifier : UniformResourceName
    {
        public BaSyxAssetIdentifier(string assetName, string version)
            : this(assetName, version, null, null, null)
        { }

        public BaSyxAssetIdentifier(string assetName, string version, string revision, string elementId, string instanceNumber) 
            : base(BaSyxUrnConstants.BASYX_NAMESPACE, BaSyxUrnConstants.BASYX_ASSETS, assetName, version, revision, elementId, instanceNumber)
        { }
    }
}
