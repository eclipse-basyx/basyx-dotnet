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
namespace BaSyx.Models.AdminShell
{
	public class ReferenceElementValue : ValueScope
	{
		public override ModelType ModelType => ModelType.ReferenceElement;

		public IReference Value { get; set; }

		public ReferenceElementValue() { }
		public ReferenceElementValue(IReference value)
		{
			Value = value;
		}
	}
}
