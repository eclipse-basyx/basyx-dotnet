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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Extensions
{
    public class PathConverterOptions : ConverterOptions
    {
        public bool EncloseInBrackets { get; set; } = true;
    }

    public class PathConverter : JsonConverter<ISubmodelElement>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<PathConverter>();
        private PathConverterOptions _converterOptions;

        public PathConverter(PathConverterOptions options = null)
        {
            _converterOptions = options ?? new PathConverterOptions();
        }

        public override ISubmodelElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ISubmodelElement value, JsonSerializerOptions options)
        {
            // TODO: do not write StartArray if it is calles from COntainerConverter ( = get paths from submodel)
            if (_converterOptions.EncloseInBrackets)
                writer.WriteStartArray();
            
            if (value is IElementContainer<ISubmodelElement> rootContainer)
            {
                writer.WriteStringValue(rootContainer.Path);

                var childPaths = new List<string>();
                if (_converterOptions.RequestLevel == RequestLevel.Core)
                {
                    foreach (var directChild in rootContainer.Children)
                        childPaths.Add(directChild.Path);
                }
                else
                    childPaths = new List<string>(GetChildPathsRecursively(rootContainer, new List<string>()));

                foreach (var path in childPaths)
                    writer.WriteStringValue(path);
            }
            else
                writer.WriteStringValue(value.IdShort);

            if (_converterOptions.EncloseInBrackets)
                writer.WriteEndArray();
        }

        /// <summary>
        /// Iterate recursively through the children and append their path to the list.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childPaths"></param>
        /// <returns></returns>
        private List<string> GetChildPathsRecursively(IElementContainer<ISubmodelElement> parent, List<string> childPaths)
        {
            foreach (var child in parent.Children)
            {
                if (child.HasChildren())
                {
                    var childContainer = child as IElementContainer<ISubmodelElement>;
                    childPaths.Add(childContainer.Path);
                    GetChildPathsRecursively(childContainer, childPaths);
                }
                else
                    childPaths.Add(child.Path);
            }

            return childPaths;
        }

    }  
}
