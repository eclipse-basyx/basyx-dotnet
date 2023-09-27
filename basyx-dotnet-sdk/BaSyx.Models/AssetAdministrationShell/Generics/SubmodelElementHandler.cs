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
using System.Reflection;

namespace BaSyx.Models.AdminShell
{
    public class SubmodelElementHandler
    {
        public GetValueHandler GetValueHandler { get; private set; }
        public SetValueHandler SetValueHandler { get; private set; }
        public SubmodelElementHandler(GetValueHandler getHandler, SetValueHandler setHandler)
        {
            GetValueHandler = getHandler;
            SetValueHandler = setHandler;
        }

        public SubmodelElementHandler(MethodInfo getMethodInfo, MethodInfo setMethodInfo, object target)
        {
            if(getMethodInfo != null)
                GetValueHandler = (GetValueHandler)getMethodInfo.CreateDelegate(typeof(GetValueHandler), target);
            if(setMethodInfo != null)
                SetValueHandler = (SetValueHandler)setMethodInfo.CreateDelegate(typeof(SetValueHandler), target);
        }
    }

}
