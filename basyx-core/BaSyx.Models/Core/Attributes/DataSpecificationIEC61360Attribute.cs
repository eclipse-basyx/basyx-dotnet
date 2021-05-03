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
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Extensions.Semantics.DataSpecifications;
using System;

namespace BaSyx.Models.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class DataSpecificationIEC61360Attribute : Attribute
    {
        public Identifier Identification { get; }
        public DataSpecificationIEC61360Content Content { get; }

        public string PreferredName_DE { get => Content.PreferredName["de"]; set => Content.PreferredName.AddLangString("de", value); }
        public string PreferredName_EN { get => Content.PreferredName["en"]; set => Content.PreferredName.AddLangString("en", value); }

        public string Definition_DE { get => Content.Definition["de"]; set => Content.Definition.AddLangString("de", value); }
        public string Definition_EN { get => Content.Definition["en"]; set => Content.Definition.AddLangString("en", value); }

        public string ShortName_DE { get => Content.ShortName["de"]; set => Content.ShortName.AddLangString("de", value); }
        public string ShortName_EN { get => Content.ShortName["en"]; set => Content.ShortName.AddLangString("en", value); }

        public DataTypeIEC61360 DataType { get => Content.DataType; set => Content.DataType = value; }

        public string SourceOfDefinition { get => Content.SourceOfDefinition; set => Content.SourceOfDefinition = value; }

        public string Symbol { get => Content.Symbol; set => Content.Symbol = value; }

        public string Unit { get => Content.Unit; set => Content.Unit = value; }

        public KeyType UnitIdKeyType { get; set; }

        public string UnitId { 
            get => Content.UnitId.ToStandardizedString(); 
            set => Content.UnitId = new Reference(new GlobalKey(KeyElements.GlobalReference, UnitIdKeyType, value)); }

        public string ValueFormat { get => Content.ValueFormat; set => Content.ValueFormat = value; }

        public object Value { get => Content.Value; set => Content.Value = value; }

        public KeyType ValueIdKeyType { get; set; }

        public string ValueId
        {
            get => Content.ValueId.ToStandardizedString();
            set => Content.ValueId = new Reference(new GlobalKey(KeyElements.GlobalReference, ValueIdKeyType, value));
        }

        public DataSpecificationIEC61360Attribute(string id, KeyType idType)
        {
            Identification = new Identifier(id, idType);
            Content = new DataSpecificationIEC61360Content();
        }
    }
}
