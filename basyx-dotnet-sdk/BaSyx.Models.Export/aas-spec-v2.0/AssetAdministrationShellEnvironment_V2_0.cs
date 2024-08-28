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
using BaSyx.Models.Export.Converter;
using BaSyx.Models.Export.EnvironmentSubmodelElements;
using BaSyx.Models.Semantics;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace BaSyx.Models.Export
{
    [DataContract]
    [XmlType(AnonymousType = true, Namespace = AAS_NAMESPACE)]
    [XmlRoot(ElementName = "aasenv", Namespace = AAS_NAMESPACE, IsNullable = false)]
    public class AssetAdministrationShellEnvironment_V2_0
    {
        public const string AAS_NAMESPACE = "http://www.admin-shell.io/aas/2/0";
        public const string IEC61360_NAMESPACE = "http://www.admin-shell.io/IEC61360/2/0";
        public const string ABAC_NAMESPACE = "http://www.admin-shell.io/aas/abac/2/0";
        public const string AAS_XSD_FILENAME = "AAS.xsd";
        public const string IEC61360_XSD_FILENAME = "IEC61360.xsd";
        public const string ABAC_XSD_FILENAME = "AAS_ABAC.xsd";

        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "assetAdministrationShells", Order = 0)]
        [XmlIgnore, JsonIgnore]
        public List<IAssetAdministrationShell> AssetAdministrationShells { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "assets", Order = 1)]
        [XmlIgnore, JsonIgnore]
        public List<IAssetInformation> Assets { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "submodels", Order = 2)]
        [XmlIgnore, JsonIgnore]
        public List<ISubmodel> Submodels { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "conceptDescriptions", Order = 3)]
        [XmlIgnore, JsonIgnore]
        public List<IConceptDescription> ConceptDescriptions { get; }

        [JsonProperty("assetAdministrationShells")]
        [XmlArray("assetAdministrationShells")]
        [XmlArrayItem("assetAdministrationShell")]
        public List<EnvironmentAssetAdministrationShell_V2_0> EnvironmentAssetAdministrationShells { get; set; }

        [JsonProperty("assets")]
        [XmlArray("assets")]
        [XmlArrayItem("asset")]
        public List<EnvironmentAsset_V2_0> EnvironmentAssets { get; set; }

        [JsonProperty("submodels")]
        [XmlArray("submodels")]
        [XmlArrayItem("submodel")]
        public List<EnvironmentSubmodel_V2_0> EnvironmentSubmodels { get; set; }

        [JsonProperty("conceptDescriptions")]
        [XmlArray("conceptDescriptions")]
        [XmlArrayItem("conceptDescription")]
        public List<EnvironmentConceptDescription_V2_0> EnvironmentConceptDescriptions { get; set; }

        [IgnoreDataMember]
        [XmlIgnore]
        public Dictionary<string, IFileElement> SupplementalFiles;

        private string ContentRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static readonly ILogger logger = LoggingExtentions.CreateLogger<AssetAdministrationShellEnvironment_V2_0>();
        private static readonly ManifestEmbeddedFileProvider fileProvider;

        public static JsonSerializerSettings JsonSettings;
        public static XmlReaderSettings XmlSettings;

        private static Action<ValidationEventArgs> _userValidationCallback;

        static AssetAdministrationShellEnvironment_V2_0()
        {
            JsonSettings = new JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Ignore,                
            };
            JsonSettings.Converters.Add(new StringEnumConverter());

            fileProvider = new ManifestEmbeddedFileProvider(typeof(AssetAdministrationShellEnvironment_V2_0).Assembly, Path.Combine("aas-spec-v2.0", "Resources"));

            XmlSettings = new XmlReaderSettings();
            XmlSettings.ValidationType = ValidationType.Schema;
            XmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            XmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            XmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            XmlSettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);

            using (var stream = fileProvider.GetFileInfo(AAS_XSD_FILENAME).CreateReadStream())
                XmlSettings.Schemas.Add(AAS_NAMESPACE, XmlReader.Create(stream));

            using (var stream = fileProvider.GetFileInfo(IEC61360_XSD_FILENAME).CreateReadStream())
                XmlSettings.Schemas.Add(IEC61360_NAMESPACE, XmlReader.Create(stream));

            using (var stream = fileProvider.GetFileInfo(ABAC_XSD_FILENAME).CreateReadStream())
                XmlSettings.Schemas.Add(ABAC_NAMESPACE, XmlReader.Create(stream));
        }

        [JsonConstructor]
        public AssetAdministrationShellEnvironment_V2_0()
        {
            AssetAdministrationShells = new List<IAssetAdministrationShell>();
            Submodels = new List<ISubmodel>();
            Assets = new List<IAssetInformation>();
            ConceptDescriptions = new List<IConceptDescription>();
            SupplementalFiles = new Dictionary<string, IFileElement>();

            EnvironmentAssetAdministrationShells = new List<EnvironmentAssetAdministrationShell_V2_0>();
            EnvironmentAssets = new List<EnvironmentAsset_V2_0>();
            EnvironmentSubmodels = new List<EnvironmentSubmodel_V2_0>();
            EnvironmentConceptDescriptions = new List<EnvironmentConceptDescription_V2_0>();
        }   
     
        private void ExtractSupplementalFiles(IElementContainer<ISubmodelElement> submodelElements)
        {
            foreach (var smElement in submodelElements)
            {
                if (smElement is FileElement file)
                {
                    string filePath = ContentRoot + file.Value.Value.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                    if (File.Exists(filePath))
                    {
                        string destinationPath = file.Value.Value;
                        if (!destinationPath.StartsWith(AASX_V2_0.AASX_FOLDER))
                            destinationPath = AASX_V2_0.AASX_FOLDER + destinationPath;

                        file.Value.Value = destinationPath;
                        SupplementalFiles.Add(filePath, file);
                    }
                }
                else if (smElement.ModelType == ModelType.SubmodelElementCollection)
                    ExtractSupplementalFiles((smElement as SubmodelElementCollection).Value.Value);
            }
        }

        public static AssetAdministrationShellEnvironment_V2_0 ReadEnvironment_V2_0(Stream stream, ExportType exportType)
        {
            AssetAdministrationShellEnvironment_V2_0 env = null;

            try
            {
                switch (exportType)
                {
                    case ExportType.Xml:
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(AssetAdministrationShellEnvironment_V2_0), AAS_NAMESPACE);

                            using (XmlReader reader = XmlReader.Create(stream, XmlSettings))
                                env = (AssetAdministrationShellEnvironment_V2_0)serializer.Deserialize(reader);
                        }
                        break;
                    case ExportType.Json:
                        {
                            using (StreamReader reader = new StreamReader(stream))
                                env = JsonConvert.DeserializeObject<AssetAdministrationShellEnvironment_V2_0>(reader.ReadToEnd(), JsonSettings);
                        }
                        break;
                    default:
                        throw new InvalidOperationException(exportType + " not supported");
                }

                ConvertToAssetAdministrationShell(env);
                return env;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to read environment");
                return null;
            }
        }


        public static AssetAdministrationShellEnvironment_V2_0 ReadEnvironment_V2_0(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(filePath);
            if (!System.IO.File.Exists(filePath))
                throw new ArgumentException(filePath + " does not exist");

            AssetAdministrationShellEnvironment_V2_0 env = null;

            string fileExtension = Path.GetExtension(filePath);
            ExportType exportType;
            switch (fileExtension)
            {
                case ".xml":
                    exportType = ExportType.Xml;
                    break;
                case ".json":
                    exportType = ExportType.Json;
                    break;
                default:
                    throw new InvalidOperationException(fileExtension + " not supported");
            }

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                env = ReadEnvironment_V2_0(file, exportType);

            return env;
        }

        private static void ConvertToAssetAdministrationShell(AssetAdministrationShellEnvironment_V2_0 environment)
        {
            foreach (var envAsset in environment.EnvironmentAssets)
            {
                AssetInformation asset = new AssetInformation()
                {
                    GlobalAssetId = envAsset.Identification.Id,
                    AssetKind = envAsset.Kind
                };
                environment.Assets.Add(asset);
            }
            foreach (var envConceptDescription in environment.EnvironmentConceptDescriptions)
            {
                ConceptDescription conceptDescription = new ConceptDescription()
                {
                    Administration = new AdministrativeInformation()
                    {
                        Revision = envConceptDescription.Administration?.Revision,
                        Version = envConceptDescription.Administration?.Version
                    },
                    Category = envConceptDescription.Category,
                    Description = envConceptDescription.Description,
                    Id = new Identifier(envConceptDescription.Identification.Id),
                    IdShort = envConceptDescription.IdShort,
                    IsCaseOf = envConceptDescription.IsCaseOf?.ConvertAll(c => c.ToReference_V2_0()),
                    EmbeddedDataSpecifications = (envConceptDescription.EmbeddedDataSpecification?.DataSpecificationContent?.DataSpecificationIEC61360 != null) ? new List<DataSpecificationIEC61360>() : null
                };
                if (conceptDescription.EmbeddedDataSpecifications != null)
                {
                    DataSpecificationIEC61360 dataSpecification = envConceptDescription
                        .EmbeddedDataSpecification
                        .DataSpecificationContent
                        .DataSpecificationIEC61360
                        .ToDataSpecificationIEC61360();

                    (conceptDescription.EmbeddedDataSpecifications as List<DataSpecificationIEC61360>).Add(dataSpecification);
                }
                environment.ConceptDescriptions.Add(conceptDescription);
            }
            foreach (var envSubmodel in environment.EnvironmentSubmodels)
            {
                Submodel submodel = new Submodel(envSubmodel.IdShort, new Identifier(envSubmodel.Identification.Id))
                {
                    Administration = new AdministrativeInformation()
                    {
                        Revision = envSubmodel.Administration?.Revision,
                        Version = envSubmodel.Administration?.Version
                    },
                    Category = envSubmodel.Category,
                    Description = envSubmodel.Description,
                    Kind = envSubmodel.Kind,
                    SemanticId = envSubmodel.SemanticId?.ToReference_V2_0(),
                    ConceptDescription = null
                };
                List<ISubmodelElement> smElements = envSubmodel.SubmodelElements?.ConvertAll(c => c.submodelElement?.ToSubmodelElement(environment.ConceptDescriptions, submodel));
                if (smElements != null)
                    foreach (var smElement in smElements)
                        submodel.SubmodelElements.Create(smElement);

                environment.Submodels.Add(submodel);
            }
            foreach (var envAssetAdministrationShell in environment.EnvironmentAssetAdministrationShells)
            {
                AssetAdministrationShell assetAdministrationShell = new AssetAdministrationShell(envAssetAdministrationShell.IdShort, new Identifier(envAssetAdministrationShell.Identification.Id))
                {
                    Administration = new AdministrativeInformation()
                    {
                        Revision = envAssetAdministrationShell.Administration?.Revision,
                        Version = envAssetAdministrationShell.Administration?.Version
                    },
                    Category = envAssetAdministrationShell.Category,
                    DerivedFrom = envAssetAdministrationShell.DerivedFrom?.ToReference_V2_0<IAssetAdministrationShell>(),
                    Description = envAssetAdministrationShell.Description
                };

                IAssetInformation asset = environment.Assets?.Find(a => a.GlobalAssetId == envAssetAdministrationShell.AssetReference?.Keys?.FirstOrDefault()?.Value);
                assetAdministrationShell.AssetInformation = asset;

                if (envAssetAdministrationShell.SubmodelReferences != null)
                    foreach (var envSubmodelRef in envAssetAdministrationShell.SubmodelReferences)
                    {
                        ISubmodel submodel = environment.Submodels?.Find(s => s.Id == envSubmodelRef.Keys?.FirstOrDefault()?.Value);
                        if (submodel != null)
                            assetAdministrationShell.Submodels.Create(submodel);
                    }

                environment.AssetAdministrationShells.Add(assetAdministrationShell);
            }
        }

        public static void RegisterXmlValidationCallback(Action<ValidationEventArgs> callback)
        {
            _userValidationCallback = callback;
        }

        private static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                logger.LogWarning("Validation warning: " + args.Message);
            else
                logger.LogError("Validation error: " + args.Message + " | LineNumber: " + args.Exception.LineNumber + " | LinePosition: " + args.Exception.LinePosition);

            _userValidationCallback?.Invoke(args);
        }
    }
}
