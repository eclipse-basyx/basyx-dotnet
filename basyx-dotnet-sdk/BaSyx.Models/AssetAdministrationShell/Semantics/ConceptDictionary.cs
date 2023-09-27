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

namespace BaSyx.Models.AdminShell
{
    public class ConceptDictionary : IConceptDictionary
    {
        public string IdShort { get; set; }

        public string Category { get; set; }

        public LangStringSet Description { get; set; }
        public LangStringSet DisplayName { get; set; }

        public IReferable Parent { get; set; }

        public Dictionary<string, string> MetaData { get; set; }

        public List<IReference<IConceptDescription>> ConceptDescriptions { get; set; }

        public ModelType ModelType => ModelType.ConceptDictionary;
    }
}
