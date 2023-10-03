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
    /// A range data element is a data element that defines a range with min and max
    /// </summary>
    public interface IRange : ISubmodelElement
    {
        /// <summary>
        /// The minimum value of the range. If the min value is missing then the value is assumed to be negative infinite.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "min")]
        IValue Min { get; }

        /// <summary>
        /// The maximum value of the range. If the max value is missing then the value is assumed to be positive infinite.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "max")]
        IValue Max { get; }

        /// <summary>
        /// Data type of the min und max.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueType")]
        DataType ValueType { get; set; }
    }
}
