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
using Newtonsoft.Json;
using BaSyx.Models.Extensions;


namespace BaSyx.Models.AdminShell
{
    [JsonConverter(typeof(SubmodelElementConverter))]
    public interface ISubmodelElement : IHasSemantics, IQualifiable, IReferable, IHasKind, IModelElement, IHasDataSpecification, IValueChanged, IGetSet
    {
     
    }
}
