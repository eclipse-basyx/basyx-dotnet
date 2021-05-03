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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.StringOperations;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract]
    public class Blob : SubmodelElement, IBlob
    {
        public override ModelType ModelType => ModelType.Blob;
        public string MimeType { get; set; }
        public string Value { get; private set; }

        public Blob(string idShort) : base(idShort) 
        {
            Get = element => { return new ElementValue(Value, new DataType(DataObjectType.Base64Binary)); };
            Set = (element, value) => { SetValue(value.Value as string); };
        }

        public void SetValue(byte[] bytes)
        {
            Value = StringOperations.Base64Encode(bytes);
        }

        public void SetValue(string value)
        {
            if (StringOperations.IsBase64String(value))
                Value = value;
            else
                Value = StringOperations.Base64Encode(value);
        }

        public byte[] GetBytes()
        {
            return StringOperations.GetBytes(Value);
        }
    }
}
