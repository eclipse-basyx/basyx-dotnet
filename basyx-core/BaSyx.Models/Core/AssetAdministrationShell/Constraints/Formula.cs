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
using BaSyx.Models.Core.Common;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BaSyx.Models.Core.AssetAdministrationShell.Constraints
{
    public class Formula : Constraint, IFormula
    {
        public IEnumerable<IReference> DependsOn { get; set; }
        public override ModelType ModelType => ModelType.Formula;

        [JsonConstructor]
        public Formula()
        {
            DependsOn = new List<IReference>();
        }
    }
}
