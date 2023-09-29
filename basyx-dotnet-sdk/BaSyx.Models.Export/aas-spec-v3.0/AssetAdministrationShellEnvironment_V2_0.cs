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
        public List<IAsset> Assets { get; }

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
        protected AssetAdministrationShellEnvironment_V2_0()
        {
            AssetAdministrationShells = new List<IAssetAdministrationShell>();
            Submodels = new List<ISubmodel>();
            Assets = new List<IAsset>();
            ConceptDescriptions = new List<IConceptDescription>();
            SupplementalFiles = new Dictionary<string, IFileElement>();

            EnvironmentAssetAdministrationShells = new List<EnvironmentAssetAdministrationShell_V2_0>();
            EnvironmentAssets = new List<EnvironmentAsset_V2_0>();
            EnvironmentSubmodels = new List<EnvironmentSubmodel_V2_0>();
            EnvironmentConceptDescriptions = new List<EnvironmentConceptDescription_V2_0>();
        }

        public AssetAdministrationShellEnvironment_V2_0(params IAssetAdministrationShell[] assetAdministrationShells) : this()
        {
            foreach (var aas in assetAdministrationShells)
                AddAssetAdministrationShell(aas, true);

            BuildEnvironment();
        }

        public void AddAssetAdministrationShell(IAssetAdministrationShell aas, bool includeSubmodels)
        {
            AssetAdministrationShells.Add(aas);
            if (aas.Asset != null)
                Assets.Add(aas.Asset);
            if (includeSubmodels && aas.Submodels?.Count() > 0)
                foreach (var submodel in aas.Submodels.Values)
                    AddSubmodel(submodel);
        }

        public void AddAsset(IAsset asset)
        {
            Assets.Add(asset);
        }

        public void AddSubmodel(ISubmodel submodel)
        {
            Submodels.Add(submodel);
            ExtractAndClearConceptDescriptions(submodel.SubmodelElements);
            ExtractSupplementalFiles(submodel.SubmodelElements);
        }

        public void AddConceptDescription(IConceptDescription conceptDescription)
        {
            ConceptDescriptions.Add(conceptDescription);
        }

        public void BuildEnvironment()
        {
            foreach (var asset in Assets)
            {
                EnvironmentAsset_V2_0 envAsset = new EnvironmentAsset_V2_0()
                {
                    Administration = asset.Administration,
                    AssetIdentificationModelReference = asset.AssetIdentificationModel?.ToEnvironmentReference_V2_0(),
                    Category = asset.Category,
                    Description = asset.Description,
                    Identification = asset.Identification,
                    IdShort = asset.IdShort,
                    Kind = asset.Kind,
                };
                EnvironmentAssets.Add(envAsset);
            }
            foreach (var conceptDescription in ConceptDescriptions)
            {
                EmbeddedDataSpecification_V2_0 embeddedDataSpecification = null;
                var dataSpecification = conceptDescription.EmbeddedDataSpecifications?.FirstOrDefault();
                if (dataSpecification != null && dataSpecification.DataSpecificationContent is DataSpecificationIEC61360Content dataSpecificationContent)
                {
                    embeddedDataSpecification = new EmbeddedDataSpecification_V2_0()
                    {
                        DataSpecification = dataSpecification.HasDataSpecification?.ToEnvironmentReference_V2_0(),
                        DataSpecificationContent = new DataSpecificationContent_V2_0()
                        {
                            DataSpecificationIEC61360 = dataSpecificationContent.ToEnvironmentDataSpecificationIEC61360_V2_0()
                        }
                    };
                }

                EnvironmentConceptDescription_V2_0 environmentConceptDescription = new EnvironmentConceptDescription_V2_0()
                {
                    Administration = conceptDescription.Administration,
                    Category = conceptDescription.Category,
                    Description = conceptDescription.Description,
                    Identification = conceptDescription.Identification,
                    IdShort = conceptDescription.IdShort,
                    IsCaseOf = conceptDescription.IsCaseOf?.ToList()?.ConvertAll(c => c.ToEnvironmentReference_V2_0()),
                    EmbeddedDataSpecification = embeddedDataSpecification
                };

                if(EnvironmentConceptDescriptions.Find(m => m.Identification.Id == conceptDescription.Identification.Id) == null)
                    EnvironmentConceptDescriptions.Add(environmentConceptDescription);
            }
            foreach (var assetAdministrationShell in AssetAdministrationShells)
            {
                EnvironmentAssetAdministrationShell_V2_0 environmentAssetAdministrationShell = new EnvironmentAssetAdministrationShell_V2_0()
                {
                    Administration = assetAdministrationShell.Administration,
                    Category = assetAdministrationShell.Category,
                    Description = assetAdministrationShell.Description,
                    IdShort = assetAdministrationShell.IdShort,
                    Identification = assetAdministrationShell.Identification,
                    AssetReference = assetAdministrationShell.Asset?.ToEnvironmentReference_V2_0(),
                    Views = null,
                    ConceptDictionaries = null
                };
                environmentAssetAdministrationShell.SubmodelReferences = new List<EnvironmentReference_V2_0>();
                foreach (var submodel in assetAdministrationShell.Submodels)
                    environmentAssetAdministrationShell.SubmodelReferences.Add(submodel.ToEnvironmentReference_V2_0());

                EnvironmentAssetAdministrationShells.Add(environmentAssetAdministrationShell);
            }
            foreach (var submodel in Submodels)
            {
                EnvironmentSubmodel_V2_0 environmentSubmodel = new EnvironmentSubmodel_V2_0()
                {
                    Administration = submodel.Administration,
                    Category = submodel.Category,
                    Description = submodel.Description,
                    Identification = submodel.Identification,
                    IdShort = submodel.IdShort,
                    Kind = submodel.Kind,
                    Qualifier = null,
                    SemanticId = submodel.SemanticId?.ToEnvironmentReference_V2_0()
                };

                environmentSubmodel.SubmodelElements = new List<EnvironmentSubmodelElement_V2_0>();
                foreach (var submodelElement in submodel.SubmodelElements)
                    environmentSubmodel.SubmodelElements.Add(submodelElement.ToEnvironmentSubmodelElement_V2_0());


                EnvironmentSubmodels.Add(environmentSubmodel);
            }
        }


        private void ExtractSupplementalFiles(IElementContainer<ISubmodelElement> submodelElements)
        {
            foreach (var smElement in submodelElements)
            {
                if (smElement is AdminShell.FileElement file)
                {
                    string filePath = ContentRoot + file.Value.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                    if (System.IO.File.Exists(filePath))
                    {
                        string destinationPath = file.Value;
                        if (!destinationPath.StartsWith(AASX.AASX_FOLDER))
                            destinationPath = AASX.AASX_FOLDER + destinationPath;

                        file.Value = destinationPath;
                        SupplementalFiles.Add(filePath, file);
                    }
                }
                else if (smElement.ModelType == ModelType.SubmodelElementCollection)
                    ExtractSupplementalFiles((smElement as SubmodelElementCollection).Value);
            }
        }

        private void ExtractAndClearConceptDescriptions(IElementContainer<ISubmodelElement> submodelElements)
        {
            foreach (var smElement in submodelElements)
            {
                if (smElement.ConceptDescription != null)
                {
                    ConceptDescriptions.Add(smElement.ConceptDescription);
                    (smElement as SubmodelElement).SemanticId = new Reference(new Key(KeyElements.ConceptDescription, smElement.ConceptDescription.Identification.IdType, smElement.ConceptDescription.Identification.Id, true));
                    (smElement as SubmodelElement).ConceptDescription = null;
                    (smElement as SubmodelElement).EmbeddedDataSpecifications = null;
                }
                if (smElement.ModelType == ModelType.SubmodelElementCollection)
                    ExtractAndClearConceptDescriptions((smElement as SubmodelElementCollection).Value);
            }
        }

        public void SetContentRoot(string contentRoot) => ContentRoot = contentRoot;

        public void WriteEnvironment_V2_0(ExportType exportType, string filePath) => WriteEnvironment_V2_0(this, exportType, filePath);

        public static void WriteEnvironment_V2_0(AssetAdministrationShellEnvironment_V2_0 environment, ExportType exportType, string filePath)
        {
            if (environment == null)
                return;

            switch (exportType)
            {
                case ExportType.Json:
                    try
                    {
                        string serialized = JsonConvert.SerializeObject(environment, JsonSettings);
                        System.IO.File.WriteAllText(filePath, serialized);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Exception while writing environment to JSON");
                    }
                    break;
                case ExportType.Xml:
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(AssetAdministrationShellEnvironment_V2_0), AAS_NAMESPACE);
                            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                            namespaces.Add("xsi", XmlSchema.InstanceNamespace);
                            namespaces.Add("aas", AAS_NAMESPACE);
                            namespaces.Add("IEC61360", IEC61360_NAMESPACE);
                            serializer.Serialize(writer, environment, namespaces);
                        }

                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Exception while writing environment to XML");
                    }
                    break;
                default:
                    break;
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
                Asset asset = new Asset(envAsset.IdShort, envAsset.Identification)
                {
                    Administration = envAsset.Administration,
                    Category = envAsset.Category,
                    Description = envAsset.Description,
                    Kind = envAsset.Kind,
                    AssetIdentificationModel = envAsset.AssetIdentificationModelReference?.ToReference_V2_0<ISubmodel>(),
                    BillOfMaterial = envAsset.BillOfMaterial?.ToReference_V2_0<ISubmodel>()
                };
                environment.Assets.Add(asset);
            }
            foreach (var envConceptDescription in environment.EnvironmentConceptDescriptions)
            {
                ConceptDescription conceptDescription = new ConceptDescription()
                {
                    Administration = envConceptDescription.Administration,
                    Category = envConceptDescription.Category,
                    Description = envConceptDescription.Description,
                    Identification = envConceptDescription.Identification,
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
                Submodel submodel = new Submodel(envSubmodel.IdShort, envSubmodel.Identification)
                {
                    Administration = envSubmodel.Administration,
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
                AssetAdministrationShell assetAdministrationShell = new AssetAdministrationShell(envAssetAdministrationShell.IdShort, envAssetAdministrationShell.Identification)
                {
                    Administration = envAssetAdministrationShell.Administration,
                    Category = envAssetAdministrationShell.Category,
                    DerivedFrom = envAssetAdministrationShell.DerivedFrom?.ToReference_V2_0<IAssetAdministrationShell>(),
                    Description = envAssetAdministrationShell.Description
                };

                IAsset asset = environment.Assets?.Find(a => a.Identification.Id == envAssetAdministrationShell.AssetReference?.Keys?.FirstOrDefault()?.Value);
                assetAdministrationShell.Asset = asset;

                if (envAssetAdministrationShell.SubmodelReferences != null)
                    foreach (var envSubmodelRef in envAssetAdministrationShell.SubmodelReferences)
                    {
                        ISubmodel submodel = environment.Submodels?.Find(s => s.Identification.Id == envSubmodelRef.Keys?.FirstOrDefault()?.Value);
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
