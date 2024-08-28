/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace BaSyx.Utils.ResultHandling.http
{
    public class PaginationHelper<T>
    {
        private readonly Func<T, string> _idResolver;
        private readonly Dictionary<string, T> _map;

        public PaginationHelper(Dictionary<string, T> map, Func<T, string> idResolver)
        {
            _map = map;
            _idResolver = idResolver;
        }

        public PagedResult GetPaged(int limit, PagingMetadata pagingMetadata)
        {
            var cursorView = GetCursorView(limit, pagingMetadata);
            IEnumerable<T> items = cursorView.Values;
            var results = ApplyLimit(limit, items);

            IEnumerable<T> resultList = results.ToList();
            string cursor = null;
            if (limit > 0 && resultList.Count() == limit)
                cursor = ComputeNextCursor(results, items, limit);

            return new PagedResult<IEnumerable<T>>(resultList, new PagingMetadata(cursor));
        }

        private static IEnumerable<T> ApplyLimit(int limit, IEnumerable<T> tStream)
        {
            if (limit != 0)
                return tStream.Take(limit);

            return tStream;
        }

        private string ComputeNextCursor(IEnumerable<T> results, IEnumerable<T> items, int limit)
        {
            if (limit == 0)
                return null;

            if (!results.Any())
                return null;

            var cursorResults = ApplyLimit(limit + 1, items);

            if (cursorResults.Count() <= results.Count())
                return null;

            return _idResolver.Invoke(cursorResults.LastOrDefault()); ;
        }

        private Dictionary<string, T> GetCursorView(int limit, PagingMetadata pagingMetadata)
        {
            if (pagingMetadata.HasCursor())
            {
                var cursorElement = _map.SkipWhile(entry => !entry.Key.Equals(pagingMetadata.Cursor));
                return new Dictionary<string, T>(cursorElement.ToDictionary(pair => pair.Key, pair => pair.Value));
            }

            return _map;
        }
    }
}