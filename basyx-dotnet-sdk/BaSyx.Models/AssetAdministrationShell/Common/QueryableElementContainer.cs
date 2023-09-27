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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BaSyx.Models.AdminShell
{
    public class QueryableElementContainer<TElement> : ElementContainer<TElement>, IQueryableElementContainer<TElement> where TElement : IReferable, IModelElement
    {
        private readonly IQueryable<TElement> _queryable;
        public QueryableElementContainer(IReferable parent, IEnumerable<TElement> list) : base(parent, list)
        {
            _queryable = list.AsQueryable();
        }

        public Type ElementType => _queryable.ElementType;

        public Expression Expression => _queryable.Expression;

        public IQueryProvider Provider => _queryable.Provider;

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }
    }
}
