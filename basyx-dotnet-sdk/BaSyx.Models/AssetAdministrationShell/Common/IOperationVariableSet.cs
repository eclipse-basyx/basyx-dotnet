/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
    public interface IOperationVariableSet : IList<IOperationVariable>
    {
        void Add(ISubmodelElement submodelElement);
        ISubmodelElement Get(string idShort);
        IElementContainer<ISubmodelElement> ToElementContainer();
        ISubmodelElement this[string idShort] { get; }
    }
}
