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
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class FileElement : SubmodelElement, IFileElement
    {
        public override ModelType ModelType => ModelType.File;
        public string ContentType { get; set; }
        public string Value { get; set; }
        public FileElement(string idShort) : base(idShort)
        {
            Get = element => { return new ElementValue(Value, new DataType(DataObjectType.String)); };
            Set = (element, value) => { Value = value.Value as string; return Task.CompletedTask; };
        }
    }
}
