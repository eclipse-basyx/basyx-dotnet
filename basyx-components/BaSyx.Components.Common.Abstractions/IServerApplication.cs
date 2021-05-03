/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
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
