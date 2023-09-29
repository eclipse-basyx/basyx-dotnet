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
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace BaSyx.Utils.Json
{
    public static class StreamingContextExtensions
    {
        public static StreamingContext AddData(this StreamingContext context, string key, object value)
        {
            IStreamingContextDataDictionary dictionary;
            if (context.Context == null)
                dictionary = new StreamingContextDataDictionary();
            else if (context.Context is IStreamingContextDataDictionary d)
                dictionary = d;
            else
                return context;

            dictionary.AddData(key, value);
            return new StreamingContext(context.State, dictionary);
        }

        public static bool TryGetData(this StreamingContext context, string key, out object value)
        {
            IStreamingContextDataDictionary dictionary = context.Context as IStreamingContextDataDictionary;
            if (dictionary == null)
            {
                value = null;
                return false;
            }
            return dictionary.TryGetData(key, out value);
        }
    }

    public interface IStreamingContextDataDictionary
    {
        void AddData(string key, object value);
        bool TryGetData(string key, out object value);
    }

    public class StreamingContextDataDictionary : IStreamingContextDataDictionary
    {
        private readonly ConcurrentDictionary<string, object> dataDictionary = new ConcurrentDictionary<string, object>();
        public void AddData(string key, object value) => dataDictionary.TryAdd(key, value);
        public bool TryGetData(string key, out object value) => dataDictionary.TryGetValue(key, out value);
    }
}
