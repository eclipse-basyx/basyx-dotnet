/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using System;

namespace BaSyx.Utils.StringOperations
{
    public static class StringOperations
    {
        public static string GetValueOrStringEmpty<T>(T? nullable) where T : struct
        {
            if (nullable != null)
            {
                var value = Nullable.GetUnderlyingType(nullable.GetType());
                if (value != null && value.IsEnum)
                    Enum.GetName(Nullable.GetUnderlyingType(nullable.GetType()), nullable.Value);
                else
                    return nullable.Value.ToString();
            }
            return string.Empty;
        }

        public static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string LowercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToLower(s[0]) + s.Substring(1);
        }

    }
}
