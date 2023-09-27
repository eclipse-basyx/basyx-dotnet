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
using BaSyx.Models.AdminShell;
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling;
using NLog;
using System;
using System.Threading.Tasks;

namespace SimpleAssetAdministrationShell
{
    public static class SimpleAssetAdministrationShell
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public static AssetAdministrationShell GetAssetAdministrationShell()
        {
            AssetAdministrationShell aas = new AssetAdministrationShell("SimpleAAS", new BaSyxShellIdentifier("SimpleAAS", "1.0.0"))
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
                Asset = new Asset("SimpleAsset", new BaSyxAssetIdentifier("SimpleAsset", "1.0.0"))
                {
                    Kind = AssetKind.Instance,
                    Description = new LangStringSet()
                    {
                          new LangString("de-DE", "Einfaches Asset"),
                          new LangString("en-US", "Simple Asset")
                    }
                }
            };

            Submodel testSubmodel = GetTestSubmodel();

            aas.Submodels.Add(testSubmodel);

            return aas;
        }

        public static Submodel GetTestSubmodel()
        {
            string propertyValue = "TestFromInside";
            int i = 0;
            double y = 2.0;

            Submodel testSubmodel = new Submodel("TestSubmodel", new BaSyxSubmodelIdentifier("TestSubmodel", "1.0.0"))
            {
                SubmodelElements =
                {
                    new Property<string>("TestProperty1")
                    {
                        Set = (prop, val) => propertyValue = val,
                        Get = prop => { return propertyValue + "_" + i++; }
                    },
                    new Property<string>("TestProperty2")
                    {
                        Set = (prop, val) => propertyValue = val,
                        Get = prop => { return propertyValue + "_" + i++; }
                    },
                    new Property<int>("TestProperty3")
                    {
                        Set = (prop, val) => i = val,
                        Get = prop => { return i++; }
                    },
                    new Property<double>("TestProperty4")
                    {
                        Set = (prop, val) => y = val,
                        Get = prop => { return Math.Pow(y, i); }
                    },
                    new Property<string>("TestPropertyNull")
                    {
                        Set = (prop, val) => propertyValue = val,
                        Get = prop => { return null; }
                    },
                    new Property<string>("TestPropertyNoSetter")
                    {
                        Set = null,
                        Get = prop => { return "You can't change me!"; }
                    },
                    new Property<string>("TestValueChanged1", "InitialValue"),
                    new SubmodelElementCollection("TestSubmodelElementCollection")
                    {
                        Value =
                        {
                            new Property<string>("TestSubProperty1")
                            {
                                Set = (prop, val) => propertyValue = val,
                                Get = prop => { return propertyValue + "_" + i--; }
                            },
                            new Property<string>("TestSubProperty2")
                            {
                                Set = (prop, val) => propertyValue = val,
                                Get = prop => { return propertyValue + "_" + i--; }
                            },
                            new Property<int>("TestSubProperty3")
                            {
                                Set = (prop, val) => i = val,
                                Get = prop => { return i--; }
                            },
                            new Property<double>("TestSubProperty4")
                            {
                                Set = (prop, val) => y = val,
                                Get = prop => { return Math.Pow(y, i); }
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
                            outArgs.Add(new Property<string>("Date") { Value = "Heute ist der " + DateTime.Now.Date.ToString() });
                            outArgs.Add(new Property<string>("Time") { Value = "Es ist " + DateTime.Now.TimeOfDay.ToString() + " Uhr" });
                            outArgs.Add(new Property<string>("Ticks") { Value = "Ticks: " + DateTime.Now.Ticks.ToString() });
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

                           inOutArgs["HierRein"]?.SetValue("DaWiederRaus");

                           if(computingTime.HasValue)
                            await Task.Delay(computingTime.Value, cancellationToken);
                     
                           if(cancellationToken.IsCancellationRequested)
                               return new OperationResult(false, new Message(MessageType.Information, "Cancellation was requested"));

                           double value = CalulcateExpression(expression);

                           outArgs.Add(new Property<double>("Result", value));
                           return new OperationResult(true);
                       }
                    }

                }
            };

            testSubmodel.SubmodelElements["TestProperty4"].Cast<IProperty>().ValueChanged += SimpleAssetAdministrationShell_ValueChanged;

            return testSubmodel;
        }

        private static void SimpleAssetAdministrationShell_ValueChanged(object sender, ValueChangedArgs e)
        {
            logger.Info($"Property {e.IdShort} changed to {e.Value}");
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
