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
using System.Text.Json.Serialization;

namespace BaSyx.Utils.ResultHandling.ResultTypes
{
    public class PagedResult
    {
        [JsonPropertyName("paging_metadata")]
        public PagingMetadata PagingMetadata { get; set; }
        [JsonPropertyName("result")]
        public object Result { get; set; }

        public PagedResult(object result, PagingMetadata pagingMetadata)
        {
            Result = result;
            PagingMetadata = pagingMetadata;
        }

        public PagedResult()
        {
            Result = null;
            PagingMetadata = null;
        }
    }

    public class PagedResult<T> : PagedResult
    {
        [JsonPropertyName("result")]
        public new T Result { get; set; }

        [JsonPropertyName("paging_metadata")]
        public new PagingMetadata PagingMetadata { get; set; }

        [JsonConstructor]
        public PagedResult() { }

        public PagedResult(T result) : base(result, null)
        {
            Result = result;
        }

        public PagedResult(T result, PagingMetadata pagingMetadata) : base(result, pagingMetadata)
        {
            PagingMetadata = pagingMetadata;
            Result = result;
        }

        public static implicit operator PagedResult<T>(T value)
        {
            return new PagedResult<T>(value);
        }
    }

    public class PagingMetadata
    {
        [JsonPropertyName("cursor")]
        public string Cursor { get; set; }

        public PagingMetadata(string cursor)
        {
            Cursor = cursor;
        }

        public bool HasCursor()
        {
            return !string.IsNullOrEmpty(Cursor) && !Cursor.Equals("null");
        }
    }
}
