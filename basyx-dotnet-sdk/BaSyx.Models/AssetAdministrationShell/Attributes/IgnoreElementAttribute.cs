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
using System;

namespace BaSyx.Models.AdminShell
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class IgnoreElementAttribute : Attribute
    {
        public IgnoreElementAttribute() { }
    }
}
