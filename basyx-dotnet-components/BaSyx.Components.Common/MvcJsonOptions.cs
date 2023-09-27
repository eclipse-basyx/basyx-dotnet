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
using BaSyx.Utils.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BaSyx.Components.Common
{
    public static class MvcJsonOptions
    {
        public static MvcNewtonsoftJsonOptions GetDefaultMvcJsonOptions(this MvcNewtonsoftJsonOptions options, IServiceCollection services)
        {
            options.SerializerSettings.ContractResolver = new DependencyInjectionContractResolver(new DependencyInjectionExtension(services));
            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            options.AllowInputFormatterExceptionMessages = true;

            return options;
        }
    }
}
