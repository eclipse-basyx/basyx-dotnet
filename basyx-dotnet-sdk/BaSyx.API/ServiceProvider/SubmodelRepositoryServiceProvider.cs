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
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace BaSyx.API.ServiceProvider
{
    public class SubmodelRepositoryServiceProvider : ISubmodelRepositoryServiceProvider, ISubmodelServiceProviderRegistry
    {
        public IEnumerable<ISubmodel> Submodels => GetBinding();

        public ISubmodelServiceProviderRegistry SubmodelProviderRegistry => this;

        private Dictionary<string, ISubmodelServiceProvider> SubmodelServiceProviders { get; }

        private ISubmodelRepositoryDescriptor _serviceDescriptor;
        public ISubmodelRepositoryDescriptor ServiceDescriptor
        {
            get
            {
                if (_serviceDescriptor == null)
                    _serviceDescriptor = new SubmodelRepositoryDescriptor(Submodels, null);

                return _serviceDescriptor;
            }
            private set
            {
                _serviceDescriptor = value;
            }
        }
        public SubmodelRepositoryServiceProvider(ISubmodelRepositoryDescriptor descriptor) : this()
        {
            ServiceDescriptor = descriptor;
        }

        public SubmodelRepositoryServiceProvider()
        {
            SubmodelServiceProviders = new Dictionary<string, ISubmodelServiceProvider>();
        }

        public void BindTo(IEnumerable<ISubmodel> submodels)
        {
            foreach (var submodel in submodels)
            {
                RegisterSubmodelServiceProvider(submodel.Id, submodel.CreateServiceProvider());
            }
            ServiceDescriptor = ServiceDescriptor ?? new SubmodelRepositoryDescriptor(submodels, null);
        }
        public IEnumerable<ISubmodel> GetBinding()
        {
            List<ISubmodel> submodels = new List<ISubmodel>();
            var retrievedSubmodelServiceProviders = GetSubmodelServiceProviders();
            if (retrievedSubmodelServiceProviders.TryGetEntity(out IEnumerable<ISubmodelServiceProvider> serviceProviders))
            {
                foreach (var serviceProvider in serviceProviders)
                {
                    ISubmodel binding = serviceProvider.GetBinding();
                    submodels.Add(binding);
                }
            }
            return submodels;
        }

        public IResult<ISubmodel> CreateSubmodel(ISubmodel submodel)
        {
            if (submodel == null)
                return new Result<ISubmodel>(new ArgumentNullException(nameof(submodel)));

            var registered = RegisterSubmodelServiceProvider(submodel.Id, submodel.CreateServiceProvider());
            if (!registered.Success)
                return new Result<ISubmodel>(registered);

            var retrievedSubmodelServiceProvider = GetSubmodelServiceProvider(submodel.Id);
            if (retrievedSubmodelServiceProvider.TryGetEntity(out ISubmodelServiceProvider serviceProvider))
                return new Result<ISubmodel>(true, serviceProvider.GetBinding());
            else
                return new Result<ISubmodel>(false, new Message(MessageType.Error, "Could not retrieve Submodel Service Provider"));
        }

        public IResult DeleteSubmodel(Identifier id)
        {
            if (string.IsNullOrEmpty(id))
                return new Result<ISubmodel>(new ArgumentNullException(nameof(id)));
            return UnregisterSubmodelServiceProvider(id);
        }

        public IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(Identifier id)
        {
            if (SubmodelServiceProviders.TryGetValue(id, out ISubmodelServiceProvider submodelServiceProvider))
                return new Result<ISubmodelServiceProvider>(true, submodelServiceProvider);
            else
                return new Result<ISubmodelServiceProvider>(false, new NotFoundMessage(id));
        }

        public IResult<IEnumerable<ISubmodelServiceProvider>> GetSubmodelServiceProviders()
        {
            if (SubmodelServiceProviders.Values == null)
                return new Result<IEnumerable<ISubmodelServiceProvider>>(false, new NotFoundMessage("Submodel Service Providers"));

            return new Result<IEnumerable<ISubmodelServiceProvider>>(true, SubmodelServiceProviders.Values?.ToList());
        }

        public IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(Identifier id, ISubmodelServiceProvider submodelServiceProvider)
        {
            if (SubmodelServiceProviders.ContainsKey(id))
                SubmodelServiceProviders[id] = submodelServiceProvider;
            else
                SubmodelServiceProviders.Add(id, submodelServiceProvider);

            return new Result<ISubmodelDescriptor>(true, submodelServiceProvider.ServiceDescriptor);
        }

        public IResult UnregisterSubmodelServiceProvider(Identifier id)
        {
            if (SubmodelServiceProviders.ContainsKey(id))
            {
                SubmodelServiceProviders.Remove(id);
                return new Result(true);
            }
            else
                return new Result(false, new NotFoundMessage(id));
        }

        public IResult<ISubmodel> RetrieveSubmodel(Identifier id)
        {
            var retrievedSubmodelServiceProvider = GetSubmodelServiceProvider(id);
            if(retrievedSubmodelServiceProvider.TryGetEntity(out ISubmodelServiceProvider serviceProvider))
            {
                ISubmodel binding = serviceProvider.GetBinding();
                return new Result<ISubmodel>(true, binding);
            }
            return new Result<ISubmodel>(false, new NotFoundMessage("Submodel Service Provider"));
        }

        public IResult<PagedResult<IElementContainer<ISubmodel>>> RetrieveSubmodels()
        {
            return new Result<PagedResult<IElementContainer<ISubmodel>>>(true, 
                new PagedResult<IElementContainer<ISubmodel>>(new ElementContainer<ISubmodel>(null, Submodels)));
        }

        public IResult UpdateSubmodel(Identifier id, ISubmodel submodel)
        {
            if (string.IsNullOrEmpty(id))
                return new Result<ISubmodel>(new ArgumentNullException(nameof(id)));
            if (submodel == null)
                return new Result<ISubmodel>(new ArgumentNullException(nameof(submodel)));

            var retrievedSubmodelServiceProvider = GetSubmodelServiceProvider(id);
            if (retrievedSubmodelServiceProvider.TryGetEntity(out ISubmodelServiceProvider serviceProvider))
                return serviceProvider.UpdateSubmodel(submodel);
            
            return new Result<ISubmodel>(false, new NotFoundMessage("Submodel Service Provider"));
        }
    }
}
