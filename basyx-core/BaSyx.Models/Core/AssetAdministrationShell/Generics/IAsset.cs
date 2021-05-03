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
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using System.Runtime.Serialization;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;

using BaSyx.Models.Core.Common;

namespace BaSyx.Models.Core.AssetAdministrationShell.Generics
{
    /// <summary>
    ///An Asset describes meta data of an asset that is represented by an AAS. The asset may either represent an asset type or an asset instance. 
    ///The asset has a globally unique identifier plus – if needed – additional domain specific(proprietary) identifiers
    /// </summary>
    public interface IAsset : IIdentifiable, IModelElement, IHasDataSpecification
    {
        /// <summary>
        /// A reference to a Submodel that defines the handling of additional domain specific (proprietary) Identifiers for the asset like e.g. serial number etc. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "assetIdentificationModel")]
        IReference<ISubmodel> AssetIdentificationModel { get; }

        /// <summary>
        /// Bill of material of the asset represented by a submodel of the same AAS. This submodel contains a set of entities describing the material used to compose the composite I4.0 Component.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "billOfMaterial")]
        IReference<ISubmodel> BillOfMaterial { get; }

        /// <summary>
        /// Denotes whether the Asset of of kind “Type” or “Instance”. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "kind")]
        AssetKind Kind { get; }
    }
}
