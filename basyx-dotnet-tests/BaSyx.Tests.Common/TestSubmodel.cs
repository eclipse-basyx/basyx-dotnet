﻿/*******************************************************************************
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
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaSyx.Models.Semantics;

namespace SimpleAssetAdministrationShell
{
    public static class TestSubmodel
    {
        public static Submodel GetSubmodel(string id)
        {
            string propertyValue = "TestFromInside";
            int i = 0;
            double y = 2.0;

            Submodel testSubmodel = new Submodel(id, new BaSyxSubmodelIdentifier(id, "1.0.0"))
            {
                DisplayName = new LangStringSet()
                {
                    new("de-DE", "Submodel"),
                    new("en-US", "submodel")
                },
                SubmodelElements =
                {
                    new Property<string>("TestProperty1")
                    {
                        Set = (prop, val) => { propertyValue = val; return Task.CompletedTask; },
                        Get = prop => { return Task.FromResult(propertyValue + "_" + i ++); }
                    },
                    new Property<string>("TestProperty2")
                    {
                        Set = (prop, val) => { propertyValue = val; return Task.CompletedTask; },
                        Get = prop => { return Task.FromResult(propertyValue + "_" + i ++); }
                    },
                    new Property<int>("TestProperty3")
                    {
                        Set = (prop, val) => { i = val; return Task.CompletedTask; },
                        Get = prop => { return Task.FromResult(i++); }
                    },
                    new Property<double>("TestProperty4")
                    {
                        Set = (prop, val) => { y = val; return Task.CompletedTask; },
                        Get = prop => { return Task.FromResult(Math.Pow(y, i)); }
                    },
                    new Property<string>("TestPropertyNoSetter")
                    {
                        Set = null,
                        Get = prop => { return Task.FromResult("You can't change me!"); }
                    },
                    new Property<string>("TestValueChanged1", "InitialValue"),
                    new SubmodelElementCollection("TestSubmodelElementCollection")
                    {
                        Value =
                        {
                            Value =
                            {
                                new Property<string>("TestSubProperty1")
                                {
                                    Set = (prop, val) => { propertyValue = val; return Task.CompletedTask; },
                                    Get = prop => { return Task.FromResult(propertyValue + "_" + i--); }
                                },
                                new Property<string>("TestSubProperty2")
                                {
                                    Set = (prop, val) => { propertyValue = val; return Task.CompletedTask; },
                                    Get = prop => { return Task.FromResult(propertyValue + "_" + i--); }
                                },
                                new Property<int>("TestSubProperty3")
                                {
                                    Set = (prop, val) => { i = val; return Task.CompletedTask; },
                                    Get = prop => { return Task.FromResult(i--); }
                                },
                                new Property<double>("TestSubProperty4")
                                {
                                    Set = (prop, val) => { y = val; return Task.CompletedTask; },
                                    Get = prop => { return Task.FromResult(Math.Pow(y, i)); }
                                }
                            }
                        }
                    },
                    new Operation("GetTime")
                    {
                        OutputVariables = new OperationVariableSet()
                        {
                            new Property<string>("Date"),
                            new Property<string>("Time"),
                            new Property<string>("Ticks")
                        },
                        OnMethodCalled = (op, inArgs, inOutArgs, outArgs, cancellationToken) =>
                        {
                            outArgs.Add(new Property<string>("Date", "Heute ist der " + DateTime.Now.Date.ToString()));
                            outArgs.Add(new Property<string>("Time", "Es ist " + DateTime.Now.TimeOfDay.ToString() + " Uhr"));
                            outArgs.Add(new Property<string>("Ticks", "Ticks: " + DateTime.Now.Ticks.ToString()));
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
                       },
                       OnMethodCalled = async (op, inArgs, inOutArgs, outArgs, cancellationToken) =>
                       {
                           string expression = inArgs["Expression"]?.GetValue<string>();
                           int? computingTime = inArgs["ComputingTime"]?.GetValue<int>();

                           inOutArgs["HierRein"]?.SetValueAsync("DaWiederRaus");

                           if(computingTime.HasValue)
                            await Task.Delay(computingTime.Value, cancellationToken);

                           if(cancellationToken.IsCancellationRequested)
                               return new OperationResult(false, new Message(MessageType.Information, "Cancellation was requested"));

                           double value = CalulcateExpression(expression);

                           outArgs.Add(new Property<double>("Result", value));
                           return new OperationResult(true);
                       }
                    },
                    new SubmodelElementCollection("NestedTestCollection")
                    {
                        Value =
                        {
                            Value =
                            {
                                new Property<string>("MySubStringProperty")
                                {
                                    Set = (prop, val) => { propertyValue = val; return Task.CompletedTask; },
                                    Get = prop => { return Task.FromResult("MySubStringPropertyValue"); }
                                },
                                new Property<int>("MySubIntProperty")
                                {
                                    Set = (prop, val) => { i = val; return Task.CompletedTask; },
                                    Get = prop => { return Task.FromResult(12); }
                                },
                                new SubmodelElementCollection("MySubTestCollection")
                                {
                                    Value =
                                    {
                                        Value =
                                        {
                                            new Property<string>("MySubSubStringProperty")
                                            {
                                                Set = (prop, val) => { propertyValue = val; return Task.CompletedTask; },
                                                Get = prop => { return Task.FromResult("MySubSubStringPropertyValue"); }
                                            },
                                            new Property<int>("MySubSubIntProperty")
                                            {
                                                Set = (prop, val) => { i = val; return Task.CompletedTask; },
                                                Get = prop => { return Task.FromResult(127); }
                                            },
                                            new SubmodelElementCollection("MySubSubTestCollection")
                                            {
                                                Value =
                                                {
                                                    Value =
                                                    {
                                                        new Property<string>("MySubSubSubStringProperty")
                                                        {
                                                            Set = (prop, val) => { propertyValue = val; return Task.CompletedTask; },
                                                            Get = prop => { return Task.FromResult("MySubSubStringPropertyValue"); }
                                                        },
                                                        new Property<int>("MySubSubSubIntProperty")
                                                        {
                                                            Set = (prop, val) => { i = val; return Task.CompletedTask; },
                                                            Get = prop => { return Task.FromResult(127); }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new Entity("MySubEntity")
                                {
                                    Value =
                                    {
                                        Statements =
                                        {
                                            new Property<string>("MySubEntityProperty")
                                            {
                                                Set = (prop, val) => { propertyValue = val; return Task.CompletedTask; },
                                                Get = prop => { return Task.FromResult("MySubSubEntityPropertyValue"); }
                                            }
                                        }
                                    }

                                },
                                new SubmodelElementList("MySubmodelElementList")
                                {
                                    Value =
                                    {
                                        Value =
                                        {
                                            new Property<string>(null)
                                            {
                                                Set = (prop, val) => { propertyValue = val; return Task.CompletedTask; },
                                                Get = prop => { return Task.FromResult("MyListStringPropertyValue1"); }
                                            },
                                            new Property<string>(null)
                                            {
                                                Set = (prop, val) => { propertyValue = val; return Task.CompletedTask; },
                                                Get = prop => { return Task.FromResult("MyListStringPropertyValue2"); }
                                            },
                                            new SubmodelElementList(null)
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return testSubmodel;
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
