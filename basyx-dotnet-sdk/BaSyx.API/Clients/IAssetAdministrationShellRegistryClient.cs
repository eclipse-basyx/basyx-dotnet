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
using BaSyx.API.Interfaces;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaSyx.API.Clients
{
    public interface IAssetAdministrationShellRegistryClient : IAssetAdministrationShellRegistryInterface, IClient
    {
        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.CreateAssetAdministrationShellRegistration(IAssetAdministrationShellDescriptor)"/>
        Task<IResult<IAssetAdministrationShellDescriptor>> CreateAssetAdministrationShellRegistrationAsync(IAssetAdministrationShellDescriptor aasDescriptor);

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.UpdateAssetAdministrationShellRegistration(string, IAssetAdministrationShellDescriptor)"/>
        Task<IResult<IAssetAdministrationShellDescriptor>> UpdateAssetAdministrationShellRegistrationAsync(string aasIdentifier, IAssetAdministrationShellDescriptor aasDescriptor);

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.RetrieveAssetAdministrationShellRegistration(string)"/>
        Task<IResult<IAssetAdministrationShellDescriptor>> RetrieveAssetAdministrationShellRegistrationAsync(string aasIdentifier);

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.RetrieveAllAssetAdministrationShellRegistrations"/>
        Task<IResult<IEnumerable<IAssetAdministrationShellDescriptor>>> RetrieveAllAssetAdministrationShellRegistrationsAsync();

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.RetrieveAllAssetAdministrationShellRegistrations(Predicate{IAssetAdministrationShellDescriptor})"/>
        Task<IResult<IEnumerable<IAssetAdministrationShellDescriptor>>> RetrieveAllAssetAdministrationShellRegistrationsAsync(Predicate<IAssetAdministrationShellDescriptor> predicate);

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.DeleteAssetAdministrationShellRegistration(string)"/>
        Task<IResult> DeleteAssetAdministrationShellRegistrationAsync(string aasIdentifier);

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.CreateSubmodelRegistration(string, ISubmodelDescriptor)"/>
        Task<IResult<ISubmodelDescriptor>> CreateSubmodelRegistrationAsync(string aasIdentifier, ISubmodelDescriptor submodelDescriptor);

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.UpdateSubmodelRegistration(string, string, ISubmodelDescriptor)"/>
        Task<IResult<ISubmodelDescriptor>> UpdateSubmodelRegistrationAsync(string aasIdentifier, string submodelIdentifier, ISubmodelDescriptor submodelDescriptor);

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.RetrieveAllSubmodelRegistrations(string)"/>
        Task<IResult<IEnumerable<ISubmodelDescriptor>>> RetrieveAllSubmodelRegistrationsAsync(string aasIdentifier);

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.RetrieveAllSubmodelRegistrations(string, Predicate{ISubmodelDescriptor})"/>
        Task<IResult<IEnumerable<ISubmodelDescriptor>>> RetrieveAllSubmodelRegistrationsAsync(string aasIdentifier, Predicate<ISubmodelDescriptor> predicate);

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.RetrieveSubmodelRegistration(string, string)"/>
        Task<IResult<ISubmodelDescriptor>> RetrieveSubmodelRegistrationAsync(string aasIdentifier, string submodelIdentifier);

        ///<inheritdoc cref="IAssetAdministrationShellRegistryInterface.DeleteSubmodelRegistration(string, string)"/>
        Task<IResult> DeleteSubmodelRegistrationAsync(string aasIdentifier, string submodelIdentifier);
    }
}
