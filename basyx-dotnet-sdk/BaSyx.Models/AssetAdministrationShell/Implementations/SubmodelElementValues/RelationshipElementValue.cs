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
	public class RelationshipElementValue : ValueScope
	{
		public override ModelType ModelType => ModelType.RelationshipElement;

		/// <summary>
		/// First element in the relationship taking the role of the subject.
		/// </summary>
		public IReference First { get; set; }

		/// <summary>
		/// Second element in the relationship taking the role of the object. 
		/// </summary>
		public IReference Second { get; set; }

		public RelationshipElementValue() { }
		public RelationshipElementValue(IReference first, IReference second)
		{
			First = first;
			Second = second;
		}
	}
}
