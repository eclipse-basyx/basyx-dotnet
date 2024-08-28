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
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// An AssetAdministration Shell. 
    /// </summary>
    public interface IAssetAdministrationShell : IIdentifiable, IModelElement, IHasDataSpecification
    {
        /// <summary>
        /// The reference to the Asset Administration Shell, which the Asset Administration Shell was derived from.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "derivedFrom")]
        IReference<IAssetAdministrationShell> DerivedFrom { get; }

        /// <summary>
        /// Meta information about the asset the Asset Administration Shell is representing.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "assetInformation")]
        IAssetInformation AssetInformation { get; }

        /// <summary>
        /// The resolved Submodels
        /// </summary>
        [IgnoreDataMember]
        IElementContainer<ISubmodel> Submodels { get; set; }

        /// <summary>
        /// Reference to a submodel of the Asset Administration Shell.
        /// A submodel is a description of an aspect of the asset the Asset Administration Shell is representing.
        /// The asset of an Asset Administration Shell is typically described by one or more submodels.
        /// Temporarily, no submodel might be assigned to the Asset Administration Shell.
        /// </summary>
        [JsonPropertyName("submodels"), DataMember(EmitDefaultValue = false, IsRequired = false, Name = "submodels")]
        IEnumerable<IReference<ISubmodel>> SubmodelReferences { get; }
    }
}
