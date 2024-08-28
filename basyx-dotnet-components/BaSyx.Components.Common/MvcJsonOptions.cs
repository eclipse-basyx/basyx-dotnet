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
using BaSyx.Models.Extensions;
using BaSyx.Utils.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BaSyx.Components.Common
{
    public static class MvcJsonOptions
    {
        public static JsonOptions GetDefaultJsonOptions(this JsonOptions options, IServiceCollection services)
        {
            DefaultJsonSerializerOptions jsonOptions = new DefaultJsonSerializerOptions();
            jsonOptions.AddDependencyInjection(new DependencyInjectionExtension(services));
            jsonOptions.AddFullSubmodelElementConverter();
            var jsonSerializerOptions = jsonOptions.Build();

            options.JsonSerializerOptions.DefaultIgnoreCondition = jsonSerializerOptions.DefaultIgnoreCondition;
            options.JsonSerializerOptions.PropertyNamingPolicy = jsonSerializerOptions.PropertyNamingPolicy;
            options.JsonSerializerOptions.TypeInfoResolver = jsonSerializerOptions.TypeInfoResolver;
            foreach(var converter in jsonSerializerOptions.Converters)
                options.JsonSerializerOptions.Converters.Add(converter);


            options.AllowInputFormatterExceptionMessages = true;

            return options;
        }
    }
}
