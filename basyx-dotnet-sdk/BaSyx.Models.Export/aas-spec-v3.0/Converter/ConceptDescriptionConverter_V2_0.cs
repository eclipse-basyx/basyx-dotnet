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

namespace BaSyx.Models.Export.Converter
{
    public static class ConceptDescriptionConverter_V2_0
    {
        public static DataSpecificationIEC61360 ToDataSpecificationIEC61360(this EnvironmentDataSpecificationIEC61360_V2_0 environmentDataSpecification)
        {
            if (environmentDataSpecification == null)
                return null;

            if (!Enum.TryParse<DataTypeIEC61360>(environmentDataSpecification.DataType.ToString(), out DataTypeIEC61360 dataType))
                dataType = DataTypeIEC61360.UNDEFINED;

            DataSpecificationIEC61360 dataSpecification = new DataSpecificationIEC61360(new DataSpecificationIEC61360Content()
            {
                DataType = dataType,
                Definition = environmentDataSpecification.Definition,
                PreferredName = environmentDataSpecification.PreferredName,
                ShortName = environmentDataSpecification.ShortName,
                SourceOfDefinition = environmentDataSpecification.SourceOfDefinition,
                Symbol = environmentDataSpecification.Symbol,
                Unit = environmentDataSpecification.Unit,
                UnitId = environmentDataSpecification.UnitId?.ToReference_V2_0(),
                Value = environmentDataSpecification.Value,
                ValueFormat = environmentDataSpecification.ValueFormat,
                ValueId = environmentDataSpecification.ValueId?.ToReference_V2_0(),
                ValueList = environmentDataSpecification.ValueList?.ConvertAll(c => new Semantics.ValueReferencePair()
                {
                    Value = c.Value,
                    ValueId = c.ValueId?.ToReference_V2_0()
                }),
                LevelTypes = environmentDataSpecification.LevelTypes?.ConvertAll(c => (LevelType)Enum.Parse(typeof(LevelType), c.ToString()))
            });

            return dataSpecification;
        }

        public static EnvironmentDataSpecificationIEC61360_V2_0 ToEnvironmentDataSpecificationIEC61360_V2_0(this DataSpecificationIEC61360Content dataSpecificationContent)
        {
            if (dataSpecificationContent == null)
                return null;

            if(!Enum.TryParse<EnvironmentDataTypeIEC61360>(dataSpecificationContent.DataType.ToString(), out EnvironmentDataTypeIEC61360 dataType))
                dataType = EnvironmentDataTypeIEC61360.UNDEFINED;

            EnvironmentDataSpecificationIEC61360_V2_0 environmentDataSpecification = new EnvironmentDataSpecificationIEC61360_V2_0()
            {
                DataType = dataType,
                Definition = dataSpecificationContent.Definition,
                PreferredName = dataSpecificationContent.PreferredName,
                ShortName = dataSpecificationContent.ShortName,
                SourceOfDefinition = dataSpecificationContent.SourceOfDefinition,
                Symbol = dataSpecificationContent.Symbol,
                Unit = dataSpecificationContent.Unit,
                UnitId = dataSpecificationContent.UnitId?.ToEnvironmentReference_V2_0(),
                Value = dataSpecificationContent.Value,
                ValueFormat = dataSpecificationContent.ValueFormat,
                ValueId = dataSpecificationContent.ValueId?.ToEnvironmentReference_V2_0(),
                ValueList = dataSpecificationContent.ValueList?.ConvertAll(c => new EnvironmentDataSpecifications.ValueReferencePair()
                {
                    Value = c.Value,
                    ValueId = c.ValueId?.ToEnvironmentReference_V2_0()
                }),
                LevelTypes = dataSpecificationContent.LevelTypes?.ConvertAll(c => (EnvironmentLevelType)Enum.Parse(typeof(EnvironmentLevelType), c.ToString()))
            };

            return environmentDataSpecification;
        }
    }
}
