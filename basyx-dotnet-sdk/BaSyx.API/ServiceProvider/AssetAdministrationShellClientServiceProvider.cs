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
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;
using BaSyx.Models.Connectivity;
using System;
using BaSyx.API.Clients;
using System.Linq;
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace BaSyx.API.ServiceProvider
{
    public class AssetAdministrationShellClientServiceProvider : IAssetAdministrationShellServiceProvider, ISubmodelServiceProviderRegistry
    {
        private IAssetAdministrationShellDescriptor _serviceDescriptor;
        
        /// <summary>
        /// The Asset Administration Shell Descriptor containing the information to register the Service Provider at the next higher instance (e.g. AssetAdministrationShellRepository, AssetAdministrationShellRegistry)
        /// </summary>
        public IAssetAdministrationShellDescriptor ServiceDescriptor
        {
            get
            {
                foreach (var sp in SubmodelServiceProviders)
                {
                    if (sp.Value?.ServiceDescriptor != null)
                        _serviceDescriptor.AddSubmodelDescriptor(sp.Value.ServiceDescriptor);
                }

                return _serviceDescriptor;
            }
            private set
            {
                _serviceDescriptor = value;
            }
        }
        public ISubmodelServiceProviderRegistry SubmodelProviderRegistry => this;
        private Dictionary<string, ISubmodelServiceProvider> SubmodelServiceProviders { get; } = new Dictionary<string, ISubmodelServiceProvider>();

        private IAssetAdministrationShellClient _shellClient;

        /// <summary>
        /// Base implementation for IAssetAdministrationShellServiceProvider
        /// </summary>
        public AssetAdministrationShellClientServiceProvider(IAssetAdministrationShellClient shellClient)
        {
            _shellClient = shellClient;
            var shell_retrieved = _shellClient.RetrieveAssetAdministrationShell();
            if (!shell_retrieved.Success)
                throw new Exception("Could not retrieve shell to create service provider: " + shell_retrieved.Messages?.ToString());
            _serviceDescriptor = new AssetAdministrationShellDescriptor(shell_retrieved.Entity, new List<IEndpoint>() { _shellClient.Endpoint });
        }


        public virtual void BindTo(IAssetAdministrationShell element)
        { }

        public virtual IAssetAdministrationShell GetBinding()
        {
            IAssetAdministrationShell shell = RetrieveAssetAdministrationShell().Entity;

            foreach (var submodelServiceProvider in SubmodelServiceProviders)
            {
                ISubmodel submodel = submodelServiceProvider.Value.GetBinding();
                shell.Submodels.CreateOrUpdate(submodel.IdShort, submodel);
            }
            return shell;
        }

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell()
        {
            return _shellClient.RetrieveAssetAdministrationShell();
        }

        public IResult UpdateAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            return _shellClient.UpdateAssetAdministrationShell(aas);
        }

        public IResult<IAssetInformation> RetrieveAssetInformation()
        {
            return _shellClient.RetrieveAssetInformation();
        }

        public IResult UpdateAssetInformation(IAssetInformation assetInformation)
        {
            return _shellClient.UpdateAssetInformation(assetInformation);
        }

        public IResult<PagedResult<IEnumerable<IReference<ISubmodel>>>> RetrieveAllSubmodelReferences()
        {
            return _shellClient.RetrieveAllSubmodelReferences();
        }

        public IResult<IReference> CreateSubmodelReference(IReference submodelRef)
        {
            return _shellClient.CreateSubmodelReference(submodelRef);
        }

        public IResult DeleteSubmodelReference(Identifier id)
        {
            return _shellClient.DeleteSubmodelReference(id);
        }

        public virtual IResult<IEnumerable<ISubmodelServiceProvider>> GetSubmodelServiceProviders()
        {
            if (SubmodelServiceProviders.Values == null)
                return new Result<IEnumerable<ISubmodelServiceProvider>>(false, new NotFoundMessage("Submodel Service Providers"));

            return new Result<IEnumerable<ISubmodelServiceProvider>>(true, SubmodelServiceProviders.Values?.ToList());
        }

        public virtual IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(Identifier id, ISubmodelServiceProvider submodelServiceProvider)
        {
            if (SubmodelServiceProviders.ContainsKey(id))
                SubmodelServiceProviders[id] = submodelServiceProvider;
            else
                SubmodelServiceProviders.Add(id, submodelServiceProvider);

            return new Result<ISubmodelDescriptor>(true, submodelServiceProvider.ServiceDescriptor);
        }
        public virtual IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(Identifier id)
        {
            if (SubmodelServiceProviders.TryGetValue(id, out ISubmodelServiceProvider submodelServiceProvider))
                return new Result<ISubmodelServiceProvider>(true, submodelServiceProvider);
            else
                return new Result<ISubmodelServiceProvider>(false, new NotFoundMessage(id));
        }

        public virtual IResult UnregisterSubmodelServiceProvider(Identifier id)
        {
            if (SubmodelServiceProviders.ContainsKey(id))
            {
                SubmodelServiceProviders.Remove(id);
                return new Result(true);
            }
            else
                return new Result(false, new NotFoundMessage(id));
        }
    }
}
