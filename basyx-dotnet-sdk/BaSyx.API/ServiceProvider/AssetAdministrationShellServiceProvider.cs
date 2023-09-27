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
using System.Linq;
using BaSyx.Models.Connectivity;
using System;
using BaSyx.Models.Extensions;

namespace BaSyx.API.ServiceProvider
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
                RegisterSubmodelServiceProvider(submodel.Identification.Id, submodelServiceProvider);
            }
        }

        public virtual IResult<IEnumerable<ISubmodelServiceProvider>> GetSubmodelServiceProviders()
        {
            if (SubmodelServiceProviders.Values == null)
                return new Result<IEnumerable<ISubmodelServiceProvider>>(false, new NotFoundMessage("Submodel Service Providers"));

            return new Result<IEnumerable<ISubmodelServiceProvider>>(true, SubmodelServiceProviders.Values?.ToList());
        }

        public virtual IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(string submodelIdentifier, ISubmodelServiceProvider submodelServiceProvider)
        {
            if (SubmodelServiceProviders.ContainsKey(submodelIdentifier))
                SubmodelServiceProviders[submodelIdentifier] = submodelServiceProvider;
            else
                SubmodelServiceProviders.Add(submodelIdentifier, submodelServiceProvider);

            return new Result<ISubmodelDescriptor>(true, submodelServiceProvider.ServiceDescriptor);
        }
        public virtual IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(string submodelId)
        {
            if (SubmodelServiceProviders.TryGetValue(submodelId, out ISubmodelServiceProvider submodelServiceProvider))
                return new Result<ISubmodelServiceProvider>(true, submodelServiceProvider);
            else
                return new Result<ISubmodelServiceProvider>(false, new NotFoundMessage(submodelId));
        }

        public virtual IResult UnregisterSubmodelServiceProvider(string submodelIdentifier)
        {
            if (SubmodelServiceProviders.ContainsKey(submodelIdentifier))
            {
                SubmodelServiceProviders.Remove(submodelIdentifier);
                return new Result(true);
            }
            else
                return new Result(false, new NotFoundMessage(submodelIdentifier));
        }

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(RequestContent content)
        {
            if (_assetAdministrationShell == null)
                return new Result<IAssetAdministrationShell>(false, new ErrorMessage("The service provider's inner Asset Administration Shell object is null"));

            return new Result<IAssetAdministrationShell>(true, _assetAdministrationShell);
        }

        public IResult UpdateAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            if (_assetAdministrationShell == null)
                return new Result(false, new ErrorMessage("The service provider's inner Asset Administration Shell object is null"));

            string idShort = aas.IdShort ?? _assetAdministrationShell.IdShort;
            Identifier identifier = aas.Identification ?? _assetAdministrationShell.Identification;

            AssetAdministrationShell tempShell = new AssetAdministrationShell(idShort, identifier)
            {
                Asset = aas.Asset ?? _assetAdministrationShell.Asset,
                Administration = aas.Administration ?? _assetAdministrationShell.Administration,
                DerivedFrom = aas.DerivedFrom ?? _assetAdministrationShell.DerivedFrom,
                Category = aas.Category ?? _assetAdministrationShell.Category,
                Description = aas.Description ?? _assetAdministrationShell.Description,
                DisplayName = aas.DisplayName ?? _assetAdministrationShell.DisplayName,
                Submodels = _assetAdministrationShell.Submodels,
                Views = _assetAdministrationShell.Views, 
            };

            _assetAdministrationShell = tempShell;
            return new Result(true);
        }

        public IResult<IAssetInformation> RetrieveAssetInformation()
        {
            if (_assetAdministrationShell == null)
                return new Result<IAssetInformation>(false, new ErrorMessage("The service provider's inner Asset Administration Shell object is null"));

            if (_assetAdministrationShell.AssetInformation == null)
                return new Result<IAssetInformation>(false, new ErrorMessage("The Asset Information object is null"));

            return new Result<IAssetInformation>(true, _assetAdministrationShell.AssetInformation);               
        }

        public IResult UpdateAssetInformation(IAssetInformation assetInformation)
        {
            if (_assetAdministrationShell == null)
                return new Result(false, new ErrorMessage("The service provider's inner Asset Administration Shell object is null"));

            AssetAdministrationShell tempShell = new AssetAdministrationShell(_assetAdministrationShell.IdShort, _assetAdministrationShell.Identification)
            {
                Administration = _assetAdministrationShell.Administration,
                Asset = _assetAdministrationShell.Asset,
                AssetInformation = assetInformation,
                DerivedFrom = _assetAdministrationShell.DerivedFrom,
                Category = _assetAdministrationShell.Category,
                Description = _assetAdministrationShell.Description,
                DisplayName = _assetAdministrationShell.DisplayName,
                Submodels = _assetAdministrationShell.Submodels,
                Views = _assetAdministrationShell.Views,
            };

            _assetAdministrationShell = tempShell;
            return new Result(true);
        }

        public IResult<IEnumerable<IReference<ISubmodel>>> RetrieveAllSubmodelReferences()
        {
            if (_assetAdministrationShell == null)
                return new Result<IEnumerable<IReference<ISubmodel>>>(false, new ErrorMessage("The service provider's inner Asset Administration Shell object is null"));

            return new Result<IEnumerable<IReference<ISubmodel>>>(true, _assetAdministrationShell.SubmodelReferences);
        }

        public IResult<IReference> CreateSubmodelReference(IReference submodelRef)
        {
            if (_assetAdministrationShell == null)
                return new Result<IReference>(false, new ErrorMessage("The service provider's inner Asset Administration Shell object is null"));

            var sp = GetSubmodelServiceProvider(submodelRef.First.Value);
            if (sp.Success)
                return new Result<IReference>(false, new ConflictMessage($"Submodel with id {submodelRef.First.Value}"));

            Submodel tempSubmodel = new Submodel(Guid.NewGuid().ToString(), new Identifier(submodelRef.First.Value, submodelRef.First.IdType));
            _assetAdministrationShell.Submodels.Add(tempSubmodel);
            var tempSubmodelSp = tempSubmodel.CreateServiceProvider();
            var result = RegisterSubmodelServiceProvider(tempSubmodel.Identification.Id, tempSubmodelSp);
            if (!result.Success)
                return new Result<IReference>(result);

            var checkRetrieve = GetSubmodelServiceProvider(tempSubmodel.Identification.Id).Entity.RetrieveSubmodel();
            if (!checkRetrieve.Success)
                return new Result<IReference>(checkRetrieve);

            var reference = checkRetrieve.Entity.CreateReference();
            return new Result<IReference>(true, reference);
        }

        public IResult DeleteSubmodelReference(string submodelIdentifier)
        {
            if (_assetAdministrationShell == null)
                return new Result(false, new ErrorMessage("The service provider's inner Asset Administration Shell object is null"));

            var sp = GetSubmodelServiceProvider(submodelIdentifier);
            if(!sp.Success)
                return new Result(false, new NotFoundMessage($"Submodel with id {submodelIdentifier}"));

            var result = UnregisterSubmodelServiceProvider(submodelIdentifier);
            if (!result.Success)
                return result;

            _assetAdministrationShell.Submodels.Remove(sp.Entity.ServiceDescriptor.IdShort);
            var checkRetrieve = _assetAdministrationShell.Submodels.Retrieve(sp.Entity.ServiceDescriptor.IdShort);

            if (!checkRetrieve.Success)
                return new Result(true);
            else
                return new Result(false, new ErrorMessage("Submodel reference could not be deleted"));
        }
    }
}
