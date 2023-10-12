using BaSyx.Utils.DependencyInjection.Abstractions;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using BaSyx.Models.AdminShell;
using System.Collections;
using BaSyx.Models.Extensions.SystemTextJson;

namespace BaSyx.Models.Extensions
{
    public class DefaultJsonSerializerOptions
    {
        JsonSerializerOptions _options;

        public DefaultJsonSerializerOptions()
        {
            _options = new JsonSerializerOptions();
            _options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault;
            _options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            _options.Converters.Add(new JsonStringEnumConverter());
            _options.Converters.Add(new DataTypeConverter());
            _options.Converters.Add(new ModelTypeConverter());
            _options.Converters.Add(new ValueScopeConverter());
            _options.Converters.Add(new IdentifierConverter());
            _options.TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { DefaultValueModifier }
            };
        }

        public DefaultJsonSerializerOptions AddDependencyInjection(IDependencyInjectionExtension extension)
        {
            _options.Converters.Add(new TypeConverter(extension));
            return this;
        }
        public DefaultJsonSerializerOptions AddFullSubmodelElementConverter()
        {
            _options.Converters.Add(new FullSubmodelElementConverter());
            return this;
        }

        public DefaultJsonSerializerOptions AddValueOnlySubmodelElementConverter()
        {
            _options.Converters.Add(new ValueOnlyConverter());
            return this;
        }

        public DefaultJsonSerializerOptions AddMetadataSubmodelElementConverter()
        {
            _options.Converters.Add(new SubmodelElementConverter());
            return this;
        }

        public JsonSerializerOptions Build() => _options;


        private static void DefaultValueModifier(JsonTypeInfo info)
        {
            foreach (var property in info.Properties)
            {
                if (DataType.IsGenericList(property.PropertyType))
                {
                    property.ShouldSerialize = (_, val) => val is ICollection collection && collection.Count > 0;
                }
            }
        }       
    }
}
