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
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaSyx.API.Components
{
    public class SubmodelRepositoryServiceProvider : ISubmodelRepositoryServiceProvider
    {
        public IEnumerable<ISubmodel> Submodels => GetBinding();

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
                RegisterSubmodelServiceProvider(submodel.Identification.Id, submodel.CreateServiceProvider());
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

            var registered = RegisterSubmodelServiceProvider(submodel.Identification.Id, submodel.CreateServiceProvider());
            if (!registered.Success)
                return new Result<ISubmodel>(registered);

            var retrievedSubmodelServiceProvider = GetSubmodelServiceProvider(submodel.Identification.Id);
            if (retrievedSubmodelServiceProvider.TryGetEntity(out ISubmodelServiceProvider serviceProvider))
                return new Result<ISubmodel>(true, serviceProvider.GetBinding());
            else
                return new Result<ISubmodel>(false, new Message(MessageType.Error, "Could not retrieve Submodel Service Provider"));
        }

        public IResult DeleteSubmodel(string submodelId)
        {
            if (string.IsNullOrEmpty(submodelId))
                return new Result<ISubmodel>(new ArgumentNullException(nameof(submodelId)));
            return UnregisterSubmodelServiceProvider(submodelId);
        }

        public IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(string id)
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

        public IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(string id, ISubmodelServiceProvider submodelServiceProvider)
        {
            if (SubmodelServiceProviders.ContainsKey(id))
                SubmodelServiceProviders[id] = submodelServiceProvider;
            else
                SubmodelServiceProviders.Add(id, submodelServiceProvider);

            return new Result<ISubmodelDescriptor>(true, submodelServiceProvider.ServiceDescriptor);
        }

        public IResult UnregisterSubmodelServiceProvider(string id)
        {
            if (SubmodelServiceProviders.ContainsKey(id))
            {
                SubmodelServiceProviders.Remove(id);
                return new Result(true);
            }
            else
                return new Result(false, new NotFoundMessage(id));
        }

        public IResult<ISubmodel> RetrieveSubmodel(string submodelId)
        {
            var retrievedSubmodelServiceProvider = GetSubmodelServiceProvider(submodelId);
            if(retrievedSubmodelServiceProvider.TryGetEntity(out ISubmodelServiceProvider serviceProvider))
            {
                ISubmodel binding = serviceProvider.GetBinding();
                return new Result<ISubmodel>(true, binding);
            }
            return new Result<ISubmodel>(false, new NotFoundMessage("Submodel Service Provider"));
        }

        public IResult<IElementContainer<ISubmodel>> RetrieveSubmodels()
        {
            return new Result<IElementContainer<ISubmodel>>(true, new ElementContainer<ISubmodel>(null, Submodels));
        }

        public IResult UpdateSubmodel(string submodelId, ISubmodel submodel)
        {
            if (string.IsNullOrEmpty(submodelId))
                return new Result<ISubmodel>(new ArgumentNullException(nameof(submodelId)));
            if (submodel == null)
                return new Result<ISubmodel>(new ArgumentNullException(nameof(submodel)));
            return CreateSubmodel(submodel);
        }
    }
}
