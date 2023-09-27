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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
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
