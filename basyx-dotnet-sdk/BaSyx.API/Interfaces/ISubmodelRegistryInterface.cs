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
using BaSyx.Models.Connectivity;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;

namespace BaSyx.API.Interfaces
{
    /// <summary>
    /// Submodel Registry Interface
    /// </summary>
    public interface ISubmodelRegistryInterface
    {
        /// <summary>
        /// Creates a new Submodel registration
        /// </summary>
        /// <param name="submodelDescriptor">The Submodel Descriptor</param>
        /// <returns>Result object with embedded Submodel Descriptor</returns>
        IResult<ISubmodelDescriptor> CreateSubmodelRegistration(ISubmodelDescriptor submodelDescriptor);

        /// <summary>
        /// Updates an existing Submodel registration
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel's unique id</param>
        /// <param name="submodelDescriptor">The Submodel Descriptor</param>
        /// <returns>Result object with embedded Submodel Descriptor</returns>
        IResult<ISubmodelDescriptor> UpdateSubmodelRegistration(string submodelIdentifier, ISubmodelDescriptor submodelDescriptor);

        /// <summary>
        /// Retrieves all Submodel registrations
        /// </summary>
        /// <param name="predicate">The predicate to explicitly look for specific Asset Administration Shell Descriptors</param>
        /// <returns>Result object with embedded list of Asset Administration Shell Descriptors</returns>
        IResult<IEnumerable<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations();

        /// <summary>
        /// Retrieves all Submodel registrations with a certain search predicate
        /// </summary>
        /// <param name="predicate">The predicate to explicitly look for specific Asset Administration Shell Descriptors</param>
        /// <returns>Result object with embedded list of Asset Administration Shell Descriptors</returns>
        IResult<IEnumerable<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(Predicate<ISubmodelDescriptor> predicate);

        /// <summary>
        /// Retrieves the Submodel registration
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel's unique id</param>
        /// <returns>Result object with embedded Submodel Descriptor</returns>
        IResult<ISubmodelDescriptor> RetrieveSubmodelRegistration(string submodelIdentifier);

        /// <summary>
        /// De-registers the Submodel
        /// </summary>
        /// <param name="submodelIdentifier">The Submodel's unique id</param>
        /// <returns>Result object returning only the success of the operation</returns>
        IResult DeleteSubmodelRegistration(string submodelIdentifier);
    }
}
