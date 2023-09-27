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
    /// <summary>
    /// A property is a data element that has a multi language value. 
    /// </summary>
    public interface IMultiLanguageProperty : ISubmodelElement
    {
        /// <summary>
        /// The value of the property instance.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        LangStringSet Value { get; }

        /// <summary>
        /// Reference to the global unqiue id of a coded value.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueId")]
        IReference ValueId { get; }
    }
}
