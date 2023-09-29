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

using BaSyx.Models.AdminShell;
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