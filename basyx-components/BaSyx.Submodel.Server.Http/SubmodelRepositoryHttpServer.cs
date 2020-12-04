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
using BaSyx.API.Components;
using BaSyx.Components.Common;
using BaSyx.Utils.Settings.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BaSyx.Submodel.Server.Http
{
    public class SubmodelRepositoryHttpServer : ServerApplication
    {
        public SubmodelRepositoryHttpServer() : this(null, null)
        { }

        public SubmodelRepositoryHttpServer(ServerSettings serverSettings) : this(serverSettings, null)
        { }

        public SubmodelRepositoryHttpServer(ServerSettings serverSettings, string[] webHostBuilderArgs)
            : base(serverSettings, webHostBuilderArgs)
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            WebHostBuilder.UseSetting(WebHostDefaults.ApplicationKey, entryAssembly.FullName);
        }

        public void SetServiceProvider(ISubmodelRepositoryServiceProvider submodelRepositoryServiceProvider)
        {
            WebHostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<ISubmodelRepositoryServiceProvider>(submodelRepositoryServiceProvider);
            });
        }       
    }
}
