/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.Models.AdminShell;
using System.Text.Json;

namespace BaSyx.Models.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJsonValueOnly(this ValueScope valueScope)
        {
            if (valueScope == null)
                return null;

            string json = JsonSerializer.Serialize<ValueScope>(valueScope, new JsonSerializerOptions()
            {
                Converters =
                {
                    new ValueScopeConverter<ValueScope>(options: new ValueScopeConverterOptions()
                    {
                        SerializationOption = SerializationOption.ValueOnly
                    })
                }
            });
            return json;
        }

        public static TValueScope FromJsonValueOnly<TValueScope>(this string valueOnlyJson) where TValueScope : ValueScope
        {
            if (string.IsNullOrEmpty(valueOnlyJson))
                return default;

            TValueScope valueScope = JsonSerializer.Deserialize<TValueScope>(valueOnlyJson, new JsonSerializerOptions()
            {
                Converters =
                {
                    new ValueScopeConverter<ValueScope>(options: new ValueScopeConverterOptions()
                    {
                        SerializationOption = SerializationOption.ValueOnly
                    })
                }
            });
            return valueScope;
        }
    }
}
