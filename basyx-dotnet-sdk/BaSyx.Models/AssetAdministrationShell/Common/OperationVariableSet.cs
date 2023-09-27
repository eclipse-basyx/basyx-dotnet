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
using BaSyx.Models.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace BaSyx.Models.AdminShell
{
    public class OperationVariableSet : List<IOperationVariable>, IOperationVariableSet
    {
        public OperationVariableSet()
        { }

        public ISubmodelElement this[string idShort] => this.Find(e => e.Value?.IdShort == idShort)?.Value;

        public void Add(ISubmodelElement submodelElement)
        {
            int index = this.FindIndex(f => f.Value?.IdShort == submodelElement.IdShort);
            if (index == -1)
                base.Add(new OperationVariable() { Value = submodelElement });
            else
                base[index] = new OperationVariable() { Value = submodelElement };
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
