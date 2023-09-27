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
using BaSyx.Models.AdminShell;
using System.Collections.Generic;

namespace BaSyx.Models.AdminShell
{
    public static class ElementContainerExtensions
    {
        public static IQueryableElementContainer<T> AsQueryableElementContainer<T>(this IEnumerable<T> enumerable, IReferable parent = null) where T: IReferable, IModelElement
        {
            return new QueryableElementContainer<T>(parent, enumerable);
        }        
    }
}
