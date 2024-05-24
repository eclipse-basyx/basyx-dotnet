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
    /// A property is a data element that has a single value. 
    /// </summary>
    public interface IProperty : ISubmodelElement<PropertyValue>, IGetSet
    {
        /// <summary>
        /// Reference to the global unique ID of a coded value
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueId")]
        IReference ValueId { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "valueType")]
        DataType ValueType { get; }
    }
}
