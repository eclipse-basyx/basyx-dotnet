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
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace BaSyx.Utils.ResultHandling
{
    public interface IFilterArgument
    {
        FilterArgumentType FilterArgumentType { get; }
        FilterType FilterType { get; set; }
    }

    public sealed class KeyValue : IFilterArgument
    {
        public FilterType FilterType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public FilterArgumentType FilterArgumentType => FilterArgumentType.KeyValue;
    }

    public class FilterExpression : IFilterArgument
    {
        public FilterType FilterType { get; set; }
        public List<IFilterArgument> Arguments { get; set; }
        public FilterArgumentType FilterArgumentType => FilterArgumentType.FilterExpression;

        public static FilterExpression Parse(JObject query)
        {
            if (query != null)
            {
                FilterExpression filterExpression = new FilterExpression();
                filterExpression.FilterType = FilterType.Parse(query.SelectToken("name").Value<string>());
                var argsArray = (JArray)query.SelectToken("args");
                if (argsArray.HasValues)
                {
                    filterExpression.Arguments = new List<IFilterArgument>();
                    FilterExpression subFilter = new FilterExpression();
                    subFilter.Arguments = new List<IFilterArgument>();  
                    foreach (var item in argsArray)
                    {
                        subFilter.FilterType = FilterType.Parse(item.SelectToken("name").Value<string>());
                        if (subFilter.FilterType == FilterType.AND || subFilter.FilterType == FilterType.OR)
                        {
                            var argObj = Parse((JObject)item);
                            subFilter.Arguments.Add(argObj);
                        }
                        else
                        {
                            var keyValueArg = new KeyValue();
                            keyValueArg.FilterType = FilterType.Parse(item.SelectToken("name").Value<string>());
                            keyValueArg.Key = item.SelectToken("args[0]").ToString();
                            keyValueArg.Value = item.SelectToken("args[1]").ToString();

                            subFilter.Arguments.Add(keyValueArg);
                        }
                    }
                    filterExpression.Arguments.Add(subFilter);
                }
                return filterExpression;
            }
            return null;
        }
    }

   

    public enum FilterArgumentType : int
    {
        KeyValue = 0,
        FilterExpression = 1
    }

    public sealed class FilterType
    {
        private readonly string Name;
        private readonly int Value;

        public static readonly FilterType EQUALS = new FilterType(1, "eq");
        public static readonly FilterType AND = new FilterType(2, "and");
        public static readonly FilterType OR = new FilterType(3, "or");

        private FilterType(int value, string name)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return Name;
        }

        public static FilterType Parse(string s)
        {
            switch (s.ToLower())
            {
                case "eq": return EQUALS;
                case "and": return AND;
                case "or": return OR;
                default:
                    break;
            }
            return null;
        }

    }
}
