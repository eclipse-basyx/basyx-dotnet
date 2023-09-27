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
using BaSyx.Models.AdminShell;

namespace BaSyx.Models.AdminShell
{
    public class UniformResourceIdentifier : UniformResource
    {
        public UniformResourceIdentifier(string organisation, string subUnit, string domainId, string version, string revision, string elementId, string instanceNumber)
            :base(organisation, subUnit, domainId, version, revision, elementId, instanceNumber)
        { }

        public override Identifier ToIdentifier()
        {
            return new Identifier(ToUri(), KeyType.IRI);
        }
    }
}
