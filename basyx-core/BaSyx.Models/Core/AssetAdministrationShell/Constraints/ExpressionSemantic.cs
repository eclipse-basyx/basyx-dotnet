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
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Constraints
{
    [DataContract]
    public enum ExpressionSemantic : int
    {
        None = 0,
        Requirement = 1,
        Assurance = 2,
        Confirmation = 3,
        Measurement = 4,
        Setting = 5
    }
}
