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
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace BaSyx.Utils.Extensions
{
    public static class StringOperations
    {
        public static Regex DEFAULT_PLACEHOLDER = new Regex("\\${(.*?)}");

        /// <summary>
        /// Replaces the default placeholder ${PLACEHOLDER_NAME} by a replacement function
        /// </summary>
        /// <param name="s">String to replace placeholders</param>
        /// <param name="func">Replacement function</param>
        /// <returns>Replaced string</returns>
        public static string ReplacePlaceholder(this string s, Func<string, string> func)
            => ReplacePlaceholder(s, DEFAULT_PLACEHOLDER, func);

        /// <summary>
        /// Replaces placeholders according to passed Regex by a replacement function
        /// </summary>
        /// <param name="s">String to replace placeholders</param>
        /// <param name="regex">Regex for placeholders</param>
        /// <param name="func">Replacement function</param>
        /// <returns>Replaced string</returns>
        public static string ReplacePlaceholder(this string s, Regex regex, Func<string, string> func)
        {
            MatchCollection matches = regex.Matches(s);
            foreach (Match match in matches)
            {
                string withoutBrackets = match.Groups[1].Value;
                string withBrackets = match.Value;
                string replacement = func.Invoke(withoutBrackets);
                s = s.Replace(withBrackets, replacement);
            }
            return s;
        }

        /// <summary>
        /// Replaces a string that contains placeholder of format ${ENVIRONMENT_VARIABLE} with the respective environment variable value
        /// </summary>
        /// <param name="s">String to replace placeholders</param>
        /// <returns>Replaced string</returns>
        public static string ReplaceWithEnvironmentVariable(this string s) => 
            ReplacePlaceholder(s, DEFAULT_PLACEHOLDER, variable => Environment.GetEnvironmentVariable(variable));
       
        public static string GetValueOrStringEmpty<T>(this T? nullable) where T : struct
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

        public static string ToUpperFirstChar(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string ToLowerFirstChar(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToLower(s[0]) + s.Substring(1);
        }

        public static string RemoveFromEnd(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            else
            {
                return s;
            }
        }

        public static string Base64Encode(this byte[] bytesToEncode)
        {
            return Convert.ToBase64String(bytesToEncode);
        }

        public static string Base64Encode(this string toEncode)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(toEncode);
            return Convert.ToBase64String(bytes);
        }

        public static string Base64Decode(this string toDecode)
        {
            byte[] bytes = Convert.FromBase64String(toDecode);
            return Encoding.UTF8.GetString(bytes);           
        }

        /// <summary>
        /// Encodes a string to a Base64UrlEncoded string (UTF8)
        /// </summary>
        /// <param name="toEncode">String to encode</param>
        /// <returns></returns>
        public static string Base64UrlEncode(this string toEncode)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(toEncode);
            int outputLength = GetMinimalOutputArrayLength(bytes.Length);
            char[] outArray = new char[outputLength];
            int count = Convert.ToBase64CharArray(bytes, 0, bytes.Length, outArray, 0);

            for (var i = 0; i < count; i++)
            {
                var ch = outArray[i];
                if (ch == '+')
                {
                    outArray[i] = '-';
                }
                else if (ch == '/')
                {
                    outArray[i] = '_';
                }
                else if (ch == '=')
                {
                    break;
                }
            }

            string encoded = new string(outArray);
            return encoded;
        }

        private static int GetMinimalOutputArrayLength(int inputLength)
        {
            var blocks = checked(inputLength + 2) / 3;
            return checked(blocks * 4);
        }

        public static byte[] GetBytesFromBase64String(string base64EncodedString)
        {
            byte[] bytes = Convert.FromBase64String(base64EncodedString);
            return bytes;
        }

        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        } 
    }
}
