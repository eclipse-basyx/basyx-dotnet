using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaSyx.Core.Tests
{
    [TestClass]
    public class SerializationTest
    {

        [TestMethod]
        public void TestMethod1_SerializeDefaultValues()
        {
            Property<int> property = new Property<int>("TestIntegerProperty", 0);

            string jsonProperty = property.ToJson();

            property.SetValue(1);

            jsonProperty = property.ToJson();

        }
    }
}
