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
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BaSyx.Utils.ResultHandling.ResultTypes
{
    public class PagedResult
    {
        [JsonPropertyName("paging_metadata")]
        public PagingMetadata PagingMetadata { get; set; }

        public object Result { get; set; }
    }

    public class PagedResult<T> : PagedResult
    {
        public new T Result { get; set; }
    }

    public class PagingMetadata
    {
        public string Cursor { get; set; }
    }
}
