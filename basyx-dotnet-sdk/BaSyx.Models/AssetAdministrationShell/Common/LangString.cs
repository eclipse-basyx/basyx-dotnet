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
using System.Xml.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class LangString
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "language")]
        [XmlAttribute("language")]
        public string Language { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "text")]
        [XmlText]
        public string Text { get; set; }

        internal LangString() { }

        public LangString(string language, string text)
        {
            Language = language;
            Text = text;
        }

        public override string ToString()
        {
            return string.Join(":", Language, Text);
        }
    }
}
