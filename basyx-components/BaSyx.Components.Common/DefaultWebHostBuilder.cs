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
using BaSyx.Utils.Settings.Types;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BaSyx.Components.Common
{
    public static class DefaultWebHostBuilder
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args, ServerSettings settings)
        {
            IWebHostBuilder webHostBuilder = WebHost.CreateDefaultBuilder(args);            
                   
            if (settings?.ServerConfig.Hosting?.Environment != null)
                webHostBuilder.UseEnvironment(settings.ServerConfig.Hosting.Environment);
            else
                webHostBuilder.UseEnvironment(Environments.Development);

            if (settings?.ServerConfig?.Hosting?.Urls?.Count > 0)
                webHostBuilder.UseUrls(settings.ServerConfig.Hosting.Urls.ToArray());

            return webHostBuilder;
        }
    }
}
