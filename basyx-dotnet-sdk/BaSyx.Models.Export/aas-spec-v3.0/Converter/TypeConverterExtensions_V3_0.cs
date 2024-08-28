/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
using System.Linq;

namespace BaSyx.Models.Export.Converter
{
    public static class TypeConverterExtensions_V3_0
    {       
        public static Reference ToReference_V3_0(this EnvironmentReference_V3_0 environmentReference)
        {
            if (environmentReference == null)
                return null;

            if (environmentReference?.Keys?.Count > 0)
                return new Reference(environmentReference.Keys.ConvertAll(c => c.ToKey()).ToArray()) { Type = environmentReference.Type };
            return null;
        }

        public static Reference<T> ToReference_V3_0<T>(this EnvironmentReference_V3_0 environmentReference) where T : IReferable
        {
            if (environmentReference == null)
                return null;

            if (environmentReference?.Keys?.Count > 0)
                return new Reference<T>(environmentReference.Keys.ConvertAll(c => c.ToKey()).ToArray()) { Type = environmentReference.Type };
            return null;
        }

        public static EnvironmentReference_V3_0 ToEnvironmentReference_V3_0(this IIdentifiable identifiable)
        {
            if (identifiable.Id == null)
                return null;

            KeyElements_V3_0 type;

            if (identifiable is IAssetAdministrationShell)
                type = KeyElements_V3_0.AssetAdministrationShell;
            else if (identifiable is IConceptDescription)
                type = KeyElements_V3_0.ConceptDescription;
            else if (identifiable is ISubmodel)
                type = KeyElements_V3_0.Submodel;
            else
                return null;

            EnvironmentReference_V3_0 reference = new EnvironmentReference_V3_0()
            {
                Type = ReferenceType.ModelReference,
                Keys = new List<EnvironmentKey_V3_0>()
                {
                    new EnvironmentKey_V3_0()
                    {
                        Value = identifiable.Id,
                        Type = type
                    }
                }
            };
            return reference;
        }

        public static EnvironmentReference_V3_0 ToEnvironmentReference_V3_0(this IReference reference)
        {
            if (reference == null)
                return null;

            if (reference?.Keys?.Count() > 0)
                return new EnvironmentReference_V3_0()
                {
                    Type = reference.Type == ReferenceType.Undefined ? ReferenceType.ExternalReference : reference.Type,
                    Keys = reference.Keys.ToList().ConvertAll(c => c.ToEnvironmentKey()).ToList()
                };
            return null;
        }

        public static Key ToKey(this EnvironmentKey_V3_0 environmentKey)
        {
            if (environmentKey == null)
                return null;

            if (!Enum.TryParse<KeyType>(environmentKey.Type.ToString(), out KeyType type))
                type = KeyType.Undefined;
            
            Key key = new Key(
                type,
                environmentKey.Value);

            return key;
        }

        public static EnvironmentKey_V3_0 ToEnvironmentKey(this IKey key)
        {
            if (key == null)
                return null;

            if (!Enum.TryParse(key.Type.ToString(), out KeyElements_V3_0 type))
                type = KeyElements_V3_0.Undefined;

            EnvironmentKey_V3_0 envKey = new EnvironmentKey_V3_0()
            {
                Type = type,
                Value = key.Value,
            };
            return envKey;
        }

        public static LangStringSet ToLangStringSet(this List<LangString> langStrings)
        {
            if (langStrings == null || langStrings.Count == 0)
                return new LangStringSet();
            else
                return new LangStringSet(langStrings);
        }

        public static List<EnvironmentLangString_V3_0> ToEnvironmentLangStringSet(this List<LangString> langStrings)
        {
            if (langStrings == null || langStrings.Count == 0)
                return new List<EnvironmentLangString_V3_0>();
            else
                return langStrings.ConvertAll(l => new EnvironmentLangString_V3_0()
                {
                    Text = l.Text,
                    Language = l.Language,
                });
        }
    }
}
