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
using BaSyx.API.ServiceProvider;
using BaSyx.Models.AdminShell;
using BaSyx.Models.Extensions;
using BaSyx.Models.Semantics;
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
            this.RegisterSubmodelServiceProvider(AssetAdministrationShell.Submodels["HelloSubmodel"].Identification.Id, helloSubmodelServiceProvider);

            assetIdentificationSubmodelProvider = new SubmodelServiceProvider();
            assetIdentificationSubmodelProvider.BindTo(AssetAdministrationShell.Submodels["AssetIdentification"]);
            assetIdentificationSubmodelProvider.UseInMemorySubmodelElementHandler();
            this.RegisterSubmodelServiceProvider(AssetAdministrationShell.Submodels["AssetIdentification"].Identification.Id, assetIdentificationSubmodelProvider);
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
                outputArguments["ReturnValue"].SetValue<string>("Hello '" + inputArguments["Text"].Cast<IProperty>().ToObject<string>() + "'");
                return new OperationResult(true);
            }
            return new OperationResult(false);
        }

        public override IAssetAdministrationShell BuildAssetAdministrationShell()
        {
            AssetAdministrationShell aas = new AssetAdministrationShell("HelloAAS", new BaSyxShellIdentifier("HelloAAS", "1.0.0"))
            {
                Description = new LangStringSet() { new LangString("en", "This is an exemplary Asset Administration Shell for starters") },

                Asset = new Asset("HelloAsset", new BaSyxAssetIdentifier("HelloAsset", "1.0.0"))
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary Asset reference from the Asset Administration Shell") },
                    Kind = AssetKind.Instance
                }
            };

            Submodel helloSubmodel = new Submodel("HelloSubmodel", new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0"))
            {
                Description = new LangStringSet() { new LangString("enS", "This is an exemplary Submodel") },
                Kind = ModelingKind.Instance,
                SemanticId = new Reference(new GlobalKey(KeyElements.Submodel, KeyType.IRI, "urn:basys:org.eclipse.basyx:submodels:HelloSubmodel:1.0.0"))
            };

            helloSubmodel.SubmodelElements = new ElementContainer<ISubmodelElement>();
            helloSubmodel.SubmodelElements.Add(new Property<string>("HelloProperty", "TestValue")
            {
                Description = new LangStringSet() { new LangString("en", "This is an exemplary property") },
                SemanticId = new Reference(new GlobalKey(KeyElements.Property, KeyType.IRI, "urn:basys:org.eclipse.basyx:dataElements:HelloProperty:1.0.0"))
            });

            helloSubmodel.SubmodelElements.Add(new FileElement("HelloFile")
            {
                Description = new LangStringSet() { new LangString("en", "This is an exemplary file attached to the Asset Administration Shell")},
                ContentType = "application/pdf",
                Value = "/HelloAssetAdministrationShell.pdf"
            });

            helloSubmodel.SubmodelElements.Add(new Operation("HelloOperation")
            {
                Description = new LangStringSet() { new LangString("en", "This is an exemplary operation returning the input argument with 'Hello' as prefix") },
                InputVariables = new OperationVariableSet() { new Property<string>("Text") },
                OutputVariables = new OperationVariableSet() { new Property<string>("ReturnValue") }
            });

            helloSubmodel.SubmodelElements.Add(new Operation("HelloOperation2")
            {
                Description = new LangStringSet() { new LangString("en", "This operation does nothing") }
            });

            helloSubmodel.SubmodelElements.Add(new Operation("Calculate")
            {
                Description = new LangStringSet()
                {
                    new LangString("DE", "Taschenrechner mit simulierter langer Rechenzeit zum Testen von asynchronen Aufrufen"),
                    new LangString("EN", "Calculator with simulated long-running computing time for testing asynchronous calls")
                },
                InputVariables = new OperationVariableSet()
                {
                    new Property<string>("Expression")
                    {
                        Description = new LangStringSet()
                        {
                            new LangString("DE", "Ein mathematischer Ausdruck (z.B. 5*9)"),
                            new LangString("EN", "A mathematical expression (e.g. 5*9)")
                        }
                    },
                    new Property<int>("ComputingTime")
                    {
                        Description = new LangStringSet()
                        {
                            new LangString("DE", "Die Bearbeitungszeit in Millisekunden"),
                            new LangString("EN", "The computation time in milliseconds")
                        }
                    }
                },
                OutputVariables = new OperationVariableSet()
                {
                    new Property<double>("Result")
                },
                OnMethodCalled = async (op, inArgs, inOutArgs, outArgs, cancellationToken) =>
                {
                    string expression = inArgs["Expression"]?.GetValue<string>();
                    int? computingTime = inArgs["ComputingTime"]?.GetValue<int>();

                    inOutArgs["HierRein"]?.SetValue("DaWiederRaus");

                    if (computingTime.HasValue)
                        await Task.Delay(computingTime.Value, cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                        return new OperationResult(false, new Message(MessageType.Information, "Cancellation was requested"));

                    double value = CalulcateExpression(expression);

                    outArgs.Add(new Property<double>("Result", value));
                    return new OperationResult(true);
                }
            });

            aas.Submodels = new ElementContainer<ISubmodel>();
            aas.Submodels.Add(helloSubmodel);

            var assetIdentificationSubmodel = new Submodel("AssetIdentification", new BaSyxSubmodelIdentifier("AssetIdentification", "1.0.0"))
            {
                Kind = ModelingKind.Instance
            };

            var productTypeProp = new Property<string>("ProductType")
            {
                SemanticId = new Reference(
                  new GlobalKey(
                      KeyElements.ConceptDescription,
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
                        PreferredName = new LangStringSet { new LangString("en", "identifying order number") },
                        Definition =  new LangStringSet { new LangString("en", "unique classifying number that enables to name an object and to order it from a supplier or manufacturer") },
                        DataType = DataTypeIEC61360.STRING
                    })
                }
            };

            var orderNumber = new Property<string>("OrderNumber")
            {
                SemanticId = orderNumberCD.CreateReference(),
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

        public static double CalulcateExpression(string expression)
        {
            string columnName = "Evaluation";
            System.Data.DataTable dataTable = new System.Data.DataTable();
            System.Data.DataColumn dataColumn = new System.Data.DataColumn(columnName, typeof(double), expression);
            dataTable.Columns.Add(dataColumn);
            dataTable.Rows.Add(0);
            return (double)(dataTable.Rows[0][columnName]);
        }
    }
}
