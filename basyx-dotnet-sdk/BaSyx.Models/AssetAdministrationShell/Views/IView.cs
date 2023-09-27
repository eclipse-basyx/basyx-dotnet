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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
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
