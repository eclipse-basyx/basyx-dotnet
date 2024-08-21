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
using BaSyx.Models.Connectivity;
using BaSyx.Utils.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BaSyx.Utils.ResultHandling;
using BaSyx.Models.Extensions;
using BaSyx.API.ServiceProvider;
using BaSyx.Utils.Settings;
using BaSyx.Models.AdminShell;
using Endpoint = BaSyx.Models.Connectivity.Endpoint;
using Microsoft.Extensions.DependencyInjection;
using BaSyx.API.Http;

namespace BaSyx.Deployment.AppDataService
{
    public static class AppDataServiceExtensions
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger("AppDataServiceExtensions");

        public static IServiceCollection AddSettings<T>(this IServiceCollection services, AppDataService appDataService) where T : Settings
        {
            services.Configure<T>(appDataService.Configuration.GetSection(typeof(T).Name));
            return services;
        }

        public static void AddConfigurationSubmodel(this IAssetAdministrationShellServiceProvider serviceProvider, AppDataService appDataService, Identifier identifier)
        {
            Submodel configSubmodel = new Submodel("ShellConfiguration", identifier);

            foreach (var settingsEntry in appDataService.AppDataContext.Settings)
            {
                var settingsSmc = settingsEntry.Value.CreateSubmodelElementCollectionFromObject(settingsEntry.Value.Name);
                settingsSmc.Value.Value.Add(
                new Operation("Save")
                {
                    OnMethodCalled = (op, inArgs, inoutArgs, outArgs, ct) =>
                    {
                        settingsEntry.Value.SaveSettings(settingsEntry.Value.FilePath, settingsEntry.Value.GetType());
                        return new OperationResult(true);
                    }
                });
                configSubmodel.SubmodelElements.Add(settingsSmc);
            }  

            configSubmodel.SubmodelElements.Add(
                new Operation("Restart")
                {
                    OnMethodCalled = (op, inArgs, inoutArgs, outArgs, ct) =>
                    {
                        Task.Run(async () =>
                        {
                            await Task.Delay(3000);
                            Environment.ExitCode = 0;
                            Environment.Exit(Environment.ExitCode);
                        });
                        return new OperationResult(true);
                    }
                });

            ISubmodelServiceProvider submodelServiceProvider = configSubmodel.CreateServiceProvider();
            serviceProvider.SubmodelProviderRegistry.RegisterSubmodelServiceProvider(configSubmodel.Id, submodelServiceProvider);            
        }
    }
}
