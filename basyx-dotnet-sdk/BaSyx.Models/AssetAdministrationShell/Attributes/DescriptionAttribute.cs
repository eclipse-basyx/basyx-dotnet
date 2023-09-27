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
    public sealed class DescriptionAttribute : Attribute
    {
        public LangString Description { get; }
        public DescriptionAttribute(string language, string text)
        {
            Description = new LangString(language, text);
        }
    }
}
