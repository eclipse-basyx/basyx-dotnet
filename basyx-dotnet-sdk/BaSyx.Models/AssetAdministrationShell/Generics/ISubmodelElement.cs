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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    public delegate IValue GetValueHandler(ISubmodelElement submodelElement);
    public delegate void SetValueHandler(ISubmodelElement submodelElement, IValue value);

    public delegate TValue GetValueHandler<TValue>(ISubmodelElement submodelElement);
    public delegate void SetValueHandler<TValue>(ISubmodelElement submodelElement, TValue value);

    [JsonConverter(typeof(SubmodelElementConverter))]
    public interface ISubmodelElement : IHasSemantics, IQualifiable, IReferable, IHasKind, IModelElement, IHasDataSpecification, IValueChanged
    {
        [IgnoreDataMember]
        GetValueHandler Get { get; }
        [IgnoreDataMember]
        SetValueHandler Set { get; }
    }
}
