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
using System.Collections.Generic;
using System;

namespace BaSyx.Models.Export.Converter
{
    public static class TypeConverterExtensions_V1_0
    {
        public static EnvironmentReference_V1_0 ToEnvironmentReference_V1_0(this IReference reference)
        {
            if (reference?.Keys?.Count == 0)
                return null;

            List<EnvironmentKey_V1_0> keys = reference?.Keys?.ConvertAll(c => c.ToEnvironmentKey_V1_0());
            if (keys?.Count > 0)
                return new EnvironmentReference_V1_0()
                {
                    Keys = keys
                };
            return null;
        }

        public static Reference ToReference_V1_0(this EnvironmentReference_V1_0 environmentReference)
        {
            if (environmentReference == null)
                return null;

            if (environmentReference?.Keys?.Count > 0)
                return new Reference(environmentReference.Keys.ConvertAll(c => c.ToKey()).ToArray());
            return null;
        }

        public static Reference<T> ToReference_V1_0<T>(this EnvironmentReference_V1_0 environmentReference) where T : IReferable
        {
            if (environmentReference == null)
                return null;

            if (environmentReference?.Keys?.Count > 0)
                return new Reference<T>(environmentReference.Keys.ConvertAll(c => c.ToKey()).ToArray());
            return null;
        }

        public static Key ToKey(this EnvironmentKey_V1_0 environmentKey)
        {
            if (environmentKey == null)
                return null;

            Key key = new Key(
                (KeyElements)Enum.Parse(typeof(KeyElements), environmentKey.Type.ToString()),
                (KeyType)Enum.Parse(typeof(KeyType), environmentKey.IdType.ToString()),
                environmentKey.Value,
                environmentKey.Local);

            return key;
        }

        public static EnvironmentKey_V1_0 ToEnvironmentKey_V1_0(this IKey key)
        {
            if (key == null)
                return null;

            EnvironmentKey_V1_0 environmentKey = new EnvironmentKey_V1_0();
            environmentKey.IdType = (KeyType_V1_0)Enum.Parse(typeof(KeyType_V1_0), key.IdType.ToString());
            environmentKey.Type = (KeyElements_V1_0)Enum.Parse(typeof(KeyElements_V1_0), key.Type.ToString());
            environmentKey.Local = key.Local;
            environmentKey.Value = key.Value;

            return environmentKey;
        }

        public static EnvironmentReference_V1_0 ToEnvironmentReference_V1_0(this IIdentifiable identifiable)
        {
            if (identifiable.Identification == null)
                return null;

            KeyElements_V1_0 type;

            if (identifiable is IAsset)
                type = KeyElements_V1_0.Asset;
            else if (identifiable is IAssetAdministrationShell)
                type = KeyElements_V1_0.AssetAdministrationShell;
            else if (identifiable is IConceptDescription)
                type = KeyElements_V1_0.ConceptDescription;
            else if (identifiable is ISubmodel)
                type = KeyElements_V1_0.Submodel;
            else
                return null;

            EnvironmentReference_V1_0 reference = new EnvironmentReference_V1_0()
            {
                Keys = new List<EnvironmentKey_V1_0>()
                {
                    new EnvironmentKey_V1_0()
                    {
                        IdType = (KeyType_V1_0)Enum.Parse(typeof(KeyType_V1_0), identifiable.Identification.IdType.ToString()),
                        Local = true,
                        Value = identifiable.Identification.Id,
                        Type = type
                    }
                }
            };
            return reference;
        }        
    }
}
