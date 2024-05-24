/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/

using System.Globalization;
using System.Text.Json.Nodes;

namespace BaSyx.Models.AdminShell
{
    public abstract class ValueScope
    {
        internal static NumberFormatInfo _nfi;
        static ValueScope()
        {
            _nfi = new NumberFormatInfo();
            _nfi.NumberDecimalSeparator = ".";
        }
        public abstract ModelType ModelType { get; }

        public abstract JsonValue ToJson();
       
    }
}
