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
    /// A File is a data element that represents an address to a file. The value is an URI that can represent an absolute or relative path. 
    /// </summary>
    public interface IFileElement : ISubmodelElement
    {
        /// <summary>
        /// Mime type of the content of the file. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "contentType")]
        string ContentType { get; }

        /// <summary>
        /// Path and name of the referenced file (with file extension).  The path can be absolute or relative
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        string Value { get; }
    }
}
