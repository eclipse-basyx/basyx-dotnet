using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    [XmlType("key")]
    public class EnvironmentKey_V1_0
    {
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute("type")]
        public KeyElements_V1_0 Type { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute("idType")]
        public KeyType_V1_0 IdType { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlText]
        public string Value { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute("local")]
        public bool Local { get; set; }
    }

    public enum KeyType_V1_0
    {
        [EnumMember(Value = "Custom")]
        Custom,
        [EnumMember(Value = "URI")]
        URI,
        [EnumMember(Value = "IRDI")]
        IRDI,
        [EnumMember(Value = "IdShort")]
        IdShort,
        [EnumMember(Value = "FragmentId")]
        FragmentId
    }

    public enum KeyElements_V1_0
    {
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
