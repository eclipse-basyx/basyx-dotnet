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
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentConstraint_V2_0
    {
        [XmlElement(ElementName = "qualifier", Type = typeof(EnvironmentQualifier_V2_0))]
        [XmlElement(ElementName = "formula", Type = typeof(EnvironmentFormula_V2_0))]
        public ConstraintType_V2_0 Constraint;
    }

    public class ConstraintType_V2_0 : IModelType
    {
        [XmlIgnore]
        public virtual ModelType ModelType { get; }
    }
}