/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaSyx.Models.Export.Converter
{
    public class JsonQualifierConverter_V2_0 : JsonConverter<List<EnvironmentConstraint_V2_0>>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override List<EnvironmentConstraint_V2_0> ReadJson(JsonReader reader, Type objectType, List<EnvironmentConstraint_V2_0> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                JArray jArray = JArray.Load(reader);
                List<EnvironmentConstraint_V2_0> constraints = new List<EnvironmentConstraint_V2_0>();
                foreach (var item in jArray)
                {
                    ModelType modelType = item.SelectToken("modelType")?.ToObject<ModelType>();
                    if(modelType != null)
                    {
                        switch (modelType.Name)
                        {
                            case "Qualifier":
                                var qualifier = item.ToObject<EnvironmentQualifier_V2_0>(serializer);
                                if (qualifier != null)
                                    constraints.Add(new EnvironmentConstraint_V2_0() { Constraint = qualifier });
                                break;
                            case "Formula":
                                var formula = item.ToObject<EnvironmentFormula_V2_0>(serializer);
                                if (formula != null)
                                    constraints.Add(new EnvironmentConstraint_V2_0() { Constraint = formula });
                                break;
                            default:
                                break;
                        }
                    }
                }
                return constraints;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, List<EnvironmentConstraint_V2_0> value, JsonSerializer serializer)
        {
            try
            {
                var values = value?.Select(s => s.Constraint);
                JArray jArray = JArray.FromObject(values, serializer);
                jArray.WriteTo(writer);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }
    }
}
