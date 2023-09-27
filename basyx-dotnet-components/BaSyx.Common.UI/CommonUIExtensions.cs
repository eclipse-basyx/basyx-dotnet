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
using BaSyx.Components.Common.Abstractions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        private const string DEFAULT_UI_URL = "ui";

        public static void AddBaSyxUI(this IServerApplication serverApp, string pageName)
        {
            string uiUrl = serverApp.Settings?.UIConfig?.Url ?? DEFAULT_UI_URL;
            serverApp.ConfigureServices(services =>
            {                
                services.AddBaSyxUI(pageName, uiUrl);
            });            
        }

        public static void AddBaSyxUI(this IServerApplication serverApp, string pageName, string uiUrl)
        {
            serverApp.ConfigureServices(services =>
            {
                services.AddBaSyxUI(pageName, uiUrl);
            });
        }

        public static void AddBaSyxUI(this IServiceCollection services, string pageName, string uiUrl = DEFAULT_UI_URL)
        {            
            services.AddMvc().AddRazorPagesOptions(options => options.Conventions.AddPageRoute("/" + pageName, uiUrl));                

            services.ConfigureOptions(typeof(CommonUIConfigureOptions));
        }
        

        public static bool TryGetValue<T>(this ViewDataDictionary<dynamic> viewDataDictionary, string key, out T value)
        {
            if (viewDataDictionary.TryGetValue(key, out object oValue) && oValue is T tValue && tValue != null)
            {
                value = tValue;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
