/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/

using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings.Types
{
    public class ServerSettings : Settings<ServerSettings>
    {
        public ControllerConfiguration ControllerConfig { get; set; } = new ControllerConfiguration();

        public class ControllerConfiguration
        {
            [XmlArrayItem("Controller")]
            public List<string> Controllers { get; set; }

            public ControllerConfiguration()
            {
                Controllers = new List<string>();
            }
        }
    }
}
