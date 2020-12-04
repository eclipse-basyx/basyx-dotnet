/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
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
