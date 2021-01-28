using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BaSyx.Core.Tests
{
    [TestClass]
    public class SerializationTest
    {

        [TestMethod]
        public void TestMethod1_SerializeDefaultValues()
        {
            Property<int> property = new Property<int>("TestIntegerProperty", 0);
            property.SetValue(1);
            string jsonProperty = property.ToJson();

            Property deserializedProperty = JsonConvert.DeserializeObject<Property>(jsonProperty);
            deserializedProperty.Should().NotBeNull();
            deserializedProperty.Value.Should().Be(1);
        }

        [TestMethod]
        public void TestMethod2_DataTypeHandling()
        {
            DataType dataType = new DataType(typeof(int));

            string jsonDataType = JsonConvert.SerializeObject(dataType, Formatting.Indented);
            DataType deserializedDataType = JsonConvert.DeserializeObject<DataType>(jsonDataType);
            deserializedDataType.Should().NotBeNull();
            deserializedDataType.SystemType.Should().Be(typeof(int));
        }
    }
}
