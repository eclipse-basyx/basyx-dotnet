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
    public class BaSyxShellIdentifier : UniformResourceName
    {
        public BaSyxShellIdentifier(string shellName, string version)
            : this(shellName, version, null, null, null)
        { }

        public BaSyxShellIdentifier(string shellName, string version, string revision, string elementId, string instanceNumber) 
            : base(BaSyxUrnConstants.BASYX_NAMESPACE, BaSyxUrnConstants.BASYX_SHELLS, shellName, version, revision, elementId, instanceNumber)
        { }
    }
}
