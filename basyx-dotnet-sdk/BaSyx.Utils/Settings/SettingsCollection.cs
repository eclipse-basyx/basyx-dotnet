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
    public class SettingsCollection : List<Settings>
    {
        public Settings this[string name] => this.Find(e => e.Name == name.Replace(".xml", ""));

        public T GetSettings<T>(string name) where T : Settings
        {
            return (T)this[name];
        }
    }
}
