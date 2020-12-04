/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Distribution License 1.0 which is available at
* https://www.eclipse.org/org/documents/edl-v10.html
*
* 
*******************************************************************************/
using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using BaSyx.Models.Extensions.Semantics.DataSpecifications;
using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell
{
    public class HelloAssetAdministrationShellService : AssetAdministrationShellServiceProvider
    {
        private readonly SubmodelServiceProvider helloSubmodelServiceProvider;
        private readonly SubmodelServiceProvider assetIdentificationSubmodelProvider;

        public HelloAssetAdministrationShellService()
        {
            helloSubmodelServiceProvider = new SubmodelServiceProvider();
            helloSubmodelServiceProvider.BindTo(AssetAdministrationShell.Submodels["HelloSubmodel"]);
            helloSubmodelServiceProvider.RegisterMethodCalledHandler("HelloOperation", HelloOperationHandler);
            helloSubmodelServiceProvider.RegisterSubmodelElementHandler("HelloProperty",
                new SubmodelElementHandler(HelloSubmodelElementGetHandler, HelloSubmodelElementSetHandler));
            this.RegisterSubmodelServiceProvider("HelloSubmodel", helloSubmodelServiceProvider);

            assetIdentificationSubmodelProvider = new SubmodelServiceProvider();
            assetIdentificationSubmodelProvider.BindTo(AssetAdministrationShell.Submodels["AssetIdentification"]);
            assetIdentificationSubmodelProvider.UseInMemorySubmodelElementHandler();
            this.RegisterSubmodelServiceProvider("AssetIdentification", assetIdentificationSubmodelProvider);
        }

        private void HelloSubmodelElementSetHandler(ISubmodelElement submodelElement, IValue value)
        {
            AssetAdministrationShell.Submodels["HelloSubmodel"].SubmodelElements["HelloProperty"].Cast<IProperty>().Value = value.Value;
        }

        private IValue HelloSubmodelElementGetHandler(ISubmodelElement submodelElement)
        {
            var localProperty = AssetAdministrationShell.Submodels["HelloSubmodel"].SubmodelElements["HelloProperty"].Cast<IProperty>();
            return new ElementValue(localProperty.Value, localProperty.ValueType);
        }

        private Task<OperationResult> HelloOperationHandler(IOperation operation, IOperationVariableSet inputArguments, IOperationVariableSet inoutputArguments, IOperationVariableSet outputArguments, CancellationToken cancellationToken)
        {
            if (inputArguments?.Count > 0)
            {
                outputArguments.Add(
                    new Property<string>("ReturnValue", "Hello '" + inputArguments["Text"].Cast<IProperty>().ToObject<string>() + "'"));
                return new OperationResult(true);
            }
            return new OperationResult(false);
        }

        public override IAssetAdministrationShell BuildAssetAdministrationShell()
        {
            AssetAdministrationShell aas = new AssetAdministrationShell("HelloAAS", new BaSyxShellIdentifier("HelloAAS", "1.0.0"))
            {
                Description = new LangStringSet() { new LangString("en-US", "This is an exemplary Asset Administration Shell for starters") },

                Asset = new Asset("HelloAsset", new BaSyxAssetIdentifier("HelloAsset", "1.0.0"))
                {
                    Description = new LangStringSet() { new LangString("en-US", "This is an exemplary Asset reference from the Asset Administration Shell") },
                    Kind = AssetKind.Instance,
                    SemanticId = new Reference(new GlobalKey(KeyElements.Asset, KeyType.IRI, "urn:basys:org.eclipse.basyx:assets:HelloAsset:1.0.0"))
                }
            };

            Submodel helloSubmodel = new Submodel("HelloSubmodel", new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0"))
            {
                Description = new LangStringSet() { new LangString("en-US", "This is an exemplary Submodel") },
                Kind = ModelingKind.Instance,
                SemanticId = new Reference(new GlobalKey(KeyElements.Submodel, KeyType.IRI, "urn:basys:org.eclipse.basyx:submodels:HelloSubmodel:1.0.0"))
            };

            helloSubmodel.SubmodelElements = new ElementContainer<ISubmodelElement>();
            helloSubmodel.SubmodelElements.Add(new Property<string>("HelloProperty", "TestValue")
            {
                Description = new LangStringSet() { new LangString("en-US", "This is an exemplary property") },
                SemanticId = new Reference(new GlobalKey(KeyElements.Property, KeyType.IRI, "urn:basys:org.eclipse.basyx:dataElements:HelloProperty:1.0.0"))
            });

            helloSubmodel.SubmodelElements.Add(new File("HelloFile")
            {
                Description = new LangStringSet() { new LangString("en-US", "This is an exemplary file attached to the Asset Administration Shell")},
                MimeType = "application/pdf",
                Value = "/HelloAssetAdministrationShell.pdf"
            });


            helloSubmodel.SubmodelElements.Add(new Operation("HelloOperation")
            {
                Description = new LangStringSet() { new LangString("en-US", "This is an exemplary operation returning the input argument with 'Hello' as prefix") },
                InputVariables = new OperationVariableSet() { new Property<string>("Text") },
                OutputVariables = new OperationVariableSet() { new Property<string>("ReturnValue") }
            });

            aas.Submodels = new ElementContainer<ISubmodel>();
            aas.Submodels.Add(helloSubmodel);

            var assetIdentificationSubmodel = new Submodel("AssetIdentification", new BaSyxSubmodelIdentifier("AssetIdentification", "1.0.0"))
            {
                Kind = ModelingKind.Instance,
                Parent = aas
            };

            var productTypeProp = new Property<string>("ProductType")
            {
                SemanticId = new Reference(
                  new GlobalKey(
                      KeyElements.Property,
                      KeyType.IRDI,
                      "0173-1#02-AAO057#002")),
                Value = "HelloAsset_ProductType"
            };

            ConceptDescription orderNumberCD = new ConceptDescription()
            {
                Identification = new Identifier("0173-1#02-AAO689#001", KeyType.IRDI),
                EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>()
                {
                    new DataSpecificationIEC61360(new DataSpecificationIEC61360Content()
                    {
                        PreferredName = new LangStringSet { new LangString("EN", "identifying order number") },
                        Definition =  new LangStringSet { new LangString("EN", "unique classifying number that enables to name an object and to order it from a supplier or manufacturer") },
                        DataType = DataTypeIEC61360.STRING
                    })
                }
            };

            var orderNumber = new Property<string>("OrderNumber")
            {
                SemanticId = new Reference(
                    new GlobalKey(
                        KeyElements.Property,
                        KeyType.IRDI,
                        "0173-1#02-AAO689#001")),
                Value = "HelloAsset_OrderNumber",
                ConceptDescription = orderNumberCD
            };

            var serialNumber = new Property<string>("SerialNumber", "HelloAsset_SerialNumber");

            assetIdentificationSubmodel.SubmodelElements.Add(productTypeProp);
            assetIdentificationSubmodel.SubmodelElements.Add(orderNumber);
            assetIdentificationSubmodel.SubmodelElements.Add(serialNumber);

            (aas.Asset as Asset).AssetIdentificationModel = new Reference<ISubmodel>(assetIdentificationSubmodel);

            aas.Submodels.Add(assetIdentificationSubmodel);

            return aas;
        }
    }
}
