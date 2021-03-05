using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Linq;

namespace BaSyx.Core.Tests
{
    [TestClass]
    public class ModelUnitTest
    {

        private static IAssetAdministrationShell shell = new AssetAdministrationShell("MyTestAAS", new Identifier("https://www.basys40.de/shells/MyTestAAS", KeyType.IRI))
        {
            Asset = new Asset("MyTestAsset", new Identifier("https://www.basys40.de/assets/MyTestAsset", KeyType.IRI))
            {
                Kind = AssetKind.Instance
            },
            Administration = new AdministrativeInformation()
            {
                Version = "1.0.0",
                Revision = "0.0.1"
            },
            Description = new LangStringSet()
            {
                new LangString(new CultureInfo("de").TwoLetterISOLanguageName, "Meine Test-Verwaltungsschale"),
                new LangString(new CultureInfo("en").TwoLetterISOLanguageName, "My Test Asset Administration Shell"),
            },
            Submodels = new ElementContainer<ISubmodel>()
            {
                new Submodel("MyTestSubmodel", new Identifier("https://www.basys40.de/submodels/MyTestSubmodel", KeyType.IRI))
                {
                    SemanticId = new Reference(new Key(KeyElements.GlobalReference, KeyType.IRI, "urn:basys:org.eclipse.basyx:submodels:MyTestSubmodel:1.0.0", false)),
                    SubmodelElements =
                    {
                        new Property<string>("Prop1") { Value = "Prop1_TestValue" },
                        GetProperty<int>("Prop2", 2),
                        new SubmodelElementCollection("MySubmodelElementCollection")
                        {
                            Value =
                            {
                                new Property<string>("SubProp1") { Value = "SubProp1_TestValue" },
                                GetProperty<int>("SubProp2", 2),
                                new Property<int>("SubProp2") { Value = 2 },
                                new SubmodelElementCollection("MySubSubmodelElementCollection")
                                {
                                    Value =
                                    {
                                        new Property<string>("SubSubProp1") { Value = "SubSubProp1_TestValue" },
                                        GetProperty<int>("SubSubProp2", 2)                           
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        private static Property<T> GetProperty<T>(string idShort, T value)
        {
            return new Property<T>(idShort) { Value = value };
        }

        [TestMethod]
        public void TestMethod1_Flatten()
        {
            var flattened = shell.Submodels["MyTestSubmodel"].SubmodelElements.Flatten();
            flattened.Should().HaveCount(8);
        }
        [TestMethod]
        public void TestMethod2_CheckPath()
        {
            EquivalencyAssertionOptions<Property<int>> options = new EquivalencyAssertionOptions<Property<int>>();
            options.Including(o => o.IdShort);
            options.Including(o => o.Value);
            options.Including(o => o.ValueType);

            shell.Submodels["MyTestSubmodel"].SubmodelElements.Retrieve<Property<int>>("Prop2").Entity.Should().BeEquivalentTo(GetProperty<int>("Prop2", 2), opts => options);
            shell.Submodels["MyTestSubmodel"].SubmodelElements.Retrieve<Property<int>>("MySubmodelElementCollection/SubProp2").Entity.Should().BeEquivalentTo(GetProperty<int>("SubProp2", 2), opts => options);
            shell.Submodels["MyTestSubmodel"].SubmodelElements.Retrieve<Property<int>>("MySubmodelElementCollection/MySubSubmodelElementCollection/SubSubProp2").Entity.Should().BeEquivalentTo(GetProperty<int>("SubSubProp2", 2), opts => options);
        }

        [TestMethod]
        public void TestMethod3_SubmodelElementCollectionArray()
        {
            SubmodelElementCollection<int> smcInt = new SubmodelElementCollection<int>("MyIntCollection", new int[] { 1, 2, 3, 4, 5 });

            int[] myArray = smcInt;
            myArray.Sum().Should().Be(15);

            SubmodelElementCollection smcInt2 = smcInt;

            SubmodelElementCollection<int> smcInt3 = new SubmodelElementCollection<int>(smcInt2);
            int[] myArray2 = smcInt3;
            myArray2.Sum().Should().Be(15);
        }

        [TestMethod]
        public void TestMethod4_SubmodelElementCollectionEntity()
        {
            MyTestClass testClass = new MyTestClass()
            {
                TestPropInt = 5,
                TestPropBool = false,
                TestPropString = "StringyString"
            };

            SubmodelElementCollectionOfEntity<MyTestClass> smc = new SubmodelElementCollectionOfEntity<MyTestClass>("MyTestClassInstance", testClass);
            smc.Count.Should().Be(3);

            smc["TestPropString"].As<IProperty>().ToObject<string>().Should().Be(testClass.TestPropString);
            smc["TestPropInt"].As<IProperty>().ToObject<int>().Should().Be(testClass.TestPropInt);
            smc["TestPropBool"].As<IProperty>().ToObject<bool>().Should().Be(testClass.TestPropBool);

            smc.Value.TestPropString.Should().Be(testClass.TestPropString);
            smc.Value.TestPropInt.Should().Be(testClass.TestPropInt);
            smc.Value.TestPropBool.Should().Be(testClass.TestPropBool);

            SubmodelElementCollection smc2 = new SubmodelElementCollection("MyTestSMC")
            {
                Value =
                {
                    new Property<string>("PropString", "StringTheString"),
                    new Property<bool>("PropBool", true),
                    new Property<int>("PropInt", 8),
                }
            };


            SubmodelElementCollectionOfEntity<MyTestClass2> smcEntity2 = new SubmodelElementCollectionOfEntity<MyTestClass2>(smc2);
            smcEntity2.Value.PropString.Should().Be("StringTheString");
            smcEntity2.Value.PropInt.Should().Be(8);
            smcEntity2.Value.PropBool.Should().Be(true);
        }
    }

    public class MyTestClass2
    {
        public string PropString { get; set; }
        public int PropInt { get; set; }
        public bool PropBool { get; set; }
    }

    public class MyTestClass
    {
        public string TestPropString { get; set; }
        public int TestPropInt { get; set; }
        public bool TestPropBool { get; set; }
    }
}
