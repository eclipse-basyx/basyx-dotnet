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
using BaSyx.Components.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BaSyx.Common.UI
{
    public static class PageNames
    {
        public const string AssetAdministrationShellServer = "AssetAdministrationShell";
        public const string AssetAdministrationShellRegistryServer = "AssetAdministrationShellRegistry";
        public const string AssetAdministrationShellRepositoryServer = "AssetAdministrationShellRepository";
        
        public const string SubmodelServer = "Submodel";
        public const string SubmodelRepositoryServer = "SubmodelRepository";        
    }
    public static class CommonUIExtensions
    {
        public static void AddBaSyxUI(this IServerApplication serverApp, string pageName)
        {
            serverApp.ConfigureServices(services =>
            {
                services.AddBaSyxUI(pageName);
            });            
        }

        public static void AddBaSyxUI(this IServiceCollection services, string pageName)
        {
            services.AddMvc()
                .AddRazorPagesOptions(options => options.Conventions.AddPageRoute("/" + pageName, "ui"));

            services.ConfigureOptions(typeof(CommonUIConfigureOptions));
        }
    }
}
