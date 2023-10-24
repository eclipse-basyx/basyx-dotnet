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

            var content = new DataSpecificationIEC61360Content()
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
                ValueList = new ValueList()
                {
                    ValueReferencePairs = environmentDataSpecification.ValueList?.ConvertAll(c => new Semantics.ValueReferencePair()
                    {
                        Value = c.Value,
                        ValueId = c.ValueId?.ToReference_V2_0()
                    })
                }
            };
          
            LevelType levelType = new LevelType();
            foreach (var envLevelType in environmentDataSpecification.LevelTypes)
            {                
                if (envLevelType == EnvironmentLevelType.Min)
                    levelType.Min = true;
                if (envLevelType == EnvironmentLevelType.Max)
                    levelType.Max = true;
                if (envLevelType == EnvironmentLevelType.Nom)
                    levelType.Nom = true;
                if (envLevelType == EnvironmentLevelType.Typ)
                    levelType.Typ = true;
            }
            content.LevelType = levelType;

            DataSpecificationIEC61360 dataSpecification = new DataSpecificationIEC61360(content);
            return dataSpecification;
        }    
    }
}
