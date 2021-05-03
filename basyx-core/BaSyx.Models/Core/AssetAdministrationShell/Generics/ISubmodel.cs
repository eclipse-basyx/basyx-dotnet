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
using BaSyx.Models.Core.AssetAdministrationShell.Constraints;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Generics
{
    /// <summary>
    /// A submodel defines a specific aspect of the asset represented by the AAS.
    /// A submodel is used to structure the digital representation and technical functionality of an Administration Shell into distinguishable parts. Each submodel refers to a well-defined domain or subject matter. 
    /// Submodels can become standardized and thus become submodels types. Submodels can have different life-cycles
    /// </summary>
    public interface ISubmodel : IIdentifiable, IHasKind, IHasSemantics, IModelElement, IHasDataSpecification, IQualifiable
    {
        /// <summary>
        /// A submodel consists of zero or more submodel elements. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "submodelElements")]
        IElementContainer<ISubmodelElement> SubmodelElements { get; }
    }
}
