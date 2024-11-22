/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling.ResultTypes;
using BaSyx.Utils.ResultHandling.http;
using System.Text.Json;

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

        public IResult<PagedResult<IElementContainer<IAssetAdministrationShell>>> RetrieveAssetAdministrationShells(int limit = 100, string cursor = "", string assetIds = "", string idShort = "")
        {
            var filteredAas = AssetAdministrationShells;

            // filter by asset IDs if set
            if (!string.IsNullOrEmpty(assetIds))
                filteredAas = FilterShellsByAssetIds(JsonDocument.Parse(assetIds));

            // else filter by ID short if set
            else if (!string.IsNullOrEmpty(idShort))
                filteredAas = AssetAdministrationShells.Where(e => idShort == e.IdShort);

            var aasDict = filteredAas.ToDictionary(aas => aas.Id.Id, aas => aas);

            // create the paged data
            var paginationHelper = new PaginationHelper<IAssetAdministrationShell>(aasDict, elem => elem.Id.Id);
            var pagingMetadata = new PagingMetadata(cursor);
            var pagedResult = paginationHelper.GetPaged(limit, pagingMetadata);

            var aasPaged = new ElementContainer<IAssetAdministrationShell>();
            aasPaged.AddRange(pagedResult.Result as IEnumerable<IAssetAdministrationShell>);
            var paginatedAAs = new PagedResult<IElementContainer<IAssetAdministrationShell>>(aasPaged, pagedResult.PagingMetadata);

            return new Result<PagedResult<IElementContainer<IAssetAdministrationShell>>>(true, paginatedAAs);
        }

        public IResult<PagedResult<IEnumerable<IReference<IAssetAdministrationShell>>>> RetrieveAssetAdministrationShellsReference(int limit = 100, string cursor = "")
        {
            var references = AssetAdministrationShells.Select(shell => shell.CreateReference());

            var refDict = references.ToDictionary(reference => reference.First.Value, reference => reference);

            // create the paged data
            var paginationHelper = new PaginationHelper<IReference<IAssetAdministrationShell>>(refDict, elem => elem.First.Value);
            var pagingMetadata = new PagingMetadata(cursor);
            var pagedResult = paginationHelper.GetPaged(limit, pagingMetadata);

            var refPaged = new List<IReference<IAssetAdministrationShell>>();
            refPaged.AddRange(pagedResult.Result as IEnumerable<IReference<IAssetAdministrationShell>>);
            var paginatedRef = new PagedResult<IEnumerable<IReference<IAssetAdministrationShell>>>(refPaged, pagedResult.PagingMetadata);

            return new Result<PagedResult<IEnumerable<IReference<IAssetAdministrationShell>>>>(true, paginatedRef);

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

        private IEnumerable<IAssetAdministrationShell> FilterShellsByAssetIds(JsonDocument assetIds)
        {
            var globalIds = assetIds.RootElement
                .EnumerateArray()
                .Where(e => e.GetProperty("key").GetString() == "globalAssetId")
                .Select(e => e.GetProperty("value").GetString())
                .ToList();

            var specificIds = assetIds.RootElement
                .EnumerateArray()
                .Where(e => e.GetProperty("key").GetString() != "globalAssetId"
                            && !string.IsNullOrEmpty(e.GetProperty("key").GetString()))
                .ToDictionary(
                    e => e.GetProperty("key").GetString(),
                    e => e.GetProperty("value").GetString()
                );

            var filteredAas = new List<IAssetAdministrationShell>();

            foreach (var aas in AssetAdministrationShells)
            {
                if (aas.AssetInformation == null)
                    continue;

                // check global IDs
                if (globalIds.Contains(aas.AssetInformation.GlobalAssetId?.Id) && !filteredAas.Contains(aas))
                {
                    filteredAas.Add(aas);
                    continue;
                }

                // check specific IDs
                foreach (var aasSpecificId in aas.AssetInformation.SpecificAssetIds)
                {
                    if (specificIds.TryGetValue(aasSpecificId.Name, out var specificIdValue) && specificIdValue == aasSpecificId.Value)
                    {
                        filteredAas.Add(aas);
                        break;
                    }
                }
            }

            return filteredAas;
        }
    }
}
