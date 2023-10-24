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
    /// <summary>
    /// A qualifier is essentially a type-value-pair. Depending on the kind of qualifier, it makes additional statements
    /// • w.r.t.the value of the qualified element,
    /// • w.r.t the concept, i.e.semantic definition of the qualified element,
    /// • w.r.t.existence and other meta information of the qualified element type.
    /// 
    /// Constraint AASd-006: If both, the value and the valueId of a Qualifier are present, the value
    /// needs to be identical to the value of the referenced coded value in Qualifier/valueId.
    /// 
    /// Constraint AASd-020: The value of Qualifier/value shall be consistent with the data type as
    /// defined in Qualifier/valueType.
    /// </summary>
    public interface IQualifier : IHasSemantics
    {
        /// <summary>
        /// The qualifier kind describes the kind of qualifier that is applied to the element.
        /// Default: ConceptQualifier
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "kind")]
        QualifierKind Kind { get; }

        /// <summary>
        /// The qualifier type describes the type of the qualifier that is applied to the element. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "type")]
        string Type { get; }

        /// <summary>
        /// The qualifier value is the value of the qualifier.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Reference to the global unique ID of a coded value
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueId")]
        IReference ValueId { get; }

        /// <summary>
        /// Data type of the qualifier value
        /// </summary>
        DataType ValueType { get; }
    }
}
