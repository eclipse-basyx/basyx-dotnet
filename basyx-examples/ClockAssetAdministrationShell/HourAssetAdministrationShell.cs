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
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;

namespace ClockAssetAdministrationShell
{
    public static class HourAssetAdministrationShell
    {
        public static AssetAdministrationShell GetAssetAdministrationShell()
        {
            AssetAdministrationShell aas = new AssetAdministrationShell("HourAAS", new BaSyxShellIdentifier("HourAAS", "1.0.0"))
            {
                Description = new LangStringSet()
                {
                   new LangString("de-DE", "Stunden VWS"),
                   new LangString("en-US", "Hour AAS")
                },
                Administration = new AdministrativeInformation()
                {
                    Version = "1.0",
                    Revision = "120"
                },
                Asset = new Asset("HourAsset", new BaSyxAssetIdentifier("HourAsset", "1.0.0"))
                {
                    Kind = AssetKind.Instance,
                    Description = new LangStringSet()
                    {
                          new LangString("de-DE", "Stunden Asset"),
                          new LangString("en-US", "Hour Asset")
                    }
                }
            };

            Submodel hourSubmodel = GetHourSubmodel();

            aas.Submodels.Add(hourSubmodel);

            return aas;
        }

        public static Submodel GetHourSubmodel()
        {
            Submodel hourSubmodel = new Submodel("HourSubmodel", new BaSyxSubmodelIdentifier("HourSubmodel", "1.0.0"))
            {
                SubmodelElements =
                {
                    new Operation("GetHours")
                    {
                        OutputVariables = new OperationVariableSet()
                        {
                            new Property<string>("Hours")
                        },
                        OnMethodCalled = (op, inArgs, inOutArgs, outArgs, cancellationToken) =>
                        {
                            outArgs.Add(new Property<string>("Hours", DateTime.Now.Hour.ToString()));

                            return new OperationResult(true);
                        }
                    }                  
                }
            };
            return hourSubmodel;
        }
    }
}
