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
namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// A reference element is a data element that defines a logical reference to another element within the same or another AAS or a reference to an external object or entity. 
    /// </summary>
    public interface IReferenceElement : ISubmodelElement<ReferenceElementValue>
	{

    }
}

