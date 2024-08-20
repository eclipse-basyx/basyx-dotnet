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
using BaSyx.Utils.Settings;
using System.Xml.Serialization;

namespace BaSyx.Deployment.AppDataService
{
    public class AppDataServiceSettings : Settings<AppDataServiceSettings>
    {
        public ServiceConfiguration ServiceConfig { get; set; } = new ServiceConfiguration();

        public class ServiceConfiguration
        {
            [XmlElement]
            public string AppName { get; set; }

			[XmlElement]
			public bool IsVirtual { get; set; }

            [XmlElement]
            public string SystemInfoUrl { get; set; }

            [XmlElement]
            public bool UseUnixSocket { get; set; }
        }   
    }
}
