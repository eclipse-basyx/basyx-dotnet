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
namespace BaSyx.Utils.Settings
{
    public class ServerSettings : Settings<ServerSettings>
    {
        public ServerConfiguration ServerConfig { get; set; } = new ServerConfiguration();
        public ControllerConfiguration ControllerConfig { get; set; } = new ControllerConfiguration();
        public UserInterfaceConfiguration UserInterfaceConfig { get; set; } = new UserInterfaceConfiguration();
        public DiscoveryConfiguration DiscoveryConfig { get; set; } = new DiscoveryConfiguration();
    }   
}
