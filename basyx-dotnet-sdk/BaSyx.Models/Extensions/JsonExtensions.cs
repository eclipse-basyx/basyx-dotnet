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
using BaSyx.Models.AdminShell;
using System.Text.Json;

namespace BaSyx.Models.Extensions
{
    public static class JsonExtensions
    {
        private static JsonSerializerOptions _defaultSerializerOptions;
        private static JsonSerializerOptions _valueOnlySerializerOptions;
        private static JsonSerializerOptions _valueScopeSerializerOptions;
        static JsonExtensions()
        {
            DefaultJsonSerializerOptions defaultOptions = new DefaultJsonSerializerOptions();
            _defaultSerializerOptions = defaultOptions.Build();

            _valueOnlySerializerOptions = new JsonSerializerOptions()
            {
                Converters = { new SubmodelElementContainerValueOnlyConverter(_defaultSerializerOptions) }
            };

            _valueScopeSerializerOptions = new JsonSerializerOptions()
            {
                Converters =
                {
                    new ValueScopeConverter<ValueScope>(options: new ValueScopeConverterOptions()
                    {
                        SerializationOption = SerializationOption.ValueOnly
                    })
                }
            };
        }

        public static string ToJsonValueOnly(this IElementContainer<ISubmodelElement> container)
        {
            if (container == null)
                return null;

            string json = JsonSerializer.Serialize(container, _valueOnlySerializerOptions);
            return json;
        }

        public static string ToJsonValueOnly(this ValueScope valueScope)
        {
            if (valueScope == null)
                return null;

            string json = JsonSerializer.Serialize<ValueScope>(valueScope, _valueScopeSerializerOptions);
            return json;
        }

        public static TValueScope FromJsonValueOnly<TValueScope>(this string valueOnlyJson) where TValueScope : ValueScope
        {
            if (string.IsNullOrEmpty(valueOnlyJson))
                return default;

            TValueScope valueScope = JsonSerializer.Deserialize<TValueScope>(valueOnlyJson, _valueScopeSerializerOptions);
            return valueScope;
        }
    }
}
