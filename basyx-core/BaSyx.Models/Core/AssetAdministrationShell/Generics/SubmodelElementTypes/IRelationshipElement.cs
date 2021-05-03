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

namespace BaSyx.Models.Core.AssetAdministrationShell.Generics
{
    /// <summary>
    /// A relationship element is used to define a relationship between two referable elements.
    /// </summary>
    public interface IRelationshipElement : ISubmodelElement
    {
        /// <summary>
        /// First element in the relationship taking the role of the subject.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "first")]
        IReference First { get; }

        /// <summary>
        /// Second element in the relationship taking the role of the object. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "second")]
        IReference Second { get; }
    }
}
