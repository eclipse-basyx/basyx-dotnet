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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// A qualifiable element may be further qualified by one or more qualifiers.
    /// </summary>
    public interface IQualifiable
    {
        /// <summary>
        /// Additional qualification of a qualifiable element. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "qualifiers")]
        IEnumerable<IQualifier> Qualifiers { get; }
    }
}
