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
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;

namespace BaSyx.API.Components
{
    /// <summary>
    /// The SubmodelRegistry-Interface
    /// </summary>
    public interface ISubmodelRegistry
    {
        /// <summary>
        /// Creates a new or updates an existing Submodel registration at a specific Asset Administration Shell registered at the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="submodelDescriptor">The Submodel Descriptor</param>
        /// <returns>Result object with embedded Submodel Descriptor</returns>
        IResult<ISubmodelDescriptor> CreateOrUpdateSubmodelRegistration(string aasId, string submodelId, ISubmodelDescriptor submodelDescriptor);

        /// <summary>
        /// Retrieves all Submodel registrations from a specific Asset Administration Shell registered at the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="predicate">The predicate to explicitly look for specific Asset Administration Shell Descriptors</param>
        /// <returns>Result object with embedded list of Asset Administration Shell Descriptors</returns>
        IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId);

        /// <summary>
        /// Retrieves all Submodel registrations from a specific Asset Administration Shell registered at the Registry with a certain search predicate
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="predicate">The predicate to explicitly look for specific Asset Administration Shell Descriptors</param>
        /// <returns>Result object with embedded list of Asset Administration Shell Descriptors</returns>
        IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId, Predicate<ISubmodelDescriptor> predicate);

        /// <summary>
        /// Retrieves the Submodel registration from a specific Asset Administration Shell registered at the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <returns>Result object with embedded Submodel Descriptor</returns>
        IResult<ISubmodelDescriptor> RetrieveSubmodelRegistration(string aasId, string submodelId);

        /// <summary>
        /// Unregisters the Submodel from a specific Asset Administration Shell registered at the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <returns>Result object returning only the success of the operation</returns>
        IResult DeleteSubmodelRegistration(string aasId, string submodelId);
    }
}
