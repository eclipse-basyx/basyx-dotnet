/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/

using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Identification
{
    /// <summary>
    /// An element with a kind is an element that can either represent a template (type) or an instance. Default for an element is that it is representing an instance
    /// </summary>
    public interface IHasKind
    {
        /// <summary>
        ///Kind of the element: either type or instance (Default Value = Instance )
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "kind")]
        ModelingKind Kind { get; }
    }
}
