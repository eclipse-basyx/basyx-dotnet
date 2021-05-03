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
using BaSyx.API.Components;
using BaSyx.Models.Connectivity.Descriptors;
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
            IResult<IAssetAdministrationShellDescriptor> result = registryHttpClient.CreateOrUpdateAssetAdministrationShellRegistration(serviceProvider.ServiceDescriptor.Identification.Id, serviceProvider.ServiceDescriptor);
            return result;
        }

        public static IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellWithRepeat(this IAssetAdministrationShellServiceProvider serviceProvider, RegistryClientSettings settings, TimeSpan interval, out CancellationTokenSource cancellationToken)
        {
            RegistryClientSettings registryClientSettings = settings ?? RegistryClientSettings.LoadSettings();
            RegistryHttpClient registryHttpClient = new RegistryHttpClient(registryClientSettings);

            cancellationToken = new CancellationTokenSource();
            registryHttpClient.RepeatRegistration(serviceProvider.ServiceDescriptor, interval, cancellationToken);

            IResult<IAssetAdministrationShellDescriptor> result = registryHttpClient.CreateOrUpdateAssetAdministrationShellRegistration(serviceProvider.ServiceDescriptor.Identification.Id, serviceProvider.ServiceDescriptor);
            return result;
        }

    }
}
