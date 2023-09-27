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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public static class QualifierType
    {
        public const string ExpressionLogic = "ExpressionLogic";
        public const string ExpressionSemantic = "ExpressionSemantic";
        public const string Enumeration = "Enumeration";
        public const string Owner = "Owner";
        public const string Min = "Min";
        public const string Max = "Max";
        public const string StrLen = "StrLen";
        public const string MimeType = "MimeType";
        public const string RegEx = "RegEx";
        public const string Existence = "Existence";
    }
}
