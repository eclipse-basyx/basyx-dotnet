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
    public class BaSyxPropertyIdentifier : UniformResourceName
    {
        public BaSyxPropertyIdentifier(string propertyName, string version)
            : this(propertyName, version, null, null, null)
        { }

        public BaSyxPropertyIdentifier(string propertyName, string version, string revision, string elementId, string instanceNumber) 
            : base(BaSyxUrnConstants.BASYX_NAMESPACE, BaSyxUrnConstants.BASYX_PROPERTIES, propertyName, version, revision, elementId, instanceNumber)
        { }
    }
}
