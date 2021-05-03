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
using System.Collections.Generic;
using System.Runtime.Serialization;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.Common;
using Newtonsoft.Json;

namespace BaSyx.Models.Core.AssetAdministrationShell.Views
{
    [DataContract]
    public class View : IView
    {
        public IEnumerable<IReference> ContainedElements { get; set; }

        public IReference SemanticId { get; set; }

        public string IdShort { get; set; }

        public string Category { get; set; }

        public LangStringSet Description { get; set; }

        public IReferable Parent { get; set; }
        public Dictionary<string, string> MetaData { get; set; }

        public ModelType ModelType => ModelType.View;

        [JsonConstructor]
        public View()
        {
            ContainedElements = new List<IReference>();
            MetaData = new Dictionary<string, string>();
        }
    }
}
