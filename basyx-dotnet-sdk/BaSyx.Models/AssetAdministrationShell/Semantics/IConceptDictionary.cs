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
using System.Collections.Generic;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// A dictionary contains elements that can be reused. The concept dictionary contains concept descriptions. 
    /// Typically a concept description dictionary of an AAS contains only concept descriptions of elements used within submodels of the AAS. 
    /// </summary>
    public interface IConceptDictionary : IReferable, IModelElement
    {
        /// <summary>
        /// Concept description defines a concept.
        /// </summary>
        List<IReference<IConceptDescription>> ConceptDescriptions { get; }       
    }
}
