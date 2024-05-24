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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    public enum QualifierKind
    {
        /// <summary>
        /// Qualifies the value of the element; the corresponding qualifier value can change over time. Value qualifiers are only applicable to elements with kind = "Instance".
        /// </summary>
        [EnumMember(Value = "ValueQualifier")]
        ValueQualifier,
        /// <summary>
        /// Qualifies the semantic definition (HasSemantics/semanticId) the element is referring to; the corresponding qualifier value is static.
        /// </summary>
        [EnumMember(Value = "ConceptQualifier")]
        ConceptQualifier,
        /// <summary>
        /// Qualifies the elements within a specific submodel on concept level; the corresponding qualifier value is static.
        /// Note: template qualifiers are only applicable to elements with kind="Template". See constraint AASd-129.
        /// </summary>
        [EnumMember(Value = "TemplateQualifier")]
        TemplateQualifier     
    }
}
