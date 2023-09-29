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
    public static class TypeConverterExtensions_V2_0
    {
        public static EnvironmentReference_V2_0 ToEnvironmentReference_V2_0(this IReference reference)
        {
            if (reference?.Keys?.Count == 0)
                return null;

            List<EnvironmentKey_V2_0> keys = reference?.Keys?.ConvertAll(c => c.ToEnvironmentKey_V2_0());
            if (keys?.Count > 0)
                return new EnvironmentReference_V2_0()
                {
                    Keys = keys
                };
            return null;
        }

        public static Reference ToReference_V2_0(this EnvironmentReference_V2_0 environmentReference)
        {
            if (environmentReference == null)
                return null;

            if (environmentReference?.Keys?.Count > 0)
                return new Reference(environmentReference.Keys.ConvertAll(c => c.ToKey()).ToArray());
            return null;
        }

        public static Reference<T> ToReference_V2_0<T>(this EnvironmentReference_V2_0 environmentReference) where T : IReferable
        {
            if (environmentReference == null)
                return null;

            if (environmentReference?.Keys?.Count > 0)
                return new Reference<T>(environmentReference.Keys.ConvertAll(c => c.ToKey()).ToArray());
            return null;
        }

        public static Key ToKey(this EnvironmentKey_V2_0 environmentKey)
        {
            if (environmentKey == null)
                return null;

            if (!Enum.TryParse<KeyElements>(environmentKey.Type.ToString(), out KeyElements type))
                type = KeyElements.Undefined;
            
            if (!Enum.TryParse<KeyType>(environmentKey.IdType.ToString(), out KeyType idType))
                idType = KeyType.Undefined;

            Key key = new Key(
                type,
                idType,
                environmentKey.Value,
                environmentKey.Local);

            return key;
        }

        public static EnvironmentKey_V2_0 ToEnvironmentKey_V2_0(this IKey key)
        {
            if (key == null)
                return null;

            EnvironmentKey_V2_0 environmentKey = new EnvironmentKey_V2_0();
            if(!Enum.TryParse<KeyType_V2_0>(key.IdType.ToString(), out KeyType_V2_0 keyType))
            {
                if (key.IdType == KeyType.URI)
                    keyType = KeyType_V2_0.IRI;
            }
            if (!Enum.TryParse<KeyElements_V2_0>(key.Type.ToString(), out KeyElements_V2_0 type))
            {
                type = KeyElements_V2_0.Undefined;
            }


            environmentKey.IdType = keyType;
            environmentKey.Type = type;
            environmentKey.Local = key.Local;
            environmentKey.Value = key.Value;

            return environmentKey;
        }

        public static EnvironmentReference_V2_0 ToEnvironmentReference_V2_0(this IIdentifiable identifiable)
        {
            if (identifiable.Identification == null)
                return null;

            KeyElements_V2_0 type;

            if (identifiable is IAsset)
                type = KeyElements_V2_0.Asset;
            else if (identifiable is IAssetAdministrationShell)
                type = KeyElements_V2_0.AssetAdministrationShell;
            else if (identifiable is IConceptDescription)
                type = KeyElements_V2_0.ConceptDescription;
            else if (identifiable is ISubmodel)
                type = KeyElements_V2_0.Submodel;
            else
                return null;

            if(!Enum.TryParse<KeyType_V2_0>(identifiable.Identification.IdType.ToString(), out KeyType_V2_0 idType))
            {
                if (identifiable.Identification.IdType == KeyType.URI)
                    idType = KeyType_V2_0.IRI;
            }

            EnvironmentReference_V2_0 reference = new EnvironmentReference_V2_0()
            {
                Keys = new List<EnvironmentKey_V2_0>()
                {
                    new EnvironmentKey_V2_0()
                    {
                        IdType = idType,
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
