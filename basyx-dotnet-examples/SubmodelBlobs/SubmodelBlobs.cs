/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

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
                AssetInformation = new AssetInformation()
                {
                    AssetKind = AssetKind.Instance,
                    GlobalAssetId = new BaSyxAssetIdentifier("SimpleAsset", "1.0.0")
                }
            };

            Submodel testSubmodel = GetTestSubmodel();

            aas.Submodels.Add(testSubmodel);

            return aas;
        }

        public static Submodel GetTestSubmodel()
        {
	        var blob = new Blob("TheBlob");
            blob.SetValue("decaf");

            var subBlob = new Blob("TheSubBlob");
            subBlob.SetValue("decaf");

			Submodel testSubmodel = new Submodel("TestSubmodel", new BaSyxSubmodelIdentifier("TestSubmodel", "1.0.0"))
            {
                SubmodelElements = new ElementContainer<ISubmodelElement>
				{
					new Property<string>("StringProperty", "Level 1 String"),
					blob,
					new Property<int>("IntProperty", 42),
					new Property<double>("DoubleProperty", 42),
                    new SubmodelElementCollection("TestSubmodelElementCollectionProperty")
                    {
                        Value =
                        {
							new Property<string>("StringSubProperty", "Level 2 String"),
							subBlob,
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

                }
            };

            return testSubmodel;
        }

        private static void SimpleAssetAdministrationShell_ValueChanged(object sender, ValueChangedArgs e)
        {
            logger.Info($"Property {e.IdShort} changed to {e.ValueScope.ToJson()}");
        }
    }
}
