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
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using System;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Constraints
{
    [DataContract]
    public class Qualifier : Constraint, IQualifier
    {
        public string Type { get; set; }
        public object Value { get; set; }
        public DataType ValueType { get; set; }
        public IReference SemanticId { get; set; }
        public override ModelType ModelType => ModelType.Qualifier;
        public IReference ValueId { get; set; }

        public T ToObject<T>()
        {
            return new ElementValue(Value, ValueType).ToObject<T>();
        }

        public object ToObject(Type type)
        {
            return new ElementValue(Value, ValueType).ToObject(type);
        }

    }
}
