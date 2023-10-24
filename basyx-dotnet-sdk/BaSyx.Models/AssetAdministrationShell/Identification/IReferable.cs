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
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// An element that is referable by its idShort. This id is not globally unique. This id is unique within the name space of the element.
    /// </summary>
    public interface IReferable
    {
        /// <summary>
        /// In case of identifiables, this attribute is a short name of the element. In case of a referable, this ID is an identifying string of the element within its name space. 
        /// Note: if the element is a property and the property has a semantic definition (HasSemantics/semanticId) conformant to IEC61360, the idShort is typically identical to the short name in English, if available.
        /// </summary>
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false, Name = "idShort")]
        string IdShort { get; }

        /// <summary>
        /// The category is a value that gives further meta information w.r.t.the class of the element. It affects the expected existence of attributes and the applicability of constraints.
        /// 
        /// Note: The category is not identical to the semantic definition(HasSemantics) of an element.The category could e.g.denote that
        /// the element is a measurement value, whereas the semantic definition of the element would denote that it is the measured temperature.
        /// </summary>
        [Obsolete]
        [JsonIgnore, DataMember(EmitDefaultValue = false, IsRequired = false, Name = "category")]
        string Category { get; }

        /// <summary>
        /// Description or comments on the element.
        /// The description can be provided in several languages.
        /// If no description is defined, the definition of the concept description that defines the semantics of the element is used.
        /// Additional information can be provided, e.g. if the element is qualified and which qualifier types can be expected in which context or which additional data specification templates.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "description")]
        LangStringSet Description { get; }

        /// <summary>
        /// Display name; can be provided in several languages
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "displayName")]
        LangStringSet DisplayName { get; }

        [JsonIgnore, IgnoreDataMember]
        IReferable Parent { get; set; }
    }
}
