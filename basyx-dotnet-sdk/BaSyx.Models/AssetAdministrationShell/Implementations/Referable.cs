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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public abstract class Referable : IReferable
    {
        public string IdShort { get; set; }
        public string Category { get; set; }
        public LangStringSet Description { get; set; }
        public LangStringSet DisplayName { get; set; }
        public IReferable Parent { get; set; }
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
    }
}
