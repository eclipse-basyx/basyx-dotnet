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
    /// A BLOB is a data element that represents a file that is contained with its source code in the value attribute.
    /// </summary>
    public interface IBlob : ISubmodelElement
    {
        /// <summary>
        /// Mime type of the content of the BLOB.  
        ///  The mime type states which file extension the file has. e.g. “application/json”, “application/xls”, ”image/jpg” 
        ///  The allowed values are defined as in RFC2046.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "contentType")]
        string ContentType { get; }

        /// <summary>
        /// The value of the BLOB instance of a blob data element.  
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        string Value { get; }
    }
}
