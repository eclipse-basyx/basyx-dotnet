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
	public class BlobValue : ValueScope
	{
		public override ModelType ModelType => ModelType.Blob;

        /// <summary>
        /// Mime type of the content of the BLOB.  
        ///  The mime type states which file extension the file has. e.g. “application/json”, “application/xls”, ”image/jpg” 
        ///  The allowed values are defined as in RFC2046.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// The value of the BLOB instance of a blob data element.  
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public string Value { get; set; }

        public BlobValue() { }
		public BlobValue(string contentType, string value)
		{
            ContentType = contentType;
			Value = value;
		}
	}
}
