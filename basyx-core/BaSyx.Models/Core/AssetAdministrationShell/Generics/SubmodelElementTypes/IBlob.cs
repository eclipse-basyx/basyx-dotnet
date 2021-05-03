/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Generics
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
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "mimeType")]
        string MimeType { get; }

        /// <summary>
        /// The value of the BLOB instance of a blob data element.  
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        string Value { get; }
    }
}
