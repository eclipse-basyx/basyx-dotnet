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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell
{
    [DataContract]
    public class LangStringSet : List<LangString>
    {
        [JsonConstructor]
        public LangStringSet(IEnumerable<LangString> langStrings) : base(langStrings) { }
       
        public LangStringSet() { }

        public string this[string language] => this.Find(e => e.Language == language)?.Text;

        public LangStringSet AddLangString(string language, string text) => AddLangString(new LangString(language, text));

        public LangStringSet AddLangString(LangString langString)
        {
            int index = this.FindIndex(c => c.Language == langString.Language);
            if (index == -1)
                Add(langString);
            else
                this[index] = langString;

            return this;
        }

        public override string ToString()
        {
            return string.Join(";", this);
        }

    }
}
