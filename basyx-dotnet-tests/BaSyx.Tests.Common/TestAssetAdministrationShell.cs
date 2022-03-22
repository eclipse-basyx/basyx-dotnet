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
                Asset = new Asset("SimpleAsset", new BaSyxAssetIdentifier("SimpleAsset", "1.0.0"))
                {
                    Kind = AssetKind.Instance,
                    Description = new LangStringSet()
                    {
                          new LangString("de-DE", "Einfaches Asset"),
                          new LangString("en-US", "Simple Asset")
                    }
                }
            };

            return aas;
        }
    }
}
