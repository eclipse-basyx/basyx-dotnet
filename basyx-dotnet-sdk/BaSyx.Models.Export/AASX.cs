/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/

using BaSyx.Models.AdminShell;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System;

namespace BaSyx.Models.Export
{
    public static class AASX
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger("AASX");

        public static IAssetAdministrationShell LoadAASX(string aasxFile)
        {
            IAssetAdministrationShell shell;
            try
            {
                try
                {
                    using (AASX_V3_0 aasx = new AASX_V3_0(aasxFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        AssetAdministrationShellEnvironment_V3_0 environment = aasx.GetEnvironment_V3_0();
                        shell = environment.AssetAdministrationShells.FirstOrDefault();
                        if (shell == null)
                            throw new Exception("Asset Administration Shell v3 cannot be obtained from AASX-Package");
                    }
                }
                catch (Exception v3)
                {
                    logger.LogError(v3, $"Unable to read AASX_V3 from {aasxFile}");

                    using (AASX_V2_0 aasx = new AASX_V2_0(aasxFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        AssetAdministrationShellEnvironment_V2_0 environment = aasx.GetEnvironment_V2_0();
                        shell = environment.AssetAdministrationShells.FirstOrDefault();
                        if (shell == null)
                            throw new Exception("Asset Administration Shell v2 cannot be obtained from AASX-Package");
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Unable to read AASX from {aasxFile}");
                throw;
            }            
            return shell;
        }
    }
}
