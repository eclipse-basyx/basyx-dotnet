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
using BaSyx.Models.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BaSyx.Models.Core.Common
{
    [JsonConverter(typeof(OperationVariableSetConverter))]
    public interface IOperationVariableSet : IList<IOperationVariable>
    {
        void Add(ISubmodelElement submodelElement);
        ISubmodelElement Get(string idShort);
        IElementContainer<ISubmodelElement> ToElementContainer();
        ISubmodelElement this[string idShort] { get; }
    }
}
