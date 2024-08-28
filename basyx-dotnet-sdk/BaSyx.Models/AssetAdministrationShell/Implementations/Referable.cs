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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public abstract class Referable : IReferable
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "idShort")]
        public string IdShort { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "category")]
        public string Category { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "description")]
        public LangStringSet Description { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "displayName")]
        public LangStringSet DisplayName { get; set; }
        [JsonIgnore, IgnoreDataMember]
        public IReferable Parent { get; set; }
        [JsonIgnore, IgnoreDataMember]
        public Dictionary<string, string> MetaData { get; set; }
                 
        protected Referable(string idShort)
        {
            IdShort = idShort;
            MetaData = new Dictionary<string, string>();
            Description = new LangStringSet();
            DisplayName = new LangStringSet();
        }

        public bool ShouldSerializeMetaData()
        {
            if (MetaData?.Count > 0)
                return true;
            else
                return false;
        }

        public bool ShouldSerializeDescription()
        {
            if (Description?.Count > 0)
                return true;
            else
                return false;
        }

        public bool ShouldSerializeDisplayName()
        {
            if (DisplayName?.Count > 0)
                return true;
            else
                return false;
        }
    }
}
