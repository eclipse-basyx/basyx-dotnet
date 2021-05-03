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
    /// The AssetAdministrationShellRegistry-Interface
    /// </summary>
    public interface IAssetAdministrationShellRegistry : ISubmodelRegistry
    {
        /// <summary>
        /// Creates a new or updates an existing Asset Administration Shell registration at the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="aasDescriptor">The Asset Administration Shell Descriptor</param>
        /// <returns>Result object with embedded Asset Administration Shell Descriptor</returns>
        IResult<IAssetAdministrationShellDescriptor> CreateOrUpdateAssetAdministrationShellRegistration(string aasId, IAssetAdministrationShellDescriptor aasDescriptor);
        
        /// <summary>
        /// Retrieves the Asset Administration Shell registration from the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <returns>Result object with embedded Asset Administration Shell Descriptor</returns>
        IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellRegistration(string aasId);

        /// <summary>
        /// Retrieves all Asset Administration Shell registrations from the Registry
        /// </summary>
        /// <param name="predicate">The predicate to explicitly look for specific Asset Administration Shell Descriptors</param>
        /// <returns>Result object with embedded list of Asset Administration Shell Descriptors</returns>
        IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations();
        /// <summary>
        /// Retrieves all Asset Administration Shell registrations from the Registry with a certain search predicate
        /// </summary>
        /// <param name="predicate">The predicate to explicitly look for specific Asset Administration Shell Descriptors</param>
        /// <returns>Result object with embedded list of Asset Administration Shell Descriptors</returns>
        IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations(Predicate<IAssetAdministrationShellDescriptor> predicate);

        /// <summary>
        /// Deletes the Asset Administration Shell registration from the Registry
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <returns>Result object returning only the success of the operation</returns>
        IResult DeleteAssetAdministrationShellRegistration(string aasId);
    }
}
