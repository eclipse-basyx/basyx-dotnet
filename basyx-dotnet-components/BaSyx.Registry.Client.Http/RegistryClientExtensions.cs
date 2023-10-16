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
using BaSyx.API.ServiceProvider;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.ResultHandling;
using System;
using System.Threading;

namespace BaSyx.Registry.Client.Http
{
    public static class RegistryClientExtensions
    {
        public static IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShell(this IAssetAdministrationShellServiceProvider serviceProvider) => RegisterAssetAdministrationShell(serviceProvider, null);
        public static IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShell(this IAssetAdministrationShellServiceProvider serviceProvider, RegistryClientSettings settings)
        {
            RegistryClientSettings registryClientSettings = settings ?? RegistryClientSettings.LoadSettings();
            RegistryHttpClient registryHttpClient = new RegistryHttpClient(registryClientSettings);
            IResult<IAssetAdministrationShellDescriptor> result = registryHttpClient.UpdateAssetAdministrationShellRegistration(serviceProvider.ServiceDescriptor.Id.Id, serviceProvider.ServiceDescriptor);
            return result;
        }

        public static IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellWithRepeat(this IAssetAdministrationShellServiceProvider serviceProvider, RegistryClientSettings settings, TimeSpan interval, out CancellationTokenSource cancellationToken)
        {
            RegistryClientSettings registryClientSettings = settings ?? RegistryClientSettings.LoadSettings();
            RegistryHttpClient registryHttpClient = new RegistryHttpClient(registryClientSettings);

            cancellationToken = new CancellationTokenSource();
            registryHttpClient.RepeatRegistration(serviceProvider.ServiceDescriptor, interval, cancellationToken);

            IResult<IAssetAdministrationShellDescriptor> result = registryHttpClient.UpdateAssetAdministrationShellRegistration(serviceProvider.ServiceDescriptor.Id.Id, serviceProvider.ServiceDescriptor);
            return result;
        }

    }
}
