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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
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
