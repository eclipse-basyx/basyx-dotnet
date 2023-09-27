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
using BaSyx.Utils.Json;
using Newtonsoft.Json;

namespace BaSyx.Models.Extensions
{
    public static class ModelElementExtensions
    {
        public static string ToJson(this IModelElement modelElement, JsonSerializerSettings jsonSerializerSettings = null)
        {
            jsonSerializerSettings = jsonSerializerSettings ?? new DefaultJsonSerializerSettings();
            return JsonConvert.SerializeObject(modelElement, jsonSerializerSettings);
        }
    }
}
