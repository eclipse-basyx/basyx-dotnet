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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;
using System.Linq;
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.Models.Connectivity.Descriptors;

namespace BaSyx.API.Components
{
    public abstract class AssetAdministrationShellServiceProvider : IAssetAdministrationShellServiceProvider, ISubmodelServiceProviderRegistry
    {
        private IAssetAdministrationShell _assetAdministrationShell;

        /// <summary>
        /// Stores the Asset Administration Shell built by the BuildAssetAdministrationShell() function
        /// </summary>
        public virtual IAssetAdministrationShell AssetAdministrationShell 
        { 
            get
            {
                if (_assetAdministrationShell == null)
                {
                    IAssetAdministrationShell assetAdministrationShell = BuildAssetAdministrationShell();
                    BindTo(assetAdministrationShell);
                }
                return GetBinding();
            }
        }
        /// <summary>
        /// Custom function to build the Asset Administration Shell to be provided by the ServiceProvider. 
        /// Within this function you can import data (e.g. from AASX-packages, databases, etc.) to build your Asset Administration Shell.
        /// </summary>
        /// <returns>The built Asset Administration Shell</returns>
        public abstract IAssetAdministrationShell BuildAssetAdministrationShell();

        private IAssetAdministrationShellDescriptor _serviceDescriptor;
        
        /// <summary>
        /// The Asset Administration Shell Descriptor containing the information to register the Service Provider at the next higher instance (e.g. AssetAdministrationShellRepository, AssetAdministrationShellRegistry)
        /// </summary>
        public IAssetAdministrationShellDescriptor ServiceDescriptor
        {
            get
            {
                if (_serviceDescriptor == null)
                    _serviceDescriptor = new AssetAdministrationShellDescriptor(AssetAdministrationShell, null);

                if (_serviceDescriptor.SubmodelDescriptors?.Count() == 0 && SubmodelServiceProviders != null)
                    foreach (var submodelServiceProvider in SubmodelServiceProviders)
                    {
                        if (submodelServiceProvider.Value?.ServiceDescriptor != null)
                            _serviceDescriptor.SubmodelDescriptors.Create(submodelServiceProvider.Value.ServiceDescriptor);
                    }

                return _serviceDescriptor;
            }
            private set
            {
                _serviceDescriptor = value;
            }
        }
        public ISubmodelServiceProviderRegistry SubmodelRegistry => this;
        private Dictionary<string, ISubmodelServiceProvider> SubmodelServiceProviders { get; } = new Dictionary<string, ISubmodelServiceProvider>();

        /// <summary>
        /// Base implementation for IAssetAdministrationShellServiceProvider
        /// </summary>
        protected AssetAdministrationShellServiceProvider()
        { }

        protected AssetAdministrationShellServiceProvider(IAssetAdministrationShellDescriptor assetAdministrationShellDescriptor) : this()
        {
            ServiceDescriptor = assetAdministrationShellDescriptor;
        }

        protected AssetAdministrationShellServiceProvider(IAssetAdministrationShell assetAdministrationShell)
        {
            BindTo(assetAdministrationShell);
        }

        public virtual void BindTo(IAssetAdministrationShell element)
        {
            _assetAdministrationShell = element;
            ServiceDescriptor = ServiceDescriptor ?? new AssetAdministrationShellDescriptor(_assetAdministrationShell, null);
        }
        public virtual IAssetAdministrationShell GetBinding()
        {
            IAssetAdministrationShell shell = _assetAdministrationShell;

            foreach (var submodelServiceProvider in SubmodelServiceProviders)
            {
                ISubmodel submodel = submodelServiceProvider.Value.GetBinding();
                shell.Submodels.CreateOrUpdate(submodel.IdShort, submodel);
            }
            return shell;
        }

        public virtual void UseDefaultSubmodelServiceProvider()
        {
            foreach (var submodel in AssetAdministrationShell.Submodels.Values)
            {
                var submodelServiceProvider = submodel.CreateServiceProvider();
                RegisterSubmodelServiceProvider(submodel.IdShort, submodelServiceProvider);
            }
        }

        public virtual IResult<IEnumerable<ISubmodelServiceProvider>> GetSubmodelServiceProviders()
        {
            if (SubmodelServiceProviders.Values == null)
                return new Result<IEnumerable<ISubmodelServiceProvider>>(false, new NotFoundMessage("Submodel Service Providers"));

            return new Result<IEnumerable<ISubmodelServiceProvider>>(true, SubmodelServiceProviders.Values?.ToList());
        }

        public virtual IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(string submodelId, ISubmodelServiceProvider submodelServiceProvider)
        {
            if (SubmodelServiceProviders.ContainsKey(submodelId))
                SubmodelServiceProviders[submodelId] = submodelServiceProvider;
            else
                SubmodelServiceProviders.Add(submodelId, submodelServiceProvider);

            return new Result<ISubmodelDescriptor>(true, submodelServiceProvider.ServiceDescriptor);
        }
        public virtual IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(string submodelId)
        {
            if (SubmodelServiceProviders.TryGetValue(submodelId, out ISubmodelServiceProvider submodelServiceProvider))
                return new Result<ISubmodelServiceProvider>(true, submodelServiceProvider);
            else
                return new Result<ISubmodelServiceProvider>(false, new NotFoundMessage(submodelId));
        }

        public virtual IResult UnregisterSubmodelServiceProvider(string submodelId)
        {
            if (SubmodelServiceProviders.ContainsKey(submodelId))
            {
                SubmodelServiceProviders.Remove(submodelId);
                return new Result(true);
            }
            else
                return new Result(false, new NotFoundMessage(submodelId));
        }     
    }
}
