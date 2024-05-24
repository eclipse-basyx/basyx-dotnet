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
using BaSyx.API.ServiceProvider;
using BaSyx.Utils.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaSyx.Common.UI.Pages
{
    public class SubmodelRepositoryModel : PageModel
    {
        public ISubmodelRepositoryServiceProvider ServiceProvider { get; }
        public ServerSettings Settings { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        public SubmodelRepositoryModel(ISubmodelRepositoryServiceProvider provider, ServerSettings serverSettings, IWebHostEnvironment hostingEnvironment)
        {
            ServiceProvider = provider;
            Settings = serverSettings;
            HostingEnvironment = hostingEnvironment;
        }
    }
}
