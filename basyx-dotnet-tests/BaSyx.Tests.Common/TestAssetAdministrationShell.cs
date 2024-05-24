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
using BaSyx.Models.AdminShell;

namespace SimpleAssetAdministrationShell
{
    public static class TestAssetAdministrationShell
    {
        public static AssetAdministrationShell GetAssetAdministrationShell(string id)
        {
            AssetAdministrationShell aas = new AssetAdministrationShell(id, new BaSyxShellIdentifier(id, "1.0.0"))
            {
                Description = new LangStringSet()
                {
                   new LangString("de-DE", "Einfache VWS"),
                   new LangString("en-US", "Simple AAS")
                },
                Administration = new AdministrativeInformation()
                {
                    Version = "1.0",
                    Revision = "120"
                },
                AssetInformation = new AssetInformation()
                {
                    AssetKind = AssetKind.Instance,
                    GlobalAssetId = new BaSyxAssetIdentifier("SimpleAsset", "1.0.0"),                  
                }
            };

            return aas;
        }
    }
}
