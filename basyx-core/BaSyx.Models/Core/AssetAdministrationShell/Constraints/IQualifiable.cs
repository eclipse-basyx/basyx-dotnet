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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Constraints
{
    /// <summary>
    /// The value of a qualifiable element may be further qualified by one or more qualifiers or complex formulas. 
    /// </summary>
    public interface IQualifiable
    {
        /// <summary>
        /// A constraint is used to further qualify an element. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "qualifiers")]
        IEnumerable<IConstraint> Constraints { get; }
    }
}
