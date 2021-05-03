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
using System.Linq;

namespace BaSyx.Models.Core.Common
{
    public interface IQueryableElementContainer<TElement> : IElementContainer<TElement>, IQueryable<TElement> where TElement : IReferable, IModelElement
    {

    }
}
