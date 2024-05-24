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
using BaSyx.API.ServiceProvider;
using BaSyx.Models.AdminShell;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.ResultHandling;
using System;
using System.Threading;

namespace BaSyx.Registry.Client.Http
{
    public static class RegistryClientExtensions
    {
        public static IResult RegisterAssetAdministrationShell(this IAssetAdministrationShellServiceProvider serviceProvider) => RegisterAssetAdministrationShell(serviceProvider, null);
        public static IResult RegisterAssetAdministrationShell(this IAssetAdministrationShellServiceProvider serviceProvider, RegistryClientSettings settings)
        {
            RegistryClientSettings registryClientSettings = settings ?? RegistryClientSettings.LoadSettings();
            RegistryHttpClient registryHttpClient = new RegistryHttpClient(registryClientSettings);
            IResult result = registryHttpClient.UpdateAssetAdministrationShellRegistration(serviceProvider.ServiceDescriptor.Id.Id, serviceProvider.ServiceDescriptor);
            return result;
        }

        public static IResult RegisterAssetAdministrationShellWithRepeat(this IAssetAdministrationShellServiceProvider serviceProvider, RegistryClientSettings settings, TimeSpan interval, out CancellationTokenSource cancellationToken)
        {
            RegistryClientSettings registryClientSettings = settings ?? RegistryClientSettings.LoadSettings();
            RegistryHttpClient registryHttpClient = new RegistryHttpClient(registryClientSettings);

            cancellationToken = new CancellationTokenSource();
            registryHttpClient.RepeatRegistration(serviceProvider.ServiceDescriptor, interval, cancellationToken);

            IResult result = registryHttpClient.UpdateAssetAdministrationShellRegistration(serviceProvider.ServiceDescriptor.Id.Id, serviceProvider.ServiceDescriptor);
            return result;
        }

        public static IResult CreateOrUpdate(this RegistryHttpClient client, IAssetAdministrationShellDescriptor descriptor)
        {
            var created = client.CreateAssetAdministrationShellRegistration(descriptor);
            if(!created.Success)
            {
                var index = created.Messages.FindIndex(c => c.Code == "409");
                if (index >= 0)
                    return client.UpdateAssetAdministrationShellRegistration(descriptor.Id, descriptor);
            }
            return created;
        }

        public static IResult CreateOrUpdate(this RegistryHttpClient client, Identifier aasId, ISubmodelDescriptor descriptor)
        {
            var created = client.CreateSubmodelRegistration(aasId, descriptor);
            if (!created.Success)
            {
                var index = created.Messages.FindIndex(c => c.Code == "409");
                if (index >= 0)
                    return client.UpdateSubmodelRegistration(aasId, descriptor.Id, descriptor);
            }
            return created;
        }
    }
}
