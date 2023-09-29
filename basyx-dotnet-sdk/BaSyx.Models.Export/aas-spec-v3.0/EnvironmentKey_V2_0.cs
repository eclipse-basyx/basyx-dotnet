using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    [XmlType("key")]
    public class EnvironmentKey_V2_0
    {
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlAttribute("type")]
        public KeyElements_V2_0 Type { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "idType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlAttribute("idType")]
        public KeyType_V2_0 IdType { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "value")]
        [XmlText]
        public string Value { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include, PropertyName = "local")]
        [XmlAttribute("local")]
        public bool Local { get; set; }
    }

    public enum KeyType_V2_0
    {
        [EnumMember(Value = "Undefined")]
        Undefined,
        [EnumMember(Value = "Custom")]
        Custom,
        [EnumMember(Value = "IRI")]
        IRI,
        [EnumMember(Value = "IRDI")]
        IRDI,
        [EnumMember(Value = "IdShort")]
        IdShort,
        [EnumMember(Value = "FragmentId")]
        FragmentId
    }

    public enum KeyElements_V2_0
    {
        Undefined,

        GlobalReference,
        FragmentReference,

        AccessPermissionRule,
        AnnotatedRelationshipElement,
        BasicEvent,
        Blob,
        Capability,
        ConceptDictionary,
        DataElement,
        File,
        Entity,
        Event,
        MultiLanguageProperty,
        Operation,
        Property,
        Range,
        ReferenceElement,
        RelationshipElement,
        SubmodelElement,
        SubmodelElementCollection,
        View,

        Asset,
        AssetAdministrationShell,
        ConceptDescription,
        Submodel

    }
}
