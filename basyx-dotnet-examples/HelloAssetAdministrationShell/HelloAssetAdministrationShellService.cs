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
using BaSyx.API.ServiceProvider;
using BaSyx.Models.AdminShell;
using BaSyx.Models.Export;
using BaSyx.Models.Extensions;
using BaSyx.Models.Semantics;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Range = BaSyx.Models.AdminShell.Range;

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
            //helloSubmodelServiceProvider.RegisterSubmodelElementHandler("HelloProperty",
            //    new SubmodelElementHandler(HelloSubmodelElementGetHandler, HelloSubmodelElementSetHandler));
            this.RegisterSubmodelServiceProvider(AssetAdministrationShell.Submodels["HelloSubmodel"].Id, helloSubmodelServiceProvider);

            assetIdentificationSubmodelProvider = new SubmodelServiceProvider();
            assetIdentificationSubmodelProvider.BindTo(AssetAdministrationShell.Submodels["AssetIdentification"]);
            assetIdentificationSubmodelProvider.UseInMemorySubmodelElementHandler();
            this.RegisterSubmodelServiceProvider(AssetAdministrationShell.Submodels["AssetIdentification"].Id, assetIdentificationSubmodelProvider);

            foreach(Submodel submodel in AssetAdministrationShell.Submodels)
            {
                var sp = submodel.CreateServiceProvider();
                this.RegisterSubmodelServiceProvider(submodel.Id, sp);
            }
        }

        private async Task HelloSubmodelElementSetHandler(ISubmodelElement submodelElement, ValueScope value)
        {
            await AssetAdministrationShell.Submodels["HelloSubmodel"].SubmodelElements["HelloProperty"].Cast<IProperty>().SetValueScope(value);
        }

        private async Task<ValueScope> HelloSubmodelElementGetHandler(ISubmodelElement submodelElement)
        {
            await Task.Delay(10000);            
            string svalue = "TestValue_" + DateTime.Now.ToString();
            return new PropertyValue(new ElementValue<string>(svalue));
        }

        private async Task<OperationResult> HelloOperationHandler(IOperation operation, IOperationVariableSet inputArguments, IOperationVariableSet inoutputArguments, IOperationVariableSet outputArguments, CancellationToken cancellationToken)
        {
            if (inputArguments?.Count > 0)
            {
                var inputValue = await inputArguments["Text"].GetValueAsync<string>();
                var propValue = new PropertyValue<string>("Hello '" + inputValue + "'");
                await outputArguments["ReturnValue"].SetValueScope(propValue);
                return new OperationResult(true);
            }
            return new OperationResult(false);
        }

        public override IAssetAdministrationShell BuildAssetAdministrationShell()
        {
            AssetAdministrationShell aas = new AssetAdministrationShell("HelloAAS", new BaSyxShellIdentifier("HelloAAS", "1.0.0"))
            {
                Description = new LangStringSet() { new LangString("en", "This is an exemplary Asset Administration Shell for starters") },

                AssetInformation = new AssetInformation()
                {
                    AssetKind = AssetKind.Instance,
                    GlobalAssetId = new BaSyxAssetIdentifier("HelloAsset", "1.0.0")
                }
            };

            Submodel helloSubmodel = new Submodel("HelloSubmodel", new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0"))
            {
                Description = new LangStringSet() { new LangString("enS", "This is an exemplary Submodel") },
                Kind = ModelingKind.Instance,
                SemanticId = new Reference(new Key(KeyType.GlobalReference, "urn:basys:org.eclipse.basyx:submodels:HelloSubmodel:1.0.0"))
            };
            string myTestProperty = "MyTestString";
            RangeValue myTestRangeValue = new RangeValue() { Min = new ElementValue<int>(3), Max = new ElementValue<int>(5) };

            helloSubmodel.SubmodelElements = new ElementContainer<ISubmodelElement>
            {
                //new Property<string>("HelloProperty")
                //{
                //    Description = new LangStringSet() { new LangString("en", "This is an exemplary property") },
                //    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn()))
                //},
                new Property<string>("HelloPropertyInternal", "TestValue")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary property with internal storage") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloPropertyInternal", "1.0.0").ToUrn()))
                },
                new Property<string>("HelloPropertyLocal", "TestValue")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary property with internal storage") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloPropertyInternal", "1.0.0").ToUrn())),
                    Get = (prop) => { return Task.FromResult(myTestProperty + ": " + DateTime.Now.ToString()); },
                    Set = (prop, val) => { myTestProperty = val; return Task.CompletedTask; }
                },
                new MultiLanguageProperty("HelloMultiLanguageProperty")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary MultiLanguageProperty") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloMultiLanguageProperty", "1.0.0").ToUrn())),
                    Value = new MultiLanguagePropertyValue(new LangStringSet()
                    {
                        new LangString("en", "This is a label in English"),
                        new LangString("de", "Das ist ein Bezeichner in deutsch")
                    })
                },
                new Range("HelloRange")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary Range") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloRange", "1.0.0").ToUrn())),
                    ValueType = new DataType(DataObjectType.Int32),
                    Get = (r) =>
                    {
                        return Task.FromResult(myTestRangeValue);
                    },
                    Set = (r, v) =>
                    {
                        myTestRangeValue = v; return Task.CompletedTask;
                    }
                },
                new RelationshipElement("HelloRelationshipElement")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary RelationshipElement") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloRelationshipElement", "1.0.0").ToUrn())),
                    Value = new RelationshipElementValue()
                    {
                           First = new Reference(new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn())) { Type = ReferenceType.ExternalReference },
                           Second = new Reference(new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloPropertyInternal", "1.0.0").ToUrn())) { Type = ReferenceType.ExternalReference },
                    }
                },
                new AnnotatedRelationshipElement("HelloAnnotatedRelationshipElement")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary RelationshipElement") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloRelationshipElement", "1.0.0").ToUrn())),
                    Value = new AnnotatedRelationshipElementValue()
                    {
                           First = new Reference(new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn())) { Type = ReferenceType.ExternalReference },
                           Second = new Reference(new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloPropertyInternal", "1.0.0").ToUrn())) { Type = ReferenceType.ExternalReference },
                           Annotations =
                           {
                               new Property<string>("MyConnectionValueString", "TestConnectionValue"),
                               new Property<int>("MyConnectionValueInt", 5)
                           }
                    }
                },
                new Entity("HelloEntity")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary Entity") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloEntity", "1.0.0").ToUrn())),
                    EntityType = EntityType.SelfManagedEntity,
                    Value =
                    {
                           GlobalAssetId = new BaSyxAssetIdentifier("MyEntityAsset", "1.0.0"),
                           SpecificAssetIds = new List<SpecificAssetId>()
                           {
                               new SpecificAssetId()
                               {
                                   Name = "MySpecificAssetId",
                                   Value = "MySpecificAssetIdValue"
                               }
                           },                           
                           Statements =
                           {
                               new Property<string>("MyConnectionValueString", "TestConnectionValue"),
                               new Property<int>("MyConnectionValueInt", 5)
                           }
                    }
                },
                new ReferenceElement("HelloReferenceElement")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary ReferenceElement") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloReferenceElement", "1.0.0").ToUrn())),
                    Value = new ReferenceElementValue(
                        new Reference(new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn())){ Type = ReferenceType.ExternalReference })
                },
                new Capability("HelloCapability")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary Capability") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloCapability", "1.0.0").ToUrn())),
                },
                new BasicEventElement("HelloBasicEventElement")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary BasicEventElement") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloBasicEventElement", "1.0.0").ToUrn())),                   
                    Direction = EventDirection.Output,
                    State = EventState.On,
                    MessageTopic = "boschrexroth/helloBasicEventElement",
                    LastUpdate = DateTime.UtcNow.ToString(),
                    MinInterval = "PT3S",
                    Value = new BasicEventElementValue()
                    {
                         Observed = new Reference(new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn())),
                    }
                },
                new Blob("HelloBlob")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary Blob") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloBlob", "1.0.0").ToUrn())),
                    Value = new BlobValue()
                    {
                        ContentType = "application/octet-stream"      ,
                        Value = "decaf"
                    }                    
                },
                new FileElement("HelloFile")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary file attached to the Asset Administration Shell") },
                    Value = new FileElementValue()
                    {
                        ContentType = "application/pdf",
                        Value = "/HelloAssetAdministrationShell.pdf"
                    }
                },
                new Operation("HelloOperation")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary operation returning the input argument with 'Hello' as prefix") },
                    InputVariables = new OperationVariableSet() { new Property<string>("Text") },
                    OutputVariables = new OperationVariableSet() { new Property<string>("ReturnValue") }
                },
                new SubmodelElementCollection("HelloSubmodelElementCollection")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary SubmodelElementCollection") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloSubmodelElementCollection", "1.0.0").ToUrn())),
                    Value =
                    {
                        Value =
                        {
                            new Property<int>("MySubIntValue", 4),
                            new Property<bool>("MySubBoolValue", true),
                            new Property<float>("MySubFloatValue", 3.4f)
                        }
                    }
                },
                new SubmodelElementList("HelloSubmodelElementList")
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary SubmodelElementList") },
                    SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloSubmodelElementList", "1.0.0").ToUrn())),
                    TypeValueListElement = ModelType.Property,
                    ValueTypeListElement = typeof(int),
                    Value =
                    {
                        Value =
                        {
                            new Property<int>(null)
                            {
                                Get = (prop) =>
                                {
                                    Random random = new Random();
                                    int val = random.Next(1, 500);
                                    return Task.FromResult(val);
                                },
                                Set = (prop, val) =>
                                {
                                    int myVal = val;
                                    return Task.CompletedTask;
                                }
                            },
                            new Property<int>(null, 42),
                            new Property<int>(null, 4711)
                        }                  
                    }
                },
                new Operation("SMCToObject")
                {
                    Description =
                    {
                        new LangString("en", "That's an operation that takes a SMC as input and converts it to a class")
                    },
                    InputVariables =
                    {
                         new SubmodelElementCollection<TestClass>("MyTestClassCollection")
                    },
                    OutputVariables =
                    {
                         new Property<string>("MyStringProperty"),
                    },
                    OnMethodCalled = (op, inArgs, inoutArgs, outArgs, ct) =>
                    {
                        TestClass myClass = inArgs.Get("MyTestClassCollection").Cast<ISubmodelElementCollection>().ToObject<TestClass>();

                        outArgs.Add(new Property<string>("MyStringProperty", myClass.MyStringProperty));

                        return new OperationResult(true);
                    }
                },
                 new Operation("GetSMC")
                {
                    Description =
                    {
                        new LangString("en", "That's an operation that takes a SMC as input and converts it to a class")
                    },
                    OutputVariables =
                    {
                         new SubmodelElementCollection<TestClass>("MyTestClassCollection")
                    },
                    OnMethodCalled = (op, inArgs, inoutArgs, outArgs, ct) =>
                    {
                        TestClass myClass = new TestClass()
                        {
                            MyBoolProperty = true,
                            MyIntProperty = 1,
                            MyStringProperty = "hello",
                        };

                        outArgs.Add(new SubmodelElementCollection<TestClass>("MyTestClassCollection", myClass));

                        return new OperationResult(true);
                    }
                },
                new Operation("ByteArrayToText")
                {
                    Description =
                    {
                        new LangString("en", "That's an operation that converts a byte array to UTF8 text")
                    },
                    InputVariables =
                    {
                         new SubmodelElementList<byte>("Bytes") { TypeValueListElement = ModelType.Property }
                    },
                    OutputVariables =
                    {
                         new Property<string>("Text"),
                    },
                    OnMethodCalled = (op, inArgs, inoutArgs, outArgs, ct) =>
                    {
                        byte[] bytes = inArgs.Get("Bytes").Cast<ISubmodelElementList>().ToEnumerable<byte>().ToArray();
                        string word = Encoding.UTF8.GetString(bytes);

                        outArgs.Add(new Property<string>("Text", word));

                        return new OperationResult(true);
                    }
                },
                new Operation("TextToByteArray")
                {
                    Description =
                    {
                        new LangString("en", "That's an operation that converts a an UTF8 text to a byte array")
                    },
                    InputVariables =
                    {
                         new Property<string>("Text"),
                    },
                    OutputVariables =
                    {
                          new SubmodelElementList<byte>("Bytes") { TypeValueListElement = ModelType.Property }
                    },
                    OnMethodCalled = (op, inArgs, inoutArgs, outArgs, ct) =>
                    {
                        string word = inArgs.Get("Text").GetValue<string>();
                        byte[] bytes = Encoding.UTF8.GetBytes(word);

                        SubmodelElementList<byte> collection = new SubmodelElementList<byte>("Bytes", bytes);

                        outArgs.Add(collection);

                        return new OperationResult(true);
                    }
                },
                new Operation("Calculate")
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
                        {
                            DisplayName =
                            {
                                new LangString("de", "Berechnetes Ergebnis"),
                                new LangString("en", "Calculated result")
                            },
                            SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("Result", "1.0.0").ToUrn()))
                        }
                    },
                    OnMethodCalled = async (op, inArgs, inOutArgs, outArgs, cancellationToken) =>
                    {
                        string expression = await inArgs["Expression"]?.GetValueAsync<string>();
                        int? computingTime = await inArgs["ComputingTime"]?.GetValueAsync<int>();                        

                        if (computingTime.HasValue)
                            await Task.Delay(computingTime.Value, cancellationToken);

                        if (cancellationToken.IsCancellationRequested)
                            return new OperationResult(false, new Message(MessageType.Information, "Cancellation was requested"));

                        double value = CalulcateExpression(expression);

                        outArgs["Result"].SetValue(value);
                        return new OperationResult(true);
                    }
                }
            };

            aas.Submodels.Add(helloSubmodel);

            var assetIdentificationSubmodel = new Submodel("AssetIdentification", new BaSyxSubmodelIdentifier("AssetIdentification", "1.0.0"))
            {
                Kind = ModelingKind.Instance
            };

            var productTypeProp = new Property<string>("ProductType", "HelloAsset_ProductType")
            {
                SemanticId = new Reference(
                  new Key(
                      KeyType.ConceptDescription,
                      "0173-1#02-AAO057#002"))
            };

            ConceptDescription orderNumberCD = new ConceptDescription()
            {
                Id = new Identifier("0173-1#02-AAO689#001"),
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

            var orderNumber = new Property<string>("OrderNumber", "HelloAsset_OrderNumber")
            {
                SemanticId = orderNumberCD.CreateReference(),
                ConceptDescription = orderNumberCD
            };

            var serialNumber = new Property<string>("SerialNumber", "HelloAsset_SerialNumber");

            assetIdentificationSubmodel.SubmodelElements.Add(productTypeProp);
            assetIdentificationSubmodel.SubmodelElements.Add(orderNumber);
            assetIdentificationSubmodel.SubmodelElements.Add(serialNumber);

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

    public class TestClass
    {
        public int MyIntProperty { get; set; }
        public bool MyBoolProperty { get; set; }
        public string MyStringProperty { get; set; }
    }
}
