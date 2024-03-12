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
    }
}
