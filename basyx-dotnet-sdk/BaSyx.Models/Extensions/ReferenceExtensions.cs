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
using System.Linq;

namespace BaSyx.Models.Extensions
{
    public static class ReferenceFactory
    {
        private static List<Key> CreateKeys(string idShortPath, KeyType keyType)
        {
            List<Key> keys = new List<Key>();
            if (idShortPath.Contains("."))
            {
                string[] splitted = idShortPath.Split('.');
                for (int i = 0; i < splitted.Length; i++)
                {
                    Key key;
                    if (i + 1 == splitted.Length)
                    {
                        key = new Key(keyType, splitted[i]);
                    }
                    else
                    {
                        key = new Key(KeyType.SubmodelElementCollection, splitted[i]);
                    }
                    keys.Add(key);
                }
            }
            else
                keys.Add(new Key(keyType, idShortPath));
            return keys;
        }

        public static IReference Create(string idShortPath, ModelType modelType)
        {
            KeyType keyType = Key.GetKeyElementFromModelType(modelType);
            List<Key> keys = CreateKeys(idShortPath, keyType);
            return new Reference(keys) { Type = ReferenceType.ModelReference };
        }

        public static IReference Create(string submodelId, string idShortPath, ModelType modelType)
        {
            KeyType keyType = Key.GetKeyElementFromModelType(modelType);
            List<Key> keys = CreateKeys(idShortPath, keyType);
            Key submodelKey = new Key(KeyType.Submodel, submodelId);
            var newKeys = keys.Prepend(submodelKey);
            return new Reference(newKeys) { Type = ReferenceType.ModelReference };
        }
    }
    public static class ReferenceExtensions
    {
        public const string eClass_ICD = "0173";
        public const string ISO_ICD = "0112";
        public const string GTIN_ICD = "0160";

        public static IReference<T> CreateReference<T>(this T referable) where T : class, IReferable
        {
            IReference<T> reference = new Reference<T>(referable);
            return reference;
        }

        public static bool IsEClassReference(this IReference reference)
        {
            if (reference == null || reference.First == null || string.IsNullOrEmpty(reference.First.Value))
                return false;

            if (reference.First.Value.StartsWith(eClass_ICD))
                return true;

            return false;
        }

        public static bool IsIsoReference(this IReference reference)
        {
            if (reference == null || reference.First == null || string.IsNullOrEmpty(reference.First.Value))
                return false;

            if (reference.First.Value.StartsWith(ISO_ICD))
                return true;

            return false;
        }

        public static bool IsGtinReference(this IReference reference)
        {
            if (reference == null || reference.First == null || string.IsNullOrEmpty(reference.First.Value))
                return false;

            if (reference.First.Value.StartsWith(GTIN_ICD))
                return true;

            return false;
        }

        public static string GetLastIdShort(this IReference reference)
        {
            return reference?.Keys?.LastOrDefault()?.Value;
        }
    }
}
