using BaSyx.Models.AdminShell;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Extensions.JsonConverters
{
    public class SubmodelElementContainerPathConverter : JsonConverter<IElementContainer<ISubmodelElement>>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<PathConverter>();
        private PathConverterOptions _converterOptions;

        public SubmodelElementContainerPathConverter(PathConverterOptions options = null)
        {
            _converterOptions = options ?? new PathConverterOptions();
        }
        public override IElementContainer<ISubmodelElement> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IElementContainer<ISubmodelElement> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var element in value.Children) // Children is List of IElementContainers!
            {
                JsonSerializer.Serialize(writer, element.Value, new JsonSerializerOptions()
                {
                    Converters =
                    {
                        new PathConverter(options: new PathConverterOptions()
                        {
                            RequestLevel = _converterOptions.RequestLevel,
                            EncloseInBrackets = _converterOptions.EncloseInBrackets
                        })
                    }
                });
            }

            writer.WriteEndArray();
        }
    }
}
