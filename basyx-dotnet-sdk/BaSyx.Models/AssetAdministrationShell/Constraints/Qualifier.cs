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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class Qualifier : IQualifier
    {
        public QualifierKind Kind { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
        public IReference ValueId { get; set; }
        public DataType ValueType { get; set; }
        public IReference SemanticId { get; set; }
        public IEnumerable<IReference> SupplementalSemanticIds { get; set; }
        public ModelType ModelType => ModelType.Qualifier;

        public Qualifier()
        {
            SupplementalSemanticIds = new List<IReference>();
        }
    }
}
