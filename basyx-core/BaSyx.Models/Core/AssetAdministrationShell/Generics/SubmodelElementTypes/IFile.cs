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
    /// A File is a data element that represents an address to a file. The value is an URI that can represent an absolute or relative path. 
    /// </summary>
    public interface IFile : ISubmodelElement
    {
        /// <summary>
        /// Mime type of the content of the file. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "mimeType")]
        string MimeType { get; }

        /// <summary>
        /// Path and name of the referenced file (with file extension).  The path can be absolute or relative
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        string Value { get; }
    }
}
