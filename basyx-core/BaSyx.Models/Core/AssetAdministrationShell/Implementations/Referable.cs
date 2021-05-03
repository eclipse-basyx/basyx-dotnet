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

using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract]
    public abstract class Referable : IReferable
    {
        public string IdShort { get; set; }
        public string Category { get; set; }
        public LangStringSet Description { get; set; }
        public IReferable Parent { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
                 
        protected Referable(string idShort)
        {
            IdShort = idShort;
            MetaData = new Dictionary<string, string>();
        }

        public bool ShouldSerializeMetaData()
        {
            if (MetaData?.Count > 0)
                return true;
            else
                return false;
        }
    }
}
