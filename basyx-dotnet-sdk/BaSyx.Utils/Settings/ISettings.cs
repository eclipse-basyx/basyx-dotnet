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

namespace BaSyx.Utils.Settings
{
    public interface ISettings
    {
        string Name { get; }
        string FilePath { get; set; }
        Dictionary<string, string> Miscellaneous { get; set; }
    }
}
