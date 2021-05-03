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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Constraints
{
    /// <summary>
    ///  A formula is used to describe constraints by a logical expression.
    /// </summary>
    public interface IFormula : IConstraint
    {
        /// <summary>
        /// A formula may depend on referable or even external global elements that are used in the logical expression. 
        ///The value of the referenced elements needs to be accessible so that it can be evaluated in the formula to true or false in the corresponding logical expression it is used in.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "dependsOn")]
        IEnumerable<IReference> DependsOn { get; }
    }
}
