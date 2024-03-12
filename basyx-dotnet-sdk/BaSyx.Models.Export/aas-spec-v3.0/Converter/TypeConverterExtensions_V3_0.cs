/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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

        public static LangStringSet ToLangStringSet(this List<LangString> langStrings)
        {
            if (langStrings == null || langStrings.Count == 0)
                return new LangStringSet();
            else
                return new LangStringSet(langStrings);
        }
    }
}
