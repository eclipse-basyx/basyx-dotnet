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

namespace BaSyx.Models.Extensions
{
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
            if (reference == null || reference.First == null || string.IsNullOrEmpty(reference.First.Value) || reference.First.IdType != KeyType.IRDI)
                return false;

            if (reference.First.Value.StartsWith(eClass_ICD))
                return true;

            return false;
        }

        public static bool IsIsoReference(this IReference reference)
        {
            if (reference == null || reference.First == null || string.IsNullOrEmpty(reference.First.Value) || reference.First.IdType != KeyType.IRDI)
                return false;

            if (reference.First.Value.StartsWith(ISO_ICD))
                return true;

            return false;
        }

        public static bool IsGtinReference(this IReference reference)
        {
            if (reference == null || reference.First == null || string.IsNullOrEmpty(reference.First.Value) || reference.First.IdType != KeyType.IRDI)
                return false;

            if (reference.First.Value.StartsWith(GTIN_ICD))
                return true;

            return false;
        }
    }
}
