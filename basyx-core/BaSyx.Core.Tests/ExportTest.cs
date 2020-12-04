using BaSyx.Models.Connectivity;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Export;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.JsonHandling;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BaSyx.Core.Tests
{
    [TestClass]
    public class ExportTest
    {
        [TestMethod]
        public void Test1_ImportAASX()
        {
            string aasxPath = @"C:\Development\AASX\Nexo-TypePlate_v6.aasx";
            IAssetAdministrationShell shell;
            using (AASX aasx = new AASX(aasxPath))
            {
                AssetAdministrationShellEnvironment_V2_0 environment = aasx.GetEnvironment_V2_0();
                shell = environment.AssetAdministrationShells.FirstOrDefault();
            }
            shell.Should().NotBeNull();
        }
    }
}
