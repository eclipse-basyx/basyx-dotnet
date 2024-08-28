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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// A submodel element list is an ordered list of submodel elements.
    /// 
    /// Note: the list is ordered although the ordering might not be relevant
    /// (see attribute "orderRelevant".)
    /// 
    /// The numbering starts with Zero(0).
    /// 
    /// Constraint AASd-107: If a first level child element in a SubmodelElementList has a
    /// semanticId, it shall be identical to SubmodelElementList/semanticIdListElement.
    /// 
    /// Constraint AASd-114: If two first level child elements in a SubmodelElementList
    /// have a semanticId, they shall be identical.
    /// 
    /// Constraint AASd-115: If a first level child element in a SubmodelElementList does
    /// not specify a semanticId, the value is assumed to be identical to SubmodelElementList/semanticIdListElement.
    ///     
    /// Constraint AASd-108: All first level child elements in a SubmodelElementList shall
    /// have the same submodel element type as specified in SubmodelElementList/typeValueListElement.
    /// 
    /// Constraint AASd-109: If SubmodelElementList/typeValueListElement is equal to
    /// Property or Range, SubmodelElementList/valueTypeListElement shall be set and
    /// all first level child elements in the SubmodelElementList shall have the value type
    /// as specified in SubmodelElementList/valueTypeListElement.
    /// </summary>
    public interface ISubmodelElementList : ISubmodelElement<SubmodelElementListValue>
    {
        /// <summary>
        /// Defines whether order in list is relevant. If orderRelevant = false, the list represents a set or a bag.
        /// Default: True
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "orderRelevant")]
        bool OrderRelevant { get; set; }

        /// <summary>
        /// Semantic ID which the submodel elements contained in the list match
        /// Note: it is recommended to use an external reference.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "semanticIdListElement")]
        IReference SemanticIdListElement { get; set; }

        /// <summary>
        /// The submodel element type of the submodel elements contained in the list
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "typeValueListElement")]
        ModelType TypeValueListElement { get; set; }

        /// <summary>
        /// The value type of the submodel element contained in the list
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueTypeListElement")]
        DataType ValueTypeListElement { get; set; }
    }
}
