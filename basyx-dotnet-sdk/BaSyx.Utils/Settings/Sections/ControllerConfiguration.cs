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

using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings
{
    public class ControllerConfiguration
    {
        [XmlArray("Controllers")]
        [XmlArrayItem("Controller")]
        public List<string> Controllers { get; set; }

        public ControllerConfiguration()
        {
            Controllers = new List<string>();
        }
    }
}
