using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Attributes;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using BaSyx.Models.Extensions.Semantics.DataSpecifications;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSyx.Core.Tests
{
    [Submodel("TestSubmodel", "urn:identification:id:testSubmodel:1.0.0", KeyType.IRI)]
    public class TestClass
    {
        [Property("TestStringProperty", DataObjectTypes.String, "urn:semantic:id:testStringProperty:1.0.0", KeyElements.Property, KeyType.IRI, Category = "VARIABLE")]
        public string TestStringProperty { get; set; }

        [SubmodelElementCollection("TestCollection", "urn:semantic:id:testCollection:1.0.0", KeyElements.SubmodelElementCollection, KeyType.IRI, Category = "VARIABLE")]
        public TestSubClass TestCollection { get; set; } = new TestSubClass();

        [SubmodelElementCollection("TestList", "urn:semantic:id:testList:1.0.0", KeyElements.SubmodelElementCollection, KeyType.IRI, Category = "VARIABLE")]
        public List<TestSubClass> TestList { get; set; } = new List<TestSubClass>()
        {
            new TestSubClass() { TestSubIntProperty = 5 }
        };

        [Property("TestStaticBoolProperty", DataObjectTypes.Bool, "urn:semantic:id:testStaticBoolProperty:1.0.0", KeyElements.Property, KeyType.IRI, Category = "VARIABLE")]
        public bool TestStaticBoolProperty { get; set; } = true;

        public double TestDoubleProperty { get; set; } = 3.8;

        [IgnoreElement]
        public float TestIgnoreProperty { get; set; } = 1.7f;

    }

    [SubmodelElementCollection("TestSubClass", "urn:semantic:id:TestSubClass:1.0.0", KeyElements.SubmodelElementCollection, KeyType.IRI, Category = "VARIABLE")]
    public class TestSubClass
    {
        [Property("TestSubIntProperty", DataObjectTypes.Int32, "urn:semantic:id:testSubIntProperty:1.0.0", KeyElements.Property, KeyType.IRI, Category = "VARIABLE")]
        [DataSpecificationIEC61360("0173-ABC123-00#", KeyType.IRDI, 
            DataType = DataTypeIEC61360.INTEGER, 
            PreferredName_DE = "Test Integer Variable",
            PreferredName_EN = "Test Integer Variable",
            ShortName_DE = "testSubIntProperty",
            ShortName_EN = "testSubIntProperty",
            Definition_DE = "Eine Variable als Integer innerhalb einer SubmodelModelElementCollection",
            Definition_EN = "A variable as integer within a SubmodelElementCollection",
            Unit = "V",
            UnitId = "0173-CCDAB52-004",
            UnitIdKeyType = KeyType.IRDI)]
        public int TestSubIntProperty { get; set; }

        [Property("TestSubDataTimeProperty", DataObjectTypes.DateTime, "urn:semantic:id:testSubDataTimeProperty:1.0.0", KeyElements.Property, KeyType.IRI, Category = "VARIABLE")]       
        public DateTime TestSubDataTimeProperty { get; set; } = DateTime.Now;
    }

    [TestClass]
    public class AttributeTest
    {
        [TestMethod]
        public void Test11_CreateSubmodelElementCollectionFromObject()
        {
            TestClass testClass = new TestClass();
            testClass.TestStringProperty = "TestStringValue";
            testClass.TestCollection.TestSubIntProperty = 5;

            var seCollection = testClass.TestCollection.CreateSubmodelElementCollectionFromObject("TestCollection");
            seCollection.Should().NotBeNull();
            seCollection.Value["TestSubIntProperty"].GetValue<int>().Should().Be(5);
        }

        [TestMethod]
        public void Test12_CreateSubmodelFromObject()
        {
            TestClass testClass = new TestClass();
            testClass.TestStringProperty = "TestStringValue";
            testClass.TestCollection.TestSubIntProperty = 5;

            var submodel = typeof(TestClass).CreateSubmodelFromType(
                "TestClass", 
                new Identifier("TestClassId", KeyType.Custom), 
                SubmodelExtensions.DEFAULT_BINDING_FLAGS, 
                testClass);

            submodel.Should().NotBeNull();
            submodel.SubmodelElements["TestCollection"].Cast<SubmodelElementCollection>().Value["TestSubIntProperty"].GetValue<int>().Should().Be(5);
        }
    }
}
