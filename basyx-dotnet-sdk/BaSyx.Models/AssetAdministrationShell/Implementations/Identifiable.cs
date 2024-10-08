﻿/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public abstract class Identifiable : Referable, IIdentifiable
    {
        public Identifier Id { get; set; }
        public AdministrativeInformation Administration { get; set; }

        protected Identifiable(string idShort, Identifier id) : base(idShort)
        {
            Id = id;
        }
    }
}
