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
using System;
using BaSyx.Models.AdminShell;

namespace BaSyx.Models.AdminShell
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class DisplayNameAttribute : Attribute
    {
        public LangString DisplayName { get; }
        public DisplayNameAttribute(string language, string text)
        {
            DisplayName = new LangString(language, text);
        }
    }
}
