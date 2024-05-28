/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
    public class SubmodelElementListValue : ValueScope
    {
        public override ModelType ModelType => ModelType.SubmodelElementList;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
		public IElementContainer<ISubmodelElement> Value { get => _value; set => _value.AddRange(value); }

        private readonly IElementContainer<ISubmodelElement> _value;

        public SubmodelElementListValue() 
        {
            _value = new ElementContainer<ISubmodelElement>();
        }
        public SubmodelElementListValue(IElementContainer<ISubmodelElement> valueElements)
        {
            _value = valueElements;
        }
    }
}
