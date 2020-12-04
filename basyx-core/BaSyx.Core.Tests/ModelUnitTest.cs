using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

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
    }
}
