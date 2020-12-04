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
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Utils.ResultHandling;
using System.Threading;

namespace BaSyx.Registry.Client.Http
{
    public static class RegistryClientExtensions
    {
        public static IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShell(this IAssetAdministrationShellServiceProvider serviceProvider) => RegisterAssetAdministrationShell(serviceProvider, null);
        public static IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShell(this IAssetAdministrationShellServiceProvider serviceProvider, RegistryClientSettings settings)
        {
            RegistryHttpClient registryHttpClient = new RegistryHttpClient(settings);
            if (registryHttpClient.Settings?.RegistryConfig?.RepeatRegistration != null)
                registryHttpClient.RepeatRegistration(serviceProvider.ServiceDescriptor, new CancellationTokenSource());

            IResult<IAssetAdministrationShellDescriptor> result = registryHttpClient.CreateOrUpdateAssetAdministrationShellRegistration(serviceProvider.ServiceDescriptor.Identification.Id, serviceProvider.ServiceDescriptor);

            return result;
        }
    }
}
