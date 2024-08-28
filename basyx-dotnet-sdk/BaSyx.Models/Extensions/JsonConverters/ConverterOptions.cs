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
using BaSyx.Models.AdminShell;

namespace BaSyx.Models.Extensions
{
    public class ConverterOptions
    {
        public bool ValueSerialization { get; set; } = true;
        public RequestLevel RequestLevel { get; set; } = RequestLevel.Deep;
        public RequestExtent RequestExtent { get; set; } = RequestExtent.WithoutBlobValue;
        public int Level { get; set; } = 0;
    }
}
