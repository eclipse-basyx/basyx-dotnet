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
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace BaSyx.API.ServiceProvider
{
    public class AssetAdministrationShellRepositoryServiceProvider : IAssetAdministrationShellRepositoryServiceProvider, IAssetAdministrationShellServiceProviderRegistry
    {
        public IEnumerable<IAssetAdministrationShell> AssetAdministrationShells => GetBinding();

        public IAssetAdministrationShellServiceProviderRegistry ShellProviderRegistry => this;

        private Dictionary<string, IAssetAdministrationShellServiceProvider> AssetAdministrationShellServiceProviders { get; }

        private IAssetAdministrationShellRepositoryDescriptor _serviceDescriptor;
        public IAssetAdministrationShellRepositoryDescriptor ServiceDescriptor
        {
            get
            {
                if (_serviceDescriptor == null)
                    _serviceDescriptor = new AssetAdministrationShellRepositoryDescriptor(AssetAdministrationShells, null);

                return _serviceDescriptor;
            }
            private set
            {
                _serviceDescriptor = value;
            }
        }
        public AssetAdministrationShellRepositoryServiceProvider(IAssetAdministrationShellRepositoryDescriptor descriptor) : this()
        {
            ServiceDescriptor = descriptor;
        }

        public AssetAdministrationShellRepositoryServiceProvider()
        {
            AssetAdministrationShellServiceProviders = new Dictionary<string, IAssetAdministrationShellServiceProvider>();
        }

        public void BindTo(IEnumerable<IAssetAdministrationShell> assetAdministrationShells)
        {
            foreach (var assetAdministrationShell in assetAdministrationShells)
            {
                RegisterAssetAdministrationShellServiceProvider(assetAdministrationShell.Id, assetAdministrationShell.CreateServiceProvider(true));
            }
            ServiceDescriptor = ServiceDescriptor ?? new AssetAdministrationShellRepositoryDescriptor(assetAdministrationShells, null);
        }
        public IEnumerable<IAssetAdministrationShell> GetBinding()
        {
            List<IAssetAdministrationShell> assetAdministrationShells = new List<IAssetAdministrationShell>();
            var retrievedShellServiceProviders = GetAssetAdministrationShellServiceProviders();
            if (retrievedShellServiceProviders.TryGetEntity(out IEnumerable<IAssetAdministrationShellServiceProvider> serviceProviders))
            {
                foreach (var serviceProvider in serviceProviders)
                {
                    IAssetAdministrationShell binding = serviceProvider.GetBinding();
                    assetAdministrationShells.Add(binding);
                }
            }
            return assetAdministrationShells;
        }

        public IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            if (aas == null)
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(aas)));
            
            var registered = RegisterAssetAdministrationShellServiceProvider(aas.Id, aas.CreateServiceProvider(true));
            if (!registered.Success)
                return new Result<IAssetAdministrationShell>(registered);

            var retrievedShellServiceProvider = GetAssetAdministrationShellServiceProvider(aas.Id);
            if (retrievedShellServiceProvider.TryGetEntity(out IAssetAdministrationShellServiceProvider serviceProvider))
                return new Result<IAssetAdministrationShell>(true, serviceProvider.GetBinding());
            else
                return new Result<IAssetAdministrationShell>(false, new Message(MessageType.Error, "Could not retrieve Asset Administration Shell Service Provider"));
        }

        public IResult DeleteAssetAdministrationShell(Identifier id)
        {
            if (string.IsNullOrEmpty(id))
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(id)));
            
            return UnregisterAssetAdministrationShellServiceProvider(id);
        }

        public IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(Identifier id)
        {
            if (AssetAdministrationShellServiceProviders.TryGetValue(id, out IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider))
                return new Result<IAssetAdministrationShellServiceProvider>(true, assetAdministrationShellServiceProvider);
            else
                return new Result<IAssetAdministrationShellServiceProvider>(false, new NotFoundMessage(id));
        }

        public IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders()
        {
            if (AssetAdministrationShellServiceProviders.Values == null)
                return new Result<IEnumerable<IAssetAdministrationShellServiceProvider>>(false, new NotFoundMessage("Asset AdministrationShell Service Providers"));

            return new Result<IEnumerable<IAssetAdministrationShellServiceProvider>>(true, AssetAdministrationShellServiceProviders.Values?.ToList());
        }

        public IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(Identifier id, IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider)
        {
            if (AssetAdministrationShellServiceProviders.ContainsKey(id))
                AssetAdministrationShellServiceProviders[id] = assetAdministrationShellServiceProvider;
            else
                AssetAdministrationShellServiceProviders.Add(id, assetAdministrationShellServiceProvider);

            return new Result<IAssetAdministrationShellDescriptor>(true, assetAdministrationShellServiceProvider.ServiceDescriptor);
        }

        public IResult UnregisterAssetAdministrationShellServiceProvider(Identifier id)
        {
            if (AssetAdministrationShellServiceProviders.ContainsKey(id))
            {
                AssetAdministrationShellServiceProviders.Remove(id);
                return new Result(true);
            }
            else
                return new Result(false, new NotFoundMessage(id));
        }

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(Identifier id)
        {
            var retrievedShellServiceProvider = GetAssetAdministrationShellServiceProvider(id);
            if(retrievedShellServiceProvider.TryGetEntity(out IAssetAdministrationShellServiceProvider serviceProvider))
            {
                IAssetAdministrationShell binding = serviceProvider.GetBinding();
                return new Result<IAssetAdministrationShell>(true, binding);
            }
            return new Result<IAssetAdministrationShell>(false, new NotFoundMessage("Asset Administration Shell Service Provider"));
        }

        public IResult<PagedResult<IElementContainer<IAssetAdministrationShell>>> RetrieveAssetAdministrationShells()
        {
            return new Result<PagedResult<IElementContainer<IAssetAdministrationShell>>>(true, 
                new PagedResult<IElementContainer<IAssetAdministrationShell>>(
                new ElementContainer<IAssetAdministrationShell>(null, AssetAdministrationShells)));
        }

        public IResult UpdateAssetAdministrationShell(Identifier id, IAssetAdministrationShell aas)
        {
            if (string.IsNullOrEmpty(id))
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(id)));
            if (aas == null)
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(aas)));

            var retrievedShellServiceProvider = GetAssetAdministrationShellServiceProvider(id);
            if (retrievedShellServiceProvider.TryGetEntity(out IAssetAdministrationShellServiceProvider serviceProvider))
            {
                return serviceProvider.UpdateAssetAdministrationShell(aas);
            }
            return new Result<IAssetAdministrationShell>(false, new NotFoundMessage("Asset Administration Shell Service Provider"));
        }
    }
}
