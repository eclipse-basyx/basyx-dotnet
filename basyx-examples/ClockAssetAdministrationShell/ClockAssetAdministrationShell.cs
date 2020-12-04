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
using BaSyx.AAS.Client.Http;
using BaSyx.Models.Communication;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling;
using System;

namespace ClockAssetAdministrationShell
{
    public static class ClockAssetAdministrationShell
    {
        public static AssetAdministrationShell GetAssetAdministrationShell()
        {
            AssetAdministrationShell aas = new AssetAdministrationShell("ClockAAS", new BaSyxShellIdentifier("ClockAAS", "1.0.0"))
            {
                Description = new LangStringSet()
                {
                   new LangString("de-DE", "Uhr VWS"),
                   new LangString("en-US", "Clock AAS")
                },
                Administration = new AdministrativeInformation()
                {
                    Version = "1.0",
                    Revision = "120"
                },
                Asset = new Asset("ClockAsset", new BaSyxAssetIdentifier("ClockAsset", "1.0.0"))
                {
                    Kind = AssetKind.Instance,
                    Description = new LangStringSet()
                    {
                          new LangString("de-DE", "Uhr Asset"),
                          new LangString("en-US", "Clock Asset")
                    }
                }
            };

            Submodel clockSubmodel = GetClockSubmodel();

            aas.Submodels.Add(clockSubmodel);

            return aas;
        }

        public static Submodel GetClockSubmodel()
        {
            Submodel clockSubmodel = new Submodel("ClockSubmodel", new BaSyxSubmodelIdentifier("ClockSubmodel", "1.0.0"))
            {
                SubmodelElements =
                {
                    new Operation("GetTime")
                    {
                        OutputVariables = new OperationVariableSet()
                        {
                            new Property<string>("Hours"),
                            new Property<string>("Minutes"),
                            new Property<string>("Seconds")
                        },
                        OnMethodCalled = (op, inArgs, inOutArgs, outArgs, cancellationToken) =>
                        {
                            AssetAdministrationShellHttpClient hoursClient = new AssetAdministrationShellHttpClient(new Uri("http://localhost:5081/aas"));
                            //AssetAdministrationShellHttpClient minutesClient = new AssetAdministrationShellHttpClient(new Uri("http://localhost:5082/aas"));
                            //AssetAdministrationShellHttpClient secondsClient = new AssetAdministrationShellHttpClient(new Uri("http://localhost:5083/aas"));

                            var hoursResult = hoursClient.InvokeOperation("HourSubmodel", "GetHours", new InvocationRequest(Guid.NewGuid().ToString()));
                            outArgs.Add(new Property<string>("Hours") { Value = hoursResult.Entity.OutputArguments.Get("Hours").GetValue<string>() });

                            //var minutesResult = hoursClient.InvokeOperation("MinuteSubmodel", "GetMinutes", new InvocationRequest(Guid.NewGuid().ToString()));
                            //outArgs.Add(new Property<string>("Minutes") { Value = minutesResult.Entity.OutputArguments.Get("Minutes").GetValue<string>() });

                            //var secondsResult = hoursClient.InvokeOperation("SecondSubmodel", "GetSeconds", new InvocationRequest(Guid.NewGuid().ToString()));
                            //outArgs.Add(new Property<string>("Seconds") { Value = secondsResult.Entity.OutputArguments.Get("Seconds").GetValue<string>() });

                            return new OperationResult(true);
                        }
                    },                   
                }
            };
            return clockSubmodel;
        }
    }
}
