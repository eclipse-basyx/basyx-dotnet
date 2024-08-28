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
	public class FileElementValue : ValueScope
	{
		public override ModelType ModelType => ModelType.File;

        /// <summary>
        /// Mime type of the content of the file. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Path and name of the referenced file (with file extension).  The path can be absolute or relative
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public string Value { get; set; }

        public FileElementValue() { }
		public FileElementValue(string contentType, string value)
		{
            ContentType = contentType;
			Value = value;
		}
	}
}
