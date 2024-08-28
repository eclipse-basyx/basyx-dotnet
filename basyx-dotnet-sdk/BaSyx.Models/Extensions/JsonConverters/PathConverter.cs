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
using BaSyx.Utils.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using Range = BaSyx.Models.AdminShell.Range;

namespace BaSyx.Models.Extensions
{
    public class PathConverter : JsonConverter<ISubmodelElement>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<PathConverter>();
        private readonly RequestLevel _level;

        public PathConverter(RequestLevel level = default)
        {
            _level = level;
        }
        public override ISubmodelElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ISubmodelElement value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            if (value is IElementContainer<ISubmodelElement>)
            {
                var rootContainer = value as IElementContainer<ISubmodelElement>;
                writer.WriteStringValue(rootContainer.Path);

                var childPaths = new List<string>();
                if (_level == RequestLevel.Core)
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
            {
                // TODO: Return full IdShortPath
                if (_level == RequestLevel.Deep)
                    writer.WriteStringValue(value.IdShort);
            }

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
