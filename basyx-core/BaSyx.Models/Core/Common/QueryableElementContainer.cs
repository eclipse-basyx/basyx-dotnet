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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BaSyx.Models.Core.Common
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
