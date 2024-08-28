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
using BaSyx.Utils.Settings;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Linq;

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

            if (!args.Contains("--urls") && settings?.ServerConfig?.Hosting?.Urls?.Count > 0)
                webHostBuilder.UseUrls(settings.ServerConfig.Hosting.Urls.ToArray());

            return webHostBuilder;
        }
    }
}
