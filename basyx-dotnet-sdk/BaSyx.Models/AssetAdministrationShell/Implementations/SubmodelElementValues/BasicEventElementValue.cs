/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
    public class BasicEventElementValue : ValueScope
    {
        public override ModelType ModelType => ModelType.BasicEventElement;

        /// <summary>
        /// Reference to the data or other elements that are being observed.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "observed")]
        public IReference Observed { get; set; }

        public BasicEventElementValue() { }
        public BasicEventElementValue(IReference observed)
        {
            Observed = observed;
        }      
    }
}
