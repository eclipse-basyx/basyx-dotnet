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
using System.Text.Json.Serialization;

namespace BaSyx.Utils.ResultHandling.ResultTypes
{
    public class PagedResult
    {
        [JsonPropertyName("paging_metadata")]
        public PagingMetadata PagingMetadata { get; set; }
        [JsonPropertyName("result")]
        public object Result { get; set; }
    }

    public class PagedResult<T> : PagedResult
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Never), JsonPropertyName("result")]
        public new T Result { get; set; }

        [JsonConstructor]
        public PagedResult() { }

        public PagedResult(T result) { Result = result; }


        public static implicit operator PagedResult<T>(T value)
        {
            return new PagedResult<T>(value);
        }
    }

    public class PagingMetadata
    {
        public string Cursor { get; set; }
    }
}
