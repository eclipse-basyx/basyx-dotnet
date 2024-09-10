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

        [TestMethod]
        public void Test100_UpdateSubmodel()
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
        public void Test101_RetrieveSubmodel()
        {
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
                    new Property<int>("New_Int_Property", 262)
                }
            };

            var replaced = ReplaceSubmodel(replaceSubmodel);
            replaced.Success.Should().BeTrue();

            var submodel = RetrieveSubmodel().Entity;
            submodel.SubmodelElements.Count.Should().Be(2);
            submodel.Description.Should().BeEquivalentTo(replaceSubmodel.Description);
            submodel.IdShort.Should().BeEquivalentTo(idShort);
        }


        [TestMethod]
        public void Test116_UpdateSubmodelMetadata()
        {
            var idShort = "UpdatedSubmodel";

            var replaceSubmodel = new Submodel(idShort, new BaSyxSubmodelIdentifier("ReplacedSubmodel", "1.0.0"))
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

            var replaced = UpdateSubmodelMetadata(replaceSubmodel);
            replaced.Success.Should().BeTrue();

            var submodel = RetrieveSubmodel().Entity;
            submodel.SubmodelElements.Count.Should().BeGreaterThan(2);
            submodel.Category.Should().BeEquivalentTo(replaceSubmodel.Category);
            submodel.IdShort.Should().NotBeEquivalentTo(replaceSubmodel.IdShort);
        }

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


        public IResult<ISubmodel> RetrieveSubmodel(RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodel(level, extent);
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
    }
}
