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
using BaSyx.Components.Common.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaSyx.Components.Common
{
    public class ControllerConvention : IApplicationModelConvention
    {
        private List<Type> controllersToInclude;
        private IServerApplication ServerApplication;

        public ControllerConvention(IServerApplication serverApplication)
        {
            controllersToInclude = new List<Type>();
            ServerApplication = serverApplication;
        }

        public ControllerConvention Include(Type controllerType)
        {
            controllersToInclude.Add(controllerType);
            return this;
        }

        public void Apply(ApplicationModel application)
        {
            if (application.Controllers?.Count() > 0)
            {
                List<ControllerModel> controllerToKeep = new List<ControllerModel>();
                foreach (var controller in controllersToInclude)
                {
                    var list = application.Controllers.Where(c => c.ControllerType.IsEquivalentTo(controller)).ToList();
                    if (list?.Count() > 0)
                        controllerToKeep.AddRange(list);
                }
                foreach (var controller in application.Controllers.ToList())
                {
                    if(!controllerToKeep.Contains(controller) && controller.ControllerType.Assembly == ServerApplication.ControllerAssembly)
                        application.Controllers.Remove(controller);
                }
            }
        }
    }
}
