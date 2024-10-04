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
using BaSyx.API.Clients;
using BaSyx.Models.AdminShell;
using BaSyx.Models.Extensions;
using BaSyx.Clients.AdminShell.Http;
using BaSyx.Utils.ResultHandling;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssetAdministrationShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaSyx.Models.Connectivity;
using System.Text.Json;
using BaSyx.Utils.ResultHandling.ResultTypes;
using System.Security.Cryptography.Xml;

namespace SubmodelClientServerTests
{
    [TestClass]
    public class MainTest : ISubmodelClient
    {
        private static Submodel Submodel;

        private static SubmodelHttpClient Client;

        public IEndpoint Endpoint => ((IClient)Client).Endpoint;

        static MainTest()
        {
            Server.Run();
            Submodel = TestSubmodel.GetSubmodel("TestSubmodel");
            Client = new SubmodelHttpClient(new Uri(Server.ServerUrl));
        }

        #region test cases

        [TestMethod]
        public void Test100a_UpdateSubmodel()
        {
            LangStringSet newDescription = new LangStringSet()
            {
                new LangString("de", "Meine neue Beschreibung"),
                new LangString("en", "My new description")
            };
            Submodel.Description = newDescription;
            var updated = UpdateSubmodel(Submodel);
            updated.Success.Should().BeTrue();
        }

        [TestMethod]
        public void Test100b_UpdateSubmodelChildren()
        {
            var idShort = "UpdatedSubmodel";
            var updatedSubmodel = new Submodel(idShort, new BaSyxSubmodelIdentifier("ReplacedSubmodel", "1.0.0"))
            {
                Description = new LangStringSet()
                {
                    new("de-DE", "Submodel wurde aktualsiert"),
                    new("en-US", "submodel was updated")
                },
                Administration = new AdministrativeInformation()
                {
                    Version = "2.0",
                    Revision = "2"
                },
                DisplayName = new LangStringSet()
                {
                    new("de-DE", "Submodel aktualisiert"),
                    new("en-US", "submodel updated")
                },
                Category = "updated_category",
                SubmodelElements = new ElementContainer<ISubmodelElement>
                {
                    new Property<string>("New_String_Property", "updated"),
                    new Property<int>("New_Int_Property", 262)
                }
            };
            var updated = UpdateSubmodel(updatedSubmodel);
            updated.Success.Should().BeTrue();

            var submodel = RetrieveSubmodel().Entity;
            submodel.SubmodelElements.Count.Should().NotBe(Submodel.SubmodelElements.Count);
            submodel.Category.Should().Be(updatedSubmodel.Category);
            submodel.IdShort.Should().NotBe(updatedSubmodel.IdShort);
            submodel.IdShort.Should().Be(Submodel.IdShort);
        }

        [TestMethod]
        public void Test100c_UpdateSubmodelNoChildren()
        {
            var idShort = "UpdatedSubmodel";
            var updatedSubmodel = new Submodel(idShort, new BaSyxSubmodelIdentifier("ReplacedSubmodel", "1.0.0"))
            {
                Description = new LangStringSet()
                {
                    new("de-DE", "Submodel wurde aktualsiert"),
                    new("en-US", "submodel was updated")
                },
                Administration = new AdministrativeInformation()
                {
                    Version = "2.0",
                    Revision = "2"
                },
                DisplayName = new LangStringSet()
                {
                    new("de-DE", "Submodel aktualisiert"),
                    new("en-US", "submodel updated")
                },
                Category = "updated_category",
                SubmodelElements = null
            };

            var updated = UpdateSubmodel(updatedSubmodel);
            updated.Success.Should().BeTrue();

            var submodel = RetrieveSubmodel().Entity;
            submodel.SubmodelElements.Count.Should().Be(Submodel.SubmodelElements.Count);
            submodel.Category.Should().Be(updatedSubmodel.Category);
            submodel.IdShort.Should().NotBe(updatedSubmodel.IdShort);
            submodel.IdShort.Should().Be(Submodel.IdShort);
        }

        [TestMethod]
        public void Test101_RetrieveSubmodel()
        {
            ReplaceSubmodel(Submodel);
            var result = RetrieveSubmodel();

            result.Success.Should().BeTrue();
            result.Entity.IdShort.Should().BeEquivalentTo(Submodel.IdShort);
            result.Entity.Id.Should().BeEquivalentTo(Submodel.Id);
            result.Entity.Description.Should().BeEquivalentTo(Submodel.Description);
            result.Entity.DisplayName.Should().BeEquivalentTo(Submodel.DisplayName);
            result.Entity.SemanticId.Should().BeEquivalentTo(Submodel.SemanticId);
            result.Entity.Kind.Should().Be(Submodel.Kind);
        }

        [TestMethod]
        public void Test102_CreateSubmodelElement()
        {
            Property<string> property = new Property<string>("MyTestProperty", "MyTestValue");

            var result = CreateSubmodelElement(".", property);

            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(property, options =>
            {
                options
                .Excluding(p => p.EmbeddedDataSpecifications)
                .Excluding(p => p.Parent)
                .Excluding(p => p.Get)
                .Excluding(p => p.Set);
                return options;
            });
        }

        [TestMethod]
        public void Test103_CreateSubmodelElementHierarchy()
        {
            SubmodelElementCollection coll = new SubmodelElementCollection("MyCollection")
            {
                Value =
                {
                    Value =
                    {
                        new Property<string>("MySubString", "MySubStringValue"),
                        new Property<int>("MySubInt", 5),
                        new Property<double>("MySubDouble", 4.5d),
                        new Property<float>("MySubFloat", 2.3f),
                        new SubmodelElementCollection("MySubCollection")
                        {
                            Value =
                            {
                                Value =
                                {
                                    new Property<string>("MySubSubString", "MySubSubStringValue"),
                                    new Property<int>("MySubSubInt", 6),
                                    new Property<double>("MySubSubDouble", 5.5d),
                                    new Property<float>("MySubSubFloat", 3.3f),
                                    new SubmodelElementList("MySubmodelElementList1")
                                    {
                                        Value =
                                        {
                                            Value =
                                            {
                                                new Property<string>(null, "ListProperty1"),
                                                new Property<string>(null, "ListProperty2"),
                                                new Property<string>(null, "ListProperty3"),
                                                new Property<string>(null, "ListProperty4")
                                            }
                                        }
                                    },
                                    new SubmodelElementList("MySubmodelElementList2")
                                    {
                                        Value =
                                        {
                                            Value =
                                            {
                                                new SubmodelElementCollection(null)
                                                {
                                                    Value =
                                                    {
                                                        Value =
                                                        {
                                                            new Property<string>("CollectionInListProp1", "CollectionInsideListValue"),
                                                            new Property<int>("CollectionInListProp2", 7),
                                                            new Property<double>("CollectionInListProp3", 6.5d),
                                                            new Property<float>("CollectionInListProp4", 4.3f),
                                                            new Entity("CollectionInListEntity")
                                                            {
                                                                Value=
                                                                {
                                                                    Statements =
                                                                    {
                                                                        new Property<int>("p1", 20),
                                                                        new Property<int>("p2", 21)
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new SubmodelElementList("MultidimensionalArrayList")
                                    {
                                        Value =
                                        {
                                            Value =
                                            {
                                                new SubmodelElementList(null)
                                                {
                                                    Value=
                                                    {
                                                        Value =
                                                        {
                                                            new Property<int>(null, 1),
                                                            new Property<int>(null, 2),
                                                        }
                                                    }
                                                },
                                                new SubmodelElementList(null)
                                                {
                                                    Value=
                                                    {
                                                        Value =
                                                        {
                                                            new Property<int>(null, 3),
                                                            new Property<int>(null, 4),
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            Submodel.SubmodelElements.Add(coll);
            var created = CreateSubmodelElement(".", coll);
            created.Success.Should().BeTrue();
            var retrieved = RetrieveSubmodelElement("MyCollection.MySubString");
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.GetValue<string>().Should().Be("MySubStringValue");
        }

        [TestMethod]
        public void Test104_RetrieveSubmodelElements()
        {
            var retrieved = RetrieveSubmodelElements();
            retrieved.Entity.Result.Should().ContainEquivalentOf(Submodel.SubmodelElements["MyCollection"],
                options =>
                {
                    options
                      .Excluding(p => p.EmbeddedDataSpecifications)
                      .Excluding(p => p.Parent)
                      .Excluding(p => p.Get)
                      .Excluding(p => p.Set);
                    return options;
                });
        }

        [TestMethod]
        public void Test105_RetrieveSubmodelElement()
        {
            var result = RetrieveSubmodelElement("MyCollection.MySubCollection.MySubSubFloat");
            result.Entity.GetValue<float>().Should().Be(3.3f);
        }

        [TestMethod]
        public void Test106_UpdateSubmodelElement()
        {
            var mySubFloat = Submodel.SubmodelElements["MyCollection.MySubCollection.MySubSubFloat"].Cast<Property<float>>();
            mySubFloat.Description = new LangStringSet()
            {
                new LangString("de", "Meine float Property Beschreibung"),
                new LangString("en", "My float Property description")
            };
            var updated = UpdateSubmodelElement("MyCollection.MySubCollection.MySubSubFloat", mySubFloat);
            updated.Success.Should().BeTrue();
            var retrieved = RetrieveSubmodelElement("MyCollection.MySubCollection.MySubSubFloat");
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Description.Should().BeEquivalentTo(mySubFloat.Description);
        }

        [TestMethod]
        public void Test107_RetrieveSubmodelElementHierarchy()
        {
            var result = RetrieveSubmodelElement("MyCollection.MySubCollection");
            result.Success.Should().BeTrue();
            result.Entity.Cast<ISubmodelElementCollection>().Value.Value["MySubSubInt"].GetValue<int>().Should().Be(6);
        }

        [TestMethod]
        public void Test108_UpdateSubmodelElementValue()
        {
            var result = UpdateSubmodelElementValue("MyCollection.MySubCollection.MySubSubDouble", new PropertyValue(new ElementValue<double>(1.8d)));
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void Test109_RetrieveSubmodelElementValue()
        {
            var result = RetrieveSubmodelElementValue("MyCollection.MySubCollection.MySubSubDouble");
            result.Success.Should().BeTrue();
            result.Entity.GetValue<double>().Should().Be(1.8d);
        }

        [TestMethod]
        public void Test110_InvokeOperation()
        {
            InvocationRequest request = new InvocationRequest(Guid.NewGuid().ToString())
            {
                InputArguments =
                {
                    new Property<string>("Expression", "3*8"),
                    new Property<int>("ComputingTime", 100)
                }
            };

            var result = InvokeOperation("Calculate", request, false);
            result.Success.Should().BeTrue();
            result.Entity.OutputArguments["Result"].GetValue<double>().Should().Be(24);

        }

        [TestMethod]
        public void Test111_InvokeOperationAsync()
        {
            InvocationRequest request = new InvocationRequest(Guid.NewGuid().ToString())
            {
                InputArguments =
                {
                    new Property<string>("Expression", "3*8"),
                    new Property<int>("ComputingTime", 2000)
                }
            };

            var result = InvokeOperation("Calculate", request, true);
            result.Success.Should().BeTrue();
            result.Entity.ExecutionState.Should().Be(ExecutionState.Initiated);

            Thread.Sleep(2500);

            var handleResult = GetInvocationResult("Calculate", request.RequestId);
            handleResult.Success.Should().BeTrue();
            handleResult.Entity.OutputArguments["Result"].GetValue<double>().Should().Be(24);
        }

        [TestMethod]
        public void Test112_DeleteSubmodelElement()
        {
            var deleted = DeleteSubmodelElement("MyCollection");
            deleted.Success.Should().BeTrue();

            var retrieved = RetrieveSubmodelElements();
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Result.Should().NotContainEquivalentOf(Submodel.SubmodelElements["MyCollection"]);
        }

        [TestMethod]
        public void Test113_ToFromValueOnly()
        {
            var property = Submodel.SubmodelElements["TestProperty1"].Cast<IProperty>();
            PropertyValue oldPropertyValue = property.GetValueScope<PropertyValue>();            
            string json = oldPropertyValue.ToJsonValueOnly();
            PropertyValue newPropertyValue = json.FromJsonValueOnly<PropertyValue>();
            oldPropertyValue.Value.Value.Should().Be(newPropertyValue.Value.Value);
        }

        [TestMethod]
        public void Test104A_RetrieveSubmodelElementsWithLimit()
        {
            var retrieved = RetrieveSubmodelElements(2);
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Result.Should().ContainEquivalentOf(Submodel.SubmodelElements["TestProperty1"],
                options =>
                {
                    options
                        .Excluding(p => p.EmbeddedDataSpecifications)
                        .Excluding(p => p.Parent)
                        .Excluding(p => p.Get)
                        .Excluding(p => p.Set);
                    return options;
                });
            retrieved.Entity.Result.Should().ContainEquivalentOf(Submodel.SubmodelElements["TestProperty2"],
                options =>
                {
                    options
                        .Excluding(p => p.EmbeddedDataSpecifications)
                        .Excluding(p => p.Parent)
                        .Excluding(p => p.Get)
                        .Excluding(p => p.Set);
                    return options;
                });
            retrieved.Entity.Result.Children.Count().Should().Be(2);
            retrieved.Entity.PagingMetadata.Cursor.Should().Be("TestProperty3");
        }

        [TestMethod]
        public void Test104B_RetrieveSubmodelElementsWithLimitAndCursor()
        {
            var retrieved = RetrieveSubmodelElements(3, "TestProperty2");
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Result.Should().ContainEquivalentOf(Submodel.SubmodelElements["TestProperty2"],
                options =>
                {
                    options
                        .Excluding(p => p.EmbeddedDataSpecifications)
                        .Excluding(p => p.Parent)
                        .Excluding(p => p.Get)
                        .Excluding(p => p.Set);
                    return options;
                });
            retrieved.Entity.Result.Should().ContainEquivalentOf(Submodel.SubmodelElements["TestProperty3"],
                options =>
                {
                    options
                        .Excluding(p => p.EmbeddedDataSpecifications)
                        .Excluding(p => p.Parent)
                        .Excluding(p => p.Get)
                        .Excluding(p => p.Set);
                    return options;
                });
            retrieved.Entity.Result.Should().ContainEquivalentOf(Submodel.SubmodelElements["TestProperty4"],
                options =>
                {
                    options
                        .Excluding(p => p.EmbeddedDataSpecifications)
                        .Excluding(p => p.Parent)
                        .Excluding(p => p.Get)
                        .Excluding(p => p.Set);
                    return options;
                });
            retrieved.Entity.Result.Children.Count().Should().Be(3);
            retrieved.Entity.PagingMetadata.Cursor.Should().Be("TestPropertyNoSetter");
        }

        [TestMethod]
        public void Test104C_RetrieveSubmodelElementsPath()
        {
            string resultPart = "\"TestProperty1\"," +
                                "\"TestProperty2\"," +
                                "\"TestProperty3\"," +
                                "\"TestProperty4\"," +
                                "\"TestPropertyNoSetter\"," +
                                "\"TestValueChanged1\"," +
                                "\"TestSubmodelElementCollection\"," +
                                "\"TestSubmodelElementCollection.TestSubProperty1\"," +
                                "\"TestSubmodelElementCollection.TestSubProperty2\"," +
                                "\"TestSubmodelElementCollection.TestSubProperty3\"," +
                                "\"TestSubmodelElementCollection.TestSubProperty4\"," +
                                "\"GetTime\"," +
                                "\"Calculate\"," +
                                "\"NestedTestCollection\"," +
                                "\"NestedTestCollection.MySubStringProperty\"," +
                                "\"NestedTestCollection.MySubIntProperty\"," +
                                "\"NestedTestCollection.MySubTestCollection\"," +
                                "\"NestedTestCollection.MySubTestCollection.MySubSubStringProperty\"," +
                                "\"NestedTestCollection.MySubTestCollection.MySubSubIntProperty\"," +
                                "\"NestedTestCollection.MySubTestCollection.MySubSubTestCollection\"," +
                                "\"NestedTestCollection.MySubTestCollection.MySubSubTestCollection.MySubSubSubStringProperty\"," +
                                "\"NestedTestCollection.MySubTestCollection.MySubSubTestCollection.MySubSubSubIntProperty\"," +
                                "\"NestedTestCollection.MySubEntity\"," +
                                "\"NestedTestCollection.MySubEntity.MySubEntityProperty\"," +
                                "\"NestedTestCollection.MySubmodelElementList\"," +
                                "\"NestedTestCollection.MySubmodelElementList[0]\"," +
                                "\"NestedTestCollection.MySubmodelElementList[1]\"";

            string updatedPart = "\"MyCollection.MySubCollection.MySubmodelElementList1\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList1[0]\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList1[1]\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList1[2]\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList1[3]\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList2\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList2[0]\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp1\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp2\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp3\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp4\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListEntity\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListEntity.p1\"," +
                                 "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListEntity.p2\"," +
                                 "\"MyCollection.MySubCollection.MultidimensionalArrayList\"," +
                                 "\"MyCollection.MySubCollection.MultidimensionalArrayList[0]\"," +
                                 "\"MyCollection.MySubCollection.MultidimensionalArrayList[0][0]\"," +
                                 "\"MyCollection.MySubCollection.MultidimensionalArrayList[0][1]\"," +
                                 "\"MyCollection.MySubCollection.MultidimensionalArrayList[1]\"," +
                                 "\"MyCollection.MySubCollection.MultidimensionalArrayList[1][0]\"," +
                                 "\"MyCollection.MySubCollection.MultidimensionalArrayList[1][1]\"";

            var retrieved = RetrieveSubmodelElementsPath();
            retrieved.Success.Should().BeTrue();
            string jsonResult = JsonSerializer.Serialize(retrieved.Entity);
            jsonResult.Should().Contain(resultPart);
            jsonResult.Should().Contain(updatedPart);
        }

        [TestMethod]
        public void Test104D_RetrieveSubmodelElementsPathWithPagination()
        {
            var retrieved = RetrieveSubmodelElementsPath(2, "TestValueChanged1");
            retrieved.Success.Should().BeTrue();
            string jsonResult = JsonSerializer.Serialize(retrieved.Entity);
            string expectedResult =
                "[" +
                "\"TestValueChanged1\"," +
                "\"TestSubmodelElementCollection\"," +
                "\"TestSubmodelElementCollection.TestSubProperty1\"," +
                "\"TestSubmodelElementCollection.TestSubProperty2\"," +
                "\"TestSubmodelElementCollection.TestSubProperty3\"," +
                "\"TestSubmodelElementCollection.TestSubProperty4\"" +
                "]";
            jsonResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Test107A_RetrieveNestedSubmodelElementHierarchy()
        {
            var result = RetrieveSubmodelElement("MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListEntity.p1");
            result.Success.Should().BeTrue();
            result.Entity.Cast<IProperty>().GetValue<int>().Should().Be(20);
        }

        [TestMethod]
        public void Test107B_RetrieveNestedSubmodelElementListItems()
        {
            var result = RetrieveSubmodelElement("MyCollection.MySubCollection.MultidimensionalArrayList[1][0]");
            result.Success.Should().BeTrue();
            result.Entity.Cast<IProperty>().GetValue<int>().Should().Be(3);
        }

        [TestMethod]
        public void Test107C_RetrieveDynamicSubmodelElement()
        {
            var result = RetrieveSubmodelElement("MyDynamicSMC.DynamicSubSMC1.DynamicTestProp");
            result.Entity.Cast<IProperty>().GetValue<string>().Should().BeEquivalentTo("DynamicTestVal1");
        }

        [TestMethod]
        [Ignore]
        public void Test107D_RetrieveNonExistingSubmodelElement()
        {
            var result = RetrieveSubmodelElement("MyDynamicSMC.DynamicSubSMC3.DynamicTestProp");
            result.Success.Should().BeFalse();
        }
        

        [TestMethod]
        public void Test103A_CreateDynamicSubmodelElement()
        {
            var dynamicSmc = new SubmodelElementCollection("MyDynamicSMC")
            {
                Get = (element) =>
                {
                    var smc = element.Cast<ISubmodelElementCollection>();
                    var dynamicSmc = GetDynamicStructure(smc);
                    var value = new SubmodelElementCollectionValue(dynamicSmc);
                    return Task.FromResult(value);
                }
            };
            Submodel.SubmodelElements.Add(dynamicSmc);
            var created = CreateSubmodelElement(".", dynamicSmc);
            created.Success.Should().BeTrue();
        }
        
        [TestMethod]
        public void Test114_RetrieveSMEPathSerializationLevelDeepNestedSubmodelElementHierarchy()
        {
            string expectedResult = "[\"MyCollection.MySubCollection.MySubmodelElementList2\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0]\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp1\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp2\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp3\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp4\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListEntity\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListEntity.p1\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListEntity.p2\"" +
                                    "]";

            var retrieved = RetrieveSubmodelElementPath("MyCollection.MySubCollection.MySubmodelElementList2");
            retrieved.Success.Should().BeTrue();
            string jsonResult = JsonSerializer.Serialize(retrieved.Entity);

            jsonResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Test114A_RetrieveSMEPathSerializationLevelCoreSubmodelElementCollection()
        {
            string expectedResult = "[\"MyCollection.MySubCollection.MySubmodelElementList2[0]\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp1\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp2\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp3\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp4\"," +
                                    "\"MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListEntity\"" +
                                    "]";

            var retrieved = RetrieveSubmodelElementPath("MyCollection.MySubCollection.MySubmodelElementList2[0]", RequestLevel.Core);
            retrieved.Success.Should().BeTrue();
            string jsonResult = JsonSerializer.Serialize(retrieved.Entity);

            jsonResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Test114B_RetrieveSMEPathSerializationLevelCoreProperty()
        {
            string expectedResult = "[]";

            var retrieved = RetrieveSubmodelElementPath("MyCollection.MySubCollection.MySubmodelElementList2[0].CollectionInListProp1", RequestLevel.Core);
            retrieved.Success.Should().BeTrue();
            string jsonResult = JsonSerializer.Serialize(retrieved.Entity);

            jsonResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Test115_ReplaceSubmodel()
        {
            var idShort = "ReplacedSubmodel";

            var replaceSubmodel = new Submodel(idShort, new BaSyxSubmodelIdentifier("ReplacedSubmodel", "1.0.0"))
            {
                Description = new LangStringSet()
                {
                    new("de-DE", "Ersetztes Submodel"),
                    new("en-US", "Replaced submodel")
                },
                SubmodelElements = new ElementContainer<ISubmodelElement>
                {
                    new Property<string>("New_String_Property", "Replaced"),
                    new Property<int>("New_Int_Property", 262),
                    new Property<int>("Another_Int_Property", 262)
                }
            };

            var replaced = ReplaceSubmodel(replaceSubmodel);
            replaced.Success.Should().BeTrue();

            var submodel = RetrieveSubmodel().Entity;
            submodel.SubmodelElements.Count.Should().Be(3);
            submodel.Description.Should().BeEquivalentTo(replaceSubmodel.Description);
            submodel.IdShort.Should().BeEquivalentTo(idShort);

            // switch back to original model for following tests
            ReplaceSubmodel(Submodel);
        }


        [TestMethod]
        public void Test116_UpdateSubmodelMetadata()
        {
            ReplaceSubmodel(Submodel);

            var idShort = "UpdatedSubmodel";

            var updatedSubmodel = new Submodel(idShort, new BaSyxSubmodelIdentifier("ReplacedSubmodel", "1.0.0"))
            {
                Description = new LangStringSet()
                {
                    new("de-DE", "Submodel wurde ersetzt"),
                    new("en-US", "submodel was replaced")
                },
                Administration = new AdministrativeInformation()
                {
                    Version = "2.0",
                    Revision = "2"
                },
                DisplayName = new LangStringSet()
                {
                    new("de-DE", "Submodel ersetzt"),
                    new("en-US", "submodel replaced")
                },
                Category = "replaced_category",
                SubmodelElements = new ElementContainer<ISubmodelElement>
                {
                    new Property<string>("New_String_Property", "Replaced"),
                    new Property<int>("New_Int_Property", 262)
                }
            };

            var updated = UpdateSubmodelMetadata(updatedSubmodel);
            updated.Success.Should().BeTrue();

            var submodel = RetrieveSubmodel().Entity;
            submodel.SubmodelElements.Count.Should().BeGreaterThan(2);
            submodel.Category.Should().Be(updatedSubmodel.Category);
            submodel.IdShort.Should().NotBe(updatedSubmodel.IdShort);

            ReplaceSubmodel(Submodel);
        }

        [TestMethod]
        public void Test117_GetSubmodelReference()
        {
            var submodel = RetrieveSubmodel().Entity;
            var reference = submodel.CreateReference();
            var keys = reference.Keys.ToList();

            keys[0].Value.Should().Be(submodel.Id.Id);
            keys[0].Type.Should().Be(KeyType.Submodel);
        }

        [TestMethod]
        public void Test118_GetSubmodelMetadata()
        {
            var submodel = RetrieveSubmodel().Entity;
            var metadata = submodel.GetMetadata();

            metadata.SubmodelElements.Should().BeNull();
            metadata.Id.Should().Be(submodel.Id);
            metadata.Administration.Should().Be(submodel.Administration);
            metadata.Category.Should().Be(submodel.Category);
            metadata.Description.Should().BeEquivalentTo(submodel.Description);
            metadata.DisplayName.Should().BeEquivalentTo(submodel.DisplayName);
            metadata.EmbeddedDataSpecifications.Should().BeEquivalentTo(submodel.EmbeddedDataSpecifications);
            metadata.Kind.Should().Be(submodel.Kind);
            metadata.Qualifiers.Should().BeEquivalentTo(submodel.Qualifiers);
            metadata.SemanticId.Should().Be(submodel.SemanticId);
            metadata.SupplementalSemanticIds.Should().BeEquivalentTo(submodel.SupplementalSemanticIds);
        }

        [TestMethod]
        [DataRow("TestSubmodelElementCollection", "Property_String_1", true)]
        [DataRow("NestedTestCollection.MySubTestCollection", "Property_String_1", true)]
        [DataRow("TestSubmodelElementCollection", null, false)]
        public void Test119_PostSubmodelElementByPathCollection(string containerId, string? elementId, bool expectedResult)
        {
            ReplaceSubmodel(Submodel);

            var sme = RetrieveSubmodelElement(containerId).Entity;
            var originalCount = ((SubmodelElementCollection)sme).Count();

            ISubmodelElement element = new Property<string>(elementId, "new added string");
            var result = CreateSubmodelElement(containerId, element);
            result.Success.Should().Be(expectedResult);
            
            sme = RetrieveSubmodelElement(containerId).Entity;
            var newCount = ((SubmodelElementCollection)sme).Count();

            var count = newCount == originalCount + 1;
            count.Should().Be(expectedResult);            
            
            // switch back to original model for following tests
            ReplaceSubmodel(Submodel);
        }

        [TestMethod]
        [DataRow("NestedTestCollection.MySubmodelElementList", "Property_String_1", false)]
        [DataRow("NestedTestCollection.MySubmodelElementList[2]", "Property_String_1", false)]
        [DataRow("NestedTestCollection.MySubmodelElementList", null, true)]
        public void Test119_PostSubmodelElementByPathList(string containerId, string? elementId, bool expectedResult)
        {
            ReplaceSubmodel(Submodel);

            var sme = RetrieveSubmodelElement(containerId).Entity;
            var originalCount = ((SubmodelElementList)sme).Count();

            ISubmodelElement element = new Property<string>(elementId, "new added string");
            var result = CreateSubmodelElement(containerId, element);
            result.Success.Should().Be(expectedResult);

            sme = RetrieveSubmodelElement(containerId).Entity;
            var newCount = ((SubmodelElementList)sme).Count();

            var count = newCount == originalCount + 1;
            count.Should().Be(expectedResult);

            // switch back to original model for following tests
            ReplaceSubmodel(Submodel);
        }

        [TestMethod]
        [DataRow("", "TestProperty2", "ReplacedProperty", true)]
        [DataRow("NestedTestCollection", "MySubStringProperty", "ReplacedProperty", true)]
        [DataRow("NestedTestCollection.MySubmodelElementList", "[1]", null, true)]
        [DataRow("NestedTestCollection", "MySubStringProperty", null, false)]
        [DataRow("NestedTestCollection.MySubmodelElementList", "[1]", "ReplacedProperty", false)]
        public void Test120_PutSubmodelElementByPath(string path, string elementToReplace, string? elementReplacing, bool expectedResult)
        {
            var elementToReplacePath = elementToReplace;
            if (!string.IsNullOrEmpty(path))
            {
                if (elementToReplace.StartsWith("["))
                    elementToReplacePath = $"{path}{elementToReplace}";
                else
                    elementToReplacePath = $"{path}.{elementToReplace}";
            }

            var sme = RetrieveSubmodelElement(elementToReplacePath).Entity;

            ISubmodelElement element = new Property<string>(elementReplacing, "new added string");
            var result = UpdateSubmodelElement(elementToReplacePath, element);

            result.Success.Should().Be(expectedResult);

            //exit if not updated
            if (!expectedResult)
                return;

            var replacedElementPath = elementReplacing;
            if (!string.IsNullOrEmpty(path))
            {
                if (elementToReplace.StartsWith("["))
                    replacedElementPath = $"{path}{elementToReplace}";
                else
                    replacedElementPath = $"{path}.{elementReplacing}";
            }

            var rsme = RetrieveSubmodelElement(replacedElementPath).Entity;

            if (sme.IdShort == null)
                rsme.IdShort.Should().BeNull();
            else
                sme.IdShort.Should().NotBe(rsme.IdShort);

            sme.GetValue<string>().Should().NotBe(rsme.GetValue<string>());
            //switch back to original model for following tests
            ReplaceSubmodel(Submodel);
        }

        [TestMethod]
        [DataRow("", false, KeyType.Entity)]
        [DataRow("TestProperty1", true, KeyType.Property)]
        [DataRow("NestedTestCollection.MySubStringProperty", true, KeyType.Property)]
        [DataRow("NestedTestCollection.MySubmodelElementList", true, KeyType.SubmodelElementList)]
        [DataRow("NestedTestCollection.MySubmodelElementList[0]", true, KeyType.Property)]
        public void Test121_GetSubmodelElementReference(string elementPath, bool expectedResult, KeyType expectedKeyType)
        {
            var result = RetrieveSubmodelElementReference(elementPath);
            result.Success.Should().Be(expectedResult);

            if (!result.Success)
                return;

            var paths = new List<string>();

            paths.AddRange(elementPath.Split("."));

            // handle list item index
            var listElement = "[0]";
            if (paths[^1].Contains(listElement))
            {
                paths[^1] = paths[^1].Replace(listElement, "");
                paths.Add(null);
            }

            var keys = result.Entity.Keys.ToList();

            keys.Count().Should().Be(paths.Count + 1);

            keys[0].Value.Should().Be(Submodel.Id.Id);
            keys[0].Type.Should().Be(KeyType.Submodel);

            for (var i = 1; i < keys.Count; i++)
                keys[i].Value.Should().Be(paths[i - 1]);
            
            keys[^1].Type.Should().Be(expectedKeyType);
        }

        #endregion

        #region implementations

        private IElementContainer<ISubmodelElement> GetDynamicStructure(ISubmodelElementCollection baseSmc)
        {
            ElementContainer<ISubmodelElement> dynamicContainer = new ElementContainer<ISubmodelElement>(baseSmc.Parent, baseSmc, null);
            var ItemDictionary = new Dictionary<string, string>
            {
                { "DynamicSubSMC1", "DynamicTestVal1" },
                { "DynamicSubSMC2", "DynamicTestVal2" },
            };
            foreach (var item in ItemDictionary)
            {
                var smc = new SubmodelElementCollection(item.Key)
                {
                    new Property<string>("DynamicTestProp", item.Value),
                    new Property<string>("CurrentTime")
                    {
                        Get = (prop) =>
                        {
                            return Task.FromResult(DateTime.Now.ToString());
                        }
                    }
                };
                dynamicContainer.Create(smc);
            }
            return dynamicContainer;
        }


        public IResult<ISubmodel> RetrieveSubmodel()
        {
            return ((ISubmodelClient)Client).RetrieveSubmodel();
        }

        public IResult UpdateSubmodel(ISubmodel submodel)
        {
            return ((ISubmodelClient)Client).UpdateSubmodel(submodel);
        }

        public IResult UpdateSubmodelMetadata(ISubmodel submodel)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelMetadata(submodel);
        }

        public IResult ReplaceSubmodel(ISubmodel submodel)
        {
            return ((ISubmodelClient)Client).ReplaceSubmodel(submodel);
        }

        public IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return ((ISubmodelClient)Client).CreateSubmodelElement(rootIdShortPath, submodelElement);
        }

        public IResult DeleteSubmodelElement(string idShortPath)
        {
            return ((ISubmodelClient)Client).DeleteSubmodelElement(idShortPath);
        }

        public IResult<InvocationResponse> GetInvocationResult(string idShortPath, string requestId)
        {
            return ((ISubmodelClient)Client).GetInvocationResult(idShortPath, requestId);
        }

        public IResult<InvocationResponse> InvokeOperation(string idShortPath, InvocationRequest invocationRequest, bool async)
        {
            return ((ISubmodelClient)Client).InvokeOperation(idShortPath, invocationRequest, async);
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElement(idShortPath);
        }

        public IResult<PagedResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElements(int limit  = 100, string cursor = "")
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElements(limit, cursor);
        }


        public IResult<ValueScope> RetrieveSubmodelElementValue(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementValue(idShortPath);
        }

        public IResult<IReference> RetrieveSubmodelElementReference(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementReference(idShortPath);
        }

        public IResult<PagedResult<IReference>> RetrieveSubmodelElementsReference(int limit = 100, string cursor = "")
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementsReference(limit, cursor);
        }

        public IResult<List<string>> RetrieveSubmodelElementPath(string idShortPath, RequestLevel level = RequestLevel.Deep)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementPath(idShortPath, level);
        }

        public IResult<List<string>> RetrieveSubmodelElementsPath(int limit = 100,
            string cursor = "", RequestLevel level = RequestLevel.Deep)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementsPath(limit, cursor, level);
        }

        public IResult UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelElement(rootIdShortPath, submodelElement);
        }

        public IResult UpdateSubmodelElementValue(string idShortPath, ValueScope value)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelElementValue(idShortPath, value);
        }

        public Task<IResult<ISubmodel>> RetrieveSubmodelAsync(RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelAsync(level, extent);
        }

        public Task<IResult> UpdateSubmodelAsync(ISubmodel submodel)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelAsync(submodel);
        }

        public Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return ((ISubmodelClient)Client).CreateSubmodelElementAsync(rootIdShortPath, submodelElement);
        }

        public Task<IResult> UpdateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelElementAsync(rootIdShortPath, submodelElement);
        }

        public Task<IResult<PagedResult<IElementContainer<ISubmodelElement>>>> RetrieveSubmodelElementsAsync(int limit = 100, string cursor = "", RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementsAsync(limit, cursor, level, extent);
        }

        public Task<IResult<List<string>>> RetrieveSubmodelElementsPathAsync(int limit = 100, string cursor = "", RequestLevel level = RequestLevel.Deep)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementsPathAsync(limit, cursor, level);
        }

        public Task<IResult<ISubmodelElement>> RetrieveSubmodelElementAsync(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementAsync(idShortPath);
        }

        public Task<IResult<ValueScope>> RetrieveSubmodelElementValueAsync(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementValueAsync(idShortPath);
        }

        public Task<IResult<List<string>>> RetrieveSubmodelElementPathAsync(string idShortPath, RequestLevel level = RequestLevel.Deep)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementPathAsync(idShortPath);
        }

        public Task<IResult> UpdateSubmodelElementValueAsync(string idShortPath, ValueScope value)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelElementValueAsync(idShortPath, value);
        }

        public Task<IResult> DeleteSubmodelElementAsync(string idShortPath)
        {
            return ((ISubmodelClient)Client).DeleteSubmodelElementAsync(idShortPath);
        }

        public Task<IResult<InvocationResponse>> InvokeOperationAsync(string idShortPath, InvocationRequest invocationRequest, bool async = false)
        {
            return ((ISubmodelClient)Client).InvokeOperationAsync(idShortPath, invocationRequest, async);
        }

        public Task<IResult<InvocationResponse>> GetInvocationResultAsync(string idShortPath, string requestId)
        {
            return ((ISubmodelClient)Client).GetInvocationResultAsync(idShortPath, requestId);
        }

#endregion
    }
}
