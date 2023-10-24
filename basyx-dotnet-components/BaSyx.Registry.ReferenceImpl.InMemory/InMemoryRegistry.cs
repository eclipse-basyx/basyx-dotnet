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
using BaSyx.API.Interfaces;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using BaSyx.Utils.ResultHandling.ResultTypes;
using System.Collections.Concurrent;

namespace BaSyx.Registry.ReferenceImpl.InMemory
{
    public class InMemoryRegistry : IAssetAdministrationShellRegistryInterface
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<InMemoryRegistry>();

        private readonly ConcurrentDictionary<string, IAssetAdministrationShellDescriptor> _descriptors;

        public InMemoryRegistry()
        {
            _descriptors = new ConcurrentDictionary<string, IAssetAdministrationShellDescriptor>();
        }

        public IResult<IAssetAdministrationShellDescriptor> CreateAssetAdministrationShellRegistration(IAssetAdministrationShellDescriptor aasDescriptor)
        {
            if (aasDescriptor == null)
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasDescriptor)));
            if (aasDescriptor.Id?.Id == null)
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasDescriptor.Id)));

            bool success = _descriptors.TryAdd(aasDescriptor.Id.Id, aasDescriptor);
            if (success)
                return new Result<IAssetAdministrationShellDescriptor>(true, aasDescriptor);
            else
                return new Result<IAssetAdministrationShellDescriptor>(false, new ConflictMessage($"Descriptor with {aasDescriptor.Id.Id}"));
        }

        public IResult UpdateAssetAdministrationShellRegistration(string aasId, IAssetAdministrationShellDescriptor aasDescriptor)
        {
            if(string.IsNullOrEmpty(aasId))
                return new Result(new ArgumentNullException(nameof(aasId)));
            if (aasDescriptor == null)
                return new Result(new ArgumentNullException(nameof(aasDescriptor)));
            if (aasDescriptor.Id?.Id == null)
                return new Result(new ArgumentNullException(nameof(aasDescriptor.Id)));

            if(_descriptors.TryGetValue(aasId, out var oldDescriptor))
            {
                bool success = _descriptors.TryUpdate(aasId, aasDescriptor, oldDescriptor);
                if (success)
                    return new Result(true);
                else
                    return new Result(false, new ErrorMessage($"Unable to update descriptor with {aasDescriptor.Id.Id}"));
            }
            else
                return new Result(false, new NotFoundMessage($"Descriptor with {aasDescriptor.Id.Id}"));            
        }

        public IResult<ISubmodelDescriptor> CreateSubmodelRegistration(string aasId, ISubmodelDescriptor submodelDescriptor)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(aasId)));
            if (submodelDescriptor == null)
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelDescriptor)));
            if (submodelDescriptor.Id?.Id == null)
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelDescriptor.Id)));

            if (_descriptors.TryGetValue(aasId, out var descriptor))
            {
                descriptor.AddSubmodelDescriptor(submodelDescriptor);
                return new Result<ISubmodelDescriptor>(true, submodelDescriptor);               
            }
            else
                return new Result<ISubmodelDescriptor>(false, new NotFoundMessage($"Descriptor with {aasId}"));
        }

        public IResult UpdateSubmodelRegistration(string aasId, string submodelId, ISubmodelDescriptor submodelDescriptor)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result(new ArgumentNullException(nameof(submodelId)));
            if (submodelDescriptor == null)
                return new Result(new ArgumentNullException(nameof(submodelDescriptor)));

            if (_descriptors.TryGetValue(aasId, out var descriptor))
            {
                if(descriptor.SubmodelDescriptors.Contains(submodelDescriptor, new DescriptorComparer()))
                {
                    descriptor.RemoveSubmodelDescriptor(submodelId);
                    descriptor.AddSubmodelDescriptor(submodelDescriptor);
                    return new Result(true);
                }
                else
                    return new Result(false, new NotFoundMessage($"Descriptor with {submodelId}"));
            }
            else
                return new Result(false, new NotFoundMessage($"Descriptor with {aasId}"));
        }

        public IResult DeleteAssetAdministrationShellRegistration(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result(new ArgumentNullException(nameof(aasId)));

            if (_descriptors.TryGetValue(aasId, out _))
            {
                bool success = _descriptors.TryRemove(aasId, out _);
                if (success)
                    return new Result(true);
                else
                    return new Result(false, new ErrorMessage($"Unable to delete descriptor with {aasId}"));
            }
            else
                return new Result(false, new NotFoundMessage($"Descriptor with {aasId}"));
        }

        public IResult DeleteSubmodelRegistration(string aasId, string submodelId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result(new ArgumentNullException(nameof(submodelId)));

            if (_descriptors.TryGetValue(aasId, out var descriptor))
            {
                var subDescriptor = descriptor.SubmodelDescriptors.FirstOrDefault(s => s.Id.Id == submodelId);
                if (subDescriptor != null)
                {
                    descriptor.RemoveSubmodelDescriptor(submodelId);
                    return new Result(true);
                }
                else
                    return new Result(false, new NotFoundMessage($"Descriptor with {submodelId}"));
            }
            else
                return new Result(false, new NotFoundMessage($"Descriptor with {aasId}"));
        }

        public IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellRegistration(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasId)));

            if (_descriptors.TryGetValue(aasId, out var descriptor))
                return new Result<IAssetAdministrationShellDescriptor>(true, descriptor);
            else
                return new Result<IAssetAdministrationShellDescriptor>(false, new NotFoundMessage($"Descriptor with {aasId}"));

        }
        public IResult<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>> RetrieveAllAssetAdministrationShellRegistrations(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            var allDescriptors = RetrieveAllAssetAdministrationShellRegistrations();
            return new Result<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>>(allDescriptors.Success, 
                new PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>(allDescriptors.Entity.Result.Where(ConvertToFunc(predicate))));
        }

        private Func<T, bool> ConvertToFunc<T>(Predicate<T> predicate)
        {
            return new Func<T, bool>(predicate);
        }

        public IResult<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>> RetrieveAllAssetAdministrationShellRegistrations()
        {
            var aasDescriptors = _descriptors.Values.ToList();
            return new Result<PagedResult<IEnumerable<IAssetAdministrationShellDescriptor>>>(true, aasDescriptors);
        }

         public IResult<ISubmodelDescriptor> RetrieveSubmodelRegistration(string aasId, string submodelId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelId)));

            if (_descriptors.TryGetValue(aasId, out var descriptor))
            {
                var subDescriptor = descriptor.SubmodelDescriptors.FirstOrDefault(s => s.Id.Id == submodelId);
                if (subDescriptor != null)
                    return new Result<ISubmodelDescriptor>(true, subDescriptor);
                else
                    return new Result<ISubmodelDescriptor>(false, new NotFoundMessage($"Descriptor with {submodelId}"));
            }
            else
                return new Result<ISubmodelDescriptor>(false, new NotFoundMessage($"Descriptor with {aasId}"));
        }

        public IResult<PagedResult<IEnumerable<ISubmodelDescriptor>>> RetrieveAllSubmodelRegistrations(string aasId, Predicate<ISubmodelDescriptor> predicate)
        {
            var allDescriptors = RetrieveAllSubmodelRegistrations(aasId);
            return new Result<PagedResult<IEnumerable<ISubmodelDescriptor>>>(allDescriptors.Success, 
                new PagedResult<IEnumerable<ISubmodelDescriptor>>(allDescriptors.Entity.Result.Where(ConvertToFunc(predicate))));
        }

        public IResult<PagedResult<IEnumerable<ISubmodelDescriptor>>> RetrieveAllSubmodelRegistrations(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<PagedResult<IEnumerable<ISubmodelDescriptor>>>(new ArgumentNullException(nameof(aasId)));

            if (_descriptors.TryGetValue(aasId, out var descriptor))
            {
                return new Result<PagedResult<IEnumerable<ISubmodelDescriptor>>>(true, descriptor.SubmodelDescriptors.ToList());
            }
            else
                return new Result<PagedResult<IEnumerable<ISubmodelDescriptor>>>(false, new NotFoundMessage($"Descriptor with {aasId}"));
        }
    }

    public class DescriptorComparer : IEqualityComparer<IServiceDescriptor>
    {
        public bool Equals(IServiceDescriptor x, IServiceDescriptor y)
        {
            if (x.Id.Id == y.Id.Id)
                return true;
            else
                return false;
        }

        public int GetHashCode(IServiceDescriptor obj)
        {
            return obj.GetHashCode();
        }
    }
}