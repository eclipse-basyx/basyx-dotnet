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
    public class BaSyxEventIdentifier : UniformResourceName
    {
        public BaSyxEventIdentifier(string eventName, string version)
            : this(eventName, version, null, null, null)
        { }

        public BaSyxEventIdentifier(string eventName, string version, string revision, string elementId, string instanceNumber) 
            : base(BaSyxUrnConstants.BASYX_NAMESPACE, BaSyxUrnConstants.BASYX_EVENTS, eventName, version, revision, elementId, instanceNumber)
        { }
    }
}
