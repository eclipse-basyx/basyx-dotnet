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
    /// <summary>
    /// A File is a data element that represents an address to a file. The value is an URI that can represent an absolute or relative path. 
    /// </summary>
    public interface IFileElement : ISubmodelElement<FileElementValue>
    {

    }
}
