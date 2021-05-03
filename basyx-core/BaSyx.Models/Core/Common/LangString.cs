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
using System.Xml.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell
{
    [DataContract]
    public class LangString
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "language")]
        [XmlAttribute("lang")]
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
