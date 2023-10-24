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
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BaSyx.Models.AdminShell
{
    public delegate Task<ValueScope> GetValueScopeHandler(ISubmodelElement submodelElement);
    public delegate Task SetValueScopeHandler(ISubmodelElement submodelElement, ValueScope value);

    public delegate Task<TValueScope> GetValueScopeHandler<TValueScope>(ISubmodelElement submodelElement) where TValueScope : ValueScope;
    public delegate Task SetValueScopeHandler<TValueScope>(ISubmodelElement submodelElement, TValueScope valueScope) where TValueScope : ValueScope;

    public delegate Task<IValue> GetTypedValueHandler<IValue>(ISubmodelElement submodelElement);
    public delegate Task SetTypedValueHandler<IValue>(ISubmodelElement submodelElement, IValue value);

    public interface IGetSet
    {
        [IgnoreDataMember]
        GetValueScopeHandler Get { get; }
        [IgnoreDataMember]
        SetValueScopeHandler Set { get; }

        Task<ValueScope> GetValueScope();
        Task SetValueScope(ValueScope value);
    }

    public interface IGetSet<TValueScope> : IGetSet where TValueScope : ValueScope
    {
        [IgnoreDataMember]
        new GetValueScopeHandler<TValueScope> Get { get; }
        [IgnoreDataMember]
        new SetValueScopeHandler<TValueScope> Set { get; }

        new Task<TValueScope> GetValueScope();
        Task SetValueScope(TValueScope value);
    }

    public class SubmodelElementHandler
    {
        public GetValueScopeHandler GetValueHandler { get; private set; }
        public SetValueScopeHandler SetValueHandler { get; private set; }
        public SubmodelElementHandler(GetValueScopeHandler getHandler, SetValueScopeHandler setHandler)
        {
            GetValueHandler = getHandler;
            SetValueHandler = setHandler;
        }

        public SubmodelElementHandler(MethodInfo getMethodInfo, MethodInfo setMethodInfo, object target)
        {
            if(getMethodInfo != null)
                GetValueHandler = (GetValueScopeHandler)getMethodInfo.CreateDelegate(typeof(GetValueScopeHandler), target);
            if(setMethodInfo != null)
                SetValueHandler = (SetValueScopeHandler)setMethodInfo.CreateDelegate(typeof(SetValueScopeHandler), target);
        }
    }

}
