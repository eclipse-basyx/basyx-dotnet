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
using BaSyx.Models.Semantics;
using BaSyx.Models.Export.EnvironmentDataSpecifications;
using System;
using BaSyx.Models.AdminShell;

namespace BaSyx.Models.Export.Converter
{
    public static class ConceptDescriptionConverter_V1_0
    {
        public static DataSpecificationIEC61360 ToDataSpecificationIEC61360(this EnvironmentDataSpecificationIEC61360_V1_0 environmentDataSpecification)
        {
            if (environmentDataSpecification == null)
                return null;

            DataSpecificationIEC61360 dataSpecification = new DataSpecificationIEC61360(new DataSpecificationIEC61360Content()
            {                
                Definition = environmentDataSpecification.Definition,
                PreferredName = environmentDataSpecification.PreferredName,
                ShortName = string.IsNullOrEmpty(environmentDataSpecification.ShortName) ? null :
                            new LangStringSet() { new LangString("Undefined", environmentDataSpecification.ShortName) },
                SourceOfDefinition = environmentDataSpecification.SourceOfDefinition?["EN"],
                Symbol = environmentDataSpecification.Symbol,
                Unit = environmentDataSpecification.Unit,
                UnitId = environmentDataSpecification.UnitId?.ToReference_V1_0(),
                Value = null,
                ValueFormat = environmentDataSpecification.ValueFormat,
                ValueId = null,
                ValueList = null
            });

            if (!string.IsNullOrEmpty(environmentDataSpecification.DataType))
                (dataSpecification.DataSpecificationContent as DataSpecificationIEC61360Content).DataType = 
                    (DataTypeIEC61360)Enum.Parse(typeof(DataTypeIEC61360), environmentDataSpecification.DataType);

            return dataSpecification;
        }

        public static EnvironmentDataSpecificationIEC61360_V1_0 ToEnvironmentDataSpecificationIEC61360_V1_0(this DataSpecificationIEC61360Content dataSpecificationContent)
        {
            if (dataSpecificationContent == null)
                return null;

            EnvironmentDataSpecificationIEC61360_V1_0 environmentDataSpecification = new EnvironmentDataSpecificationIEC61360_V1_0()
            {
                DataType = dataSpecificationContent.DataType.ToString(),
                Definition = dataSpecificationContent.Definition,
                PreferredName = dataSpecificationContent.PreferredName,
                ShortName = dataSpecificationContent.ShortName?["EN"],
                SourceOfDefinition = new LangStringSet() { new LangString("Undefined", dataSpecificationContent.SourceOfDefinition) },
                Symbol = dataSpecificationContent.Symbol,
                Unit = dataSpecificationContent.Unit,
                UnitId = dataSpecificationContent.UnitId?.ToEnvironmentReference_V1_0(),
                ValueFormat = dataSpecificationContent.ValueFormat
            };

            return environmentDataSpecification;
        }
    }
}
