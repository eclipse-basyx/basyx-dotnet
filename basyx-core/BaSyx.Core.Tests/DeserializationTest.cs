using BaSyx.Models.Connectivity;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.JsonHandling;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;

namespace BaSyx.Core.Tests
{
    [TestClass]
    public class DeserializationTest
    {
        private static JsonSerializerSettings jsonSerializerSettings = new DependencyInjectionJsonSerializerSettings();

        private static IAssetAdministrationShell aas = new AssetAdministrationShell("MyTestAAS", new Identifier("https://www.basys40.de/shells/MyTestAAS", KeyType.IRI))
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
                    SemanticId = new Reference(new Key(KeyElements.GlobalReference, KeyType.IRI, "urn:basys:org.eclipse.basyx:submodels:MyTestSubmodel:1.0.0", false))
                }
            }
        };

        private static ISubmodel submodel = new Submodel("MyAdditionalTestSubmodel", new Identifier("https://www.basys40.de/submodels/MyAdditionalTestSubmodel", KeyType.IRI))
        {
            SemanticId = new Reference(new Key(KeyElements.GlobalReference, KeyType.IRI, "urn:basys:org.eclipse.basyx:submodels:MyAdditionalTestSubmodel:1.0.0", false))
        };

        private static IAssetAdministrationShellDescriptor aasDescriptor = new AssetAdministrationShellDescriptor(aas, new List<IEndpoint>()
        {
            new HttpEndpoint("http://localhost:5080/aas")
        });

        private static ISubmodelDescriptor submodelDescriptor = new SubmodelDescriptor(submodel, new List<IEndpoint>()
        {
            new HttpEndpoint("http://localhost:5111/submodel")
        });

        [TestMethod]
        public void TestMethod1_Deserialize()
        {
            aasDescriptor.SubmodelDescriptors.Add(new SubmodelDescriptor(aas.Submodels["MyTestSubmodel"], new List<IEndpoint>()
            {
                new HttpEndpoint("http://localhost:5080/aas/submodels/MyTestSubmodel/submodel")
            }));

            string serialized = JsonConvert.SerializeObject(aasDescriptor, jsonSerializerSettings);
            IAssetAdministrationShellDescriptor descriptor = JsonConvert.DeserializeObject<IAssetAdministrationShellDescriptor>(serialized, jsonSerializerSettings);
            descriptor.Should().BeEquivalentTo(aasDescriptor, opts => opts.IgnoringCyclicReferences());             
        }
    }
}
