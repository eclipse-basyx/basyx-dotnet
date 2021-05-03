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
using System.Reflection;

namespace BaSyx.Models.Core.AssetAdministrationShell.Generics
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
