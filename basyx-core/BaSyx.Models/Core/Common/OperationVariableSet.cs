/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace BaSyx.Models.Core.Common
{
    public class OperationVariableSet : List<IOperationVariable>, IOperationVariableSet
    {
        public OperationVariableSet()
        { }

        public OperationVariableSet(IEnumerable<IOperationVariable> list) : base(list) { }

        public ISubmodelElement this[string idShort] => this.Find(e => e.Value.IdShort == idShort)?.Value;

        public void Add(ISubmodelElement submodelElement)
        {
            base.Add(new OperationVariable() { Value = submodelElement });
        }

        public ISubmodelElement Get(string idShort)
        {
            return this[idShort];
        }

        public IElementContainer<ISubmodelElement> ToElementContainer()
        {
            return new ElementContainer<ISubmodelElement>(null, this.Cast<IOperationVariable>().Select(s => s.Value));
        }
    }
}
