/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Constraints
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
