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
    /// A range data element is a data element that defines a range with min and max
    /// </summary>
    public interface IRange : ISubmodelElement<RangeValue>
    {      
        /// <summary>
        /// Data type of the min und max.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueType")]
        DataType ValueType { get; set; }
    }
}
