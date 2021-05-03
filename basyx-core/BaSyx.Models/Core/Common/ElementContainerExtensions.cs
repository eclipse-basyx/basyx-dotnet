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

namespace BaSyx.Models.Core.Common
{
    public static class ElementContainerExtensions
    {
        public static IQueryableElementContainer<T> AsQueryableElementContainer<T>(this IEnumerable<T> enumerable, IReferable parent = null) where T: IReferable, IModelElement
        {
            return new QueryableElementContainer<T>(parent, enumerable);
        }        
    }
}
