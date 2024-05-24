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
using BaSyx.Models.Semantics;
using BaSyx.Models.Export.EnvironmentDataSpecifications;
using System;
using BaSyx.Models.AdminShell;

namespace BaSyx.Models.Export.Converter
{
    public static class ConceptDescriptionConverter_V3_0
    {
        public static DataSpecificationIEC61360 ToDataSpecificationIEC61360(this EnvironmentDataSpecificationIEC61360_V3_0 environmentDataSpecification)
        {
            if (environmentDataSpecification == null)
                return null;

            if (!Enum.TryParse<DataTypeIEC61360>(environmentDataSpecification.DataType.ToString(), out DataTypeIEC61360 dataType))
                dataType = DataTypeIEC61360.UNDEFINED;

            var content = new DataSpecificationIEC61360Content()
            {
                DataType = dataType,
                Definition = environmentDataSpecification.Definition?.ConvertAll(l => new LangString(l.Language, l.Text))?.ToLangStringSet(),
                PreferredName = environmentDataSpecification.PreferredName?.ConvertAll(l => new LangString(l.Language, l.Text))?.ToLangStringSet(),
                ShortName = environmentDataSpecification.ShortName?.ConvertAll(l => new LangString(l.Language, l.Text))?.ToLangStringSet(),
                SourceOfDefinition = environmentDataSpecification.SourceOfDefinition,
                Symbol = environmentDataSpecification.Symbol,
                Unit = environmentDataSpecification.Unit,
                UnitId = environmentDataSpecification.UnitId?.ToReference_V3_0(),
                Value = environmentDataSpecification.Value,
                ValueFormat = environmentDataSpecification.ValueFormat,
                ValueList = new ValueList()
                {
                    ValueReferencePairs = environmentDataSpecification.ValueList?.ConvertAll(c => new Semantics.ValueReferencePair()
                    {
                        Value = c.Value,
                        ValueId = c.ValueId?.ToReference_V3_0()
                    })
                }
            };

            content.LevelType = new LevelType()
            {
                Min = environmentDataSpecification.LevelType?.Min ?? false,
                Nom = environmentDataSpecification.LevelType?.Nom ?? false,
                Typ = environmentDataSpecification.LevelType?.Typ ?? false,
                Max = environmentDataSpecification.LevelType?.Max ?? false
            };

            DataSpecificationIEC61360 dataSpecification = new DataSpecificationIEC61360(content);
            return dataSpecification;
        }

        public static EnvironmentDataSpecificationIEC61360_V3_0 ToEnvironmentDataSpecificationIEC61360_V3_0(this DataSpecificationIEC61360Content dataSpecificationContent)
        {
            if (dataSpecificationContent == null)
                return null;

            if (!Enum.TryParse<EnvironmentDataTypeIEC61360_V3_0>(dataSpecificationContent.DataType.ToString(), out EnvironmentDataTypeIEC61360_V3_0 dataType))
                dataType = EnvironmentDataTypeIEC61360_V3_0.UNDEFINED;

            EnvironmentDataSpecificationIEC61360_V3_0 environmentDataSpecification = new EnvironmentDataSpecificationIEC61360_V3_0()
            {
                DataType = dataType,
                Definition = dataSpecificationContent.Definition?.ToEnvironmentLangStringSet(),
                PreferredName = dataSpecificationContent.PreferredName?.ToEnvironmentLangStringSet(),
                ShortName = dataSpecificationContent.ShortName?.ToEnvironmentLangStringSet(),
                SourceOfDefinition = dataSpecificationContent.SourceOfDefinition,
                Symbol = dataSpecificationContent.Symbol,
                Unit = dataSpecificationContent.Unit,
                UnitId = dataSpecificationContent.UnitId?.ToEnvironmentReference_V3_0(),
                Value = dataSpecificationContent.Value,
                ValueFormat = dataSpecificationContent.ValueFormat,
                ValueList = dataSpecificationContent.ValueList?.ValueReferencePairs?.ConvertAll(c => new ValueReferencePair_V3_0()
                {
                    Value = c.Value,
                    ValueId = c.ValueId?.ToEnvironmentReference_V3_0()
                }),
                LevelType = new EnvironmentLevelType_V3_0()
                {
                    Min = dataSpecificationContent.LevelType?.Min ?? false,
                    Nom = dataSpecificationContent.LevelType?.Nom ?? false,
                    Typ = dataSpecificationContent.LevelType?.Typ ?? false,
                    Max = dataSpecificationContent.LevelType?.Max ?? false
                }
            };

            return environmentDataSpecification;
        }
    }
}
