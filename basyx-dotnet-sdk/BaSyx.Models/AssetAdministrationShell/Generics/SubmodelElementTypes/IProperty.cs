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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// A property is a data element that has a single value. 
    /// </summary>
    public interface IProperty : ISubmodelElement, IValueId
    {

    }

    ///<inheritdoc cref="IProperty"/>
    public interface IProperty<TValue> : IProperty, IValue<TValue>
    {
        [IgnoreDataMember]
        new GetValueHandler<TValue> Get { get; }
        [IgnoreDataMember]
        new SetValueHandler<TValue> Set { get; }
    }
}
