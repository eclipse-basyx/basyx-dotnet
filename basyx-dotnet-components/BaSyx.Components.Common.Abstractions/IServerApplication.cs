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
using BaSyx.Utils.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.Components.Common.Abstractions
{
    public interface IServerApplication : IServerApplicationLifetime
    {
        Assembly ControllerAssembly { get; }
        ServerSettings Settings { get; }

        void Configure(Action<IApplicationBuilder> app);
        void ConfigureServices(Action<IServiceCollection> services);
        void ConfigureLogging(LogLevel logLevel);
        void UseContentRoot(string contentRoot);
        void UseWebRoot(string webRoot);
        void UseUrls(params string[] urls);

        void Run();
        Task RunAsync(CancellationToken cancellationToken = default);

        void ProvideContent(Uri relativeUri, Stream content);

    }
}
