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

using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaSyx.Models.Semantics;

namespace DevelopmentSubmodel
{
    public static class DevelopmentSubmodel
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public static AssetAdministrationShell GetAssetAdministrationShell()
        {
            var domainId = "DemoAAS";

            AssetAdministrationShell aas = new AssetAdministrationShell(domainId, new BaSyxShellIdentifier("SimpleAAS", "1.0.0"))
            {
                Description = new LangStringSet()
                {
                   new LangString("de-DE", "Einfache VWS"),
                   new LangString("en-US", "Simple AAS")
                },
                Administration = new AdministrativeInformation()
                {
                    Version = "1.0",
                    Revision = "120"
                },
                AssetInformation = new AssetInformation()
                {
                    AssetKind = AssetKind.Instance,
                    GlobalAssetId = new BaSyxAssetIdentifier(domainId, "1.0.0")
                }
            };

            aas.Submodels.Add(GetDevSubmodel());
            aas.Submodels.Add(GetDevSubmodel2());

            return aas;
        }

        public static Submodel GetDevSubmodel2()
        {
            var domainId = "SimpleSM";

            Submodel testSubmodel = new Submodel(domainId, new BaSyxSubmodelIdentifier(domainId, "1.0.0"))
            {
                SubmodelElements = new ElementContainer<ISubmodelElement>
                {
                    new Property<string>("Property_String_1", "Level 1 String"),
                    new Property<int>("Property_Int_1", 1234),
                }
            };

            return testSubmodel;
        }

        public static Submodel GetDevSubmodel()
        {
            var domainId = "DemoSM";

            var blob = new Blob("Blob_L1")
            {
                Value = new BlobValue("application/octet-stream", "decaf")
            };

            var subBlob = new Blob("Blob_L2")
            {
                Value = new BlobValue("application/octet-stream", "decaf")
            };

            var strProperty = new Property<string>("Property_String_Full", "Level 1 String")
            {
                Category = "Test",
                Description =
                    new LangStringSet() { new LangString("en", "This is an exemplary string") },
                DisplayName =
                    new LangStringSet() { new LangString("en", "This is an exemplary string") },
                SemanticId = new Reference(new Key(KeyType.GlobalReference,
                    new BaSyxPropertyIdentifier("MultiLanguageProperty", "1.0.0").ToUrn())),
                SupplementalSemanticIds = new List<IReference>()
                {
                    new Reference(new Key(KeyType.GlobalReference,
                        new BaSyxPropertyIdentifier("MultiLanguageProperty", "1.0.0").ToUrn())),
                },
                Qualifiers = new List<IQualifier>()
                {
                    new Qualifier()
                    {
                        Type = "q.Type",
                        Value = "q.Value",
                        ValueId = new Reference(new Key(KeyType.GlobalReference,
                            new BaSyxPropertyIdentifier("MultiLanguageProperty", "1.0.0").ToUrn())),
                    }
                },

            };

            var intProperty = new Property<int>("Property_Int", 1010);

            var statements = new ElementContainer<ISubmodelElement>
            {
                strProperty,
                intProperty,
                blob
            };

            var entity = new Entity("Entity")
            {
                Value = new EntityValue()
                {
                    GlobalAssetId = new Identifier("entity_global_asset_id"),
                    SpecificAssetIds = new List<SpecificAssetId>()
                    {
                        new() { Name = "Spec_01", Value = "123" },
                        new() { Name = "Spec_02", Value = "456" }

                    },
                    Statements = statements
                }
            };

            var multiLang = new MultiLanguageProperty("MultiLanguageProperty")
            {
                Description =
                    new LangStringSet() { new LangString("en", "This is an exemplary MultiLanguageProperty") },
                SemanticId = new Reference(new Key(KeyType.GlobalReference,
                    new BaSyxPropertyIdentifier("MultiLanguageProperty", "1.0.0").ToUrn())),
                Value = new MultiLanguagePropertyValue()
                {
                    Value = new LangStringSet()
                    {
                        new LangString("en", "This is a label in English"),
                        new LangString("de", "Das ist ein Bezeichner in deutsch")
                    }
                }
            };

            var range = new BaSyx.Models.AdminShell.Range("Range")
            {
                Description =
                    new LangStringSet() { new LangString("en", "This is an exemplary MultiLanguageProperty") },
                DisplayName =
                    new LangStringSet() { new LangString("en", "This is an exemplary MultiLanguageProperty") },
                SemanticId = new Reference(new Key(KeyType.GlobalReference,
                    new BaSyxPropertyIdentifier("MultiLanguageProperty", "1.0.0").ToUrn())),
                SupplementalSemanticIds = new List<IReference>()
                {
                    new Reference(new Key(KeyType.GlobalReference,
                        new BaSyxPropertyIdentifier("MultiLanguageProperty", "1.0.0").ToUrn())),
                },
                Qualifiers = new List<IQualifier>()
                {
                    new Qualifier()
                    {
                        Type = "q.Type",
                        Value = "q.Value",
                        ValueId = new Reference(new Key(KeyType.GlobalReference,
                            new BaSyxPropertyIdentifier("MultiLanguageProperty", "1.0.0").ToUrn())),
                    }
                },
                ValueType = new DataType(DataObjectType.Int32),
                Value = new RangeValue()
                {
                    Min = new ElementValue<int>(3),
                    Max = new ElementValue<int>(5)
                }

            };

            var relationship = new RelationshipElement("RelationshipElement")
            {
                Description = new LangStringSet() { new LangString("en", "This is an exemplary RelationshipElement") },
                SemanticId = new Reference(new Key(KeyType.GlobalReference,
                    new BaSyxPropertyIdentifier("RelationshipElement", "1.0.0").ToUrn())),
                Value = new RelationshipElementValue()
                {
                    First = new Reference(
                        new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn())),
                    Second = new Reference(
                        new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloPropertyInternal", "1.0.0").ToUrn())),
                }
            };

            var annoRelationship = new AnnotatedRelationshipElement("AnnotatedRelationshipElement")
            {
                Description = new LangStringSet() { new LangString("en", "This is an exemplary RelationshipElement") },
                SemanticId = new Reference(new Key(KeyType.GlobalReference,
                    new BaSyxPropertyIdentifier("RelationshipElement", "1.0.0").ToUrn())),
                Value = new AnnotatedRelationshipElementValue()
                {
                    First = new Reference(
                        new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn())),
                    Second = new Reference(
                        new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloPropertyInternal", "1.0.0").ToUrn())),

                    Annotations = new ElementContainer<ISubmodelElement>
                    {
                        new Property<string>("Anno_Property_String", "String in AnnotatedRelationshipElement")
                    }
                }
            };

            var operation = new Operation("Operation")
            {
                Description = new LangStringSet()
                {
                    new LangString("en",
                        "This is an exemplary operation returning the input argument with 'Hello' as prefix")
                },
                InputVariables = new OperationVariableSet() { new Property<string>("Text") },
                OutputVariables = new OperationVariableSet() { new Property<string>("ReturnValue") },
                InOutputVariables = new OperationVariableSet() { new Property<string>("InOut") }
            };

            var file = new FileElement("File")
            {
                Value = new FileElementValue("application/pdf", "/AttachmentTestPDF.pdf"),
            };

            var refElement = new ReferenceElement("ReferenceElement")
            {
                Description = new LangStringSet() { new LangString("en", "This is an exemplary ReferenceElement") },
                SemanticId = new Reference(new Key(KeyType.GlobalReference,
                    new BaSyxPropertyIdentifier("HelloReferenceElement", "1.0.0").ToUrn())),
                Value = new ReferenceElementValue()
                {
                    Value = new Reference(
                        new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn()))
                }
            };

            var basicEvent = new BasicEventElement("BasicEventElement")
            {
                Description = new LangStringSet() { new LangString("en", "This is an exemplary BasicEventElement") },
                SemanticId = new Reference(new Key(KeyType.GlobalReference,
                    new BaSyxPropertyIdentifier("HelloBasicEventElement", "1.0.0").ToUrn())),

                Value = new BasicEventElementValue()
                {
                    Observed = new Reference(
                        new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
                        new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn())),
                },
                Direction = EventDirection.Output,
                State = EventState.On,
                MessageTopic = "boschrexroth/helloBasicEventElement",
                LastUpdate = DateTime.UtcNow.ToString(),
                MinInterval = "PT3S",
                ObservableReference = new Reference(new Key(KeyType.GlobalReference,
                    new BaSyxPropertyIdentifier("HelloBasicEventElement", "1.0.0").ToUrn())),
            };

            Submodel testSubmodel = new Submodel(domainId, new BaSyxSubmodelIdentifier(domainId, "1.0.0"))
            {
                //Description = new LangStringSet()
                //{
                //    new("de-DE", "Submodel für die Entwicklung"),
                //    new("en-US", "submodel for development")
                //},
                //Administration = new AdministrativeInformation()
                //{
                //    Version = "1.0",
                //    Revision = "1"
                //},
                //DisplayName = new LangStringSet()
                //{
                //    new("de-DE", "Submodel"),
                //    new("en-US", "submodel")
                //},
                //Category = "test_category",
                //SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloSubmodelElementList", "1.0.0").ToUrn())),
                //ConceptDescription = new ConceptDescription()
                //{
                //	Id = new Identifier("0173-1#02-AAO689#001"),
                //	EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>()
                //	{
                //		new DataSpecificationIEC61360(new DataSpecificationIEC61360Content()
                //		{
                //			PreferredName = new LangStringSet { new LangString("en", "identifying order number") },
                //			Definition =  new LangStringSet { new LangString("en", "unique classifying number that enables to name an object and to order it from a supplier or manufacturer") },
                //			DataType = DataTypeIEC61360.STRING
                //		})
                //	},
                //},
                //EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>()
                //{
                //	new DataSpecificationIEC61360(new DataSpecificationIEC61360Content()
                //	{
                //		PreferredName = new LangStringSet { new LangString("en", "identifying order number") },
                //		Definition =  new LangStringSet { new LangString("en", "unique classifying number that enables to name an object and to order it from a supplier or manufacturer") },
                //		DataType = DataTypeIEC61360.STRING
                //	})
                //},
                //SupplementalSemanticIds = new List<IReference>()
                //            {
                //                new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloSubmodelElementList", "1.0.0").ToUrn())),
                //            },

                SubmodelElements = new ElementContainer<ISubmodelElement>
                {
                    new Property<string>("Property_String_1", "Level 1 String"),
                    new Property<int>("Property_Int_1", 1234),
					//new Property<string>("Property_String_3", "Level 3 String"),
					//new Property<string>("Property_String_4", "Level 4 String"),
					//new Property<string>("Property_String_5", "Level 5 String"),
					//strProperty,
                    //intProperty,
                    //entity,
     //               strProperty,
     //               multiLang, 
					//range,
					//blob,
					//file,
					//relationship,
					//operation,
					//file,
					//refElement,
					//basicEvent,
					//annoRelationship,
                    new SubmodelElementCollection("TestCollection")
                    {
                        Value =
                        {
                            Value =
                            {
                                new Property<string>("OnlyProperty", "HasToBeChanged"),
                                //new Property<int>("ExcondProperty", 231)
                            }
                        }
                    },
                    new SubmodelElementList("ElementList_L1")
                    {
                        Value =
                        {
                            Value =
                            {
                                new Property<string>(null, "Level 2 String"),
                                new Property<int>(null, 123),
                                new SubmodelElementList(null)
                                {
                                    Value =
                                    {
                                        Value =
                                        {
                                            new Property<string>(null, "Level 3 String"),
                                            new Property<int>(null, 456),
                                //            new SubmodelElementList(null)
                                //            {
                                //                Value =
                                //                {
                                //                    Value =
                                //                    {
                                //                        new Property<string>(null, "Level 4 String"),
                                //                        new Property<int>(null, 789),
                                //                    }
                                //                }
                                //            }
                                        }
                                    }
                                }
                            }
                        }
                    },
        //            new SubmodelElementCollection("Collection_L1")
        //            {
        //                Value =
        //                {
        //                    Value =
        //                    {
        //                        new Property<string>("Property_String_Level_2", "Level 2 String"),
								////subBlob,
								//new SubmodelElementCollection("Collection_L2")
        //                        {
        //                            Value =
        //                            {
        //                                Value =
        //                                {
        //                                    new Property<string>("String_L3", "Level 3 String"),
        //                                    new SubmodelElementCollection("Collection_L3")
        //                                    {
        //                                        Value =
        //                                        {
        //                                            Value =
        //                                            {
        //                                                new Property<string>("String_L4", "Level 4 String"),
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
                }
            };

            return testSubmodel;
        }
    }
}