/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.API.Components;
using BaSyx.Utils.Settings.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaSyx.Common.UI.Pages
{
    public class AssetAdministrationShellModel : PageModel
    {
        public IAssetAdministrationShellServiceProvider ServiceProvider { get; }
        public ServerSettings Settings { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public AssetAdministrationShellModel(IAssetAdministrationShellServiceProvider provider, ServerSettings serverSettings, IHostingEnvironment hostingEnvironment)
        {
            ServiceProvider = provider;
            Settings = serverSettings;
            HostingEnvironment = hostingEnvironment;
        }
    }
}
