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
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BaSyx.Models.Core.Common;

namespace BaSyx.Models.Core.AssetAdministrationShell.Views
{
    /// <summary>
    /// A view is a collection of referable elements w.r.t. to a specific viewpoint of one or more stakeholders.
    /// </summary>
    public interface IView : IHasSemantics, IReferable, IModelElement
    {
        /// <summary>
        /// Referable elements that are contained in the view.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "containedElements")]
        IEnumerable<IReference> ContainedElements { get; }
    }
}
