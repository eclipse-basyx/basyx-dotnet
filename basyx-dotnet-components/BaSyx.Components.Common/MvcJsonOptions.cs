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
using BaSyx.Models.Extensions;
using BaSyx.Models.Extensions.SystemTextJson;
using BaSyx.Utils.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace BaSyx.Components.Common
{
    public static class MvcJsonOptions
    {
        public static JsonOptions GetDefaultJsonOptions(this JsonOptions options, IServiceCollection services)
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault;
            options.JsonSerializerOptions.Converters.Add(new TypeConverter(new DependencyInjectionExtension(services)));
            options.JsonSerializerOptions.Converters.Add(new FullSubmodelElementConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new DataTypeConverter());
            options.JsonSerializerOptions.Converters.Add(new ModelTypeConverter());
            options.JsonSerializerOptions.Converters.Add(new ValueScopeConverter());
            options.JsonSerializerOptions.Converters.Add(new IdentifierConverter());
            options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { DefaultValueModifier }
            };
            options.AllowInputFormatterExceptionMessages = true;
            
            return options;
        }

        private static void DefaultValueModifier(JsonTypeInfo info)
        {
            foreach (var property in info.Properties)
            {                
                if (DataType.IsGenericList(property.PropertyType))
                {
                    property.ShouldSerialize = (_, val) => val is ICollection collection && collection.Count > 0;
                }
            }
        }
    }
}
