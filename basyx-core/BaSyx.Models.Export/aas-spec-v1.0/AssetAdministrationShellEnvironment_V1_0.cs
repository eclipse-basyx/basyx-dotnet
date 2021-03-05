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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Export.Converter;
using BaSyx.Models.Export.EnvironmentSubmodelElements;
using BaSyx.Models.Extensions.Semantics.DataSpecifications;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using File = BaSyx.Models.Core.AssetAdministrationShell.Implementations.File;

namespace BaSyx.Models.Export
{
    [DataContract]
    [XmlType(AnonymousType = true, Namespace = AAS_NAMESPACE)]
    [XmlRoot(ElementName = "aasenv", Namespace = AAS_NAMESPACE, IsNullable = false)]
    public class AssetAdministrationShellEnvironment_V1_0
    {
        public const string AAS_NAMESPACE = "http://www.admin-shell.io/aas/1/0";
        public const string IEC61360_NAMESPACE = "http://www.admin-shell.io/IEC61360/1/0";
        public const string AAS_XSD_FILENAME = "aas-spec-v1.0/AAS.xsd";
        public const string IEC61360_XSD_FILENAME = "aas-spec-v1.0/IEC61360.xsd";

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
        public List<EnvironmentAssetAdministrationShell_V1_0> EnvironmentAssetAdministrationShells { get; set; }

        [JsonProperty("assets")]
        [XmlArray("assets")]
        [XmlArrayItem("asset")]
        public List<EnvironmentAsset_V1_0> EnvironmentAssets  { get; set; }

        [JsonProperty("submodels")]
        [XmlArray("submodels")]
        [XmlArrayItem("submodel")]
        public List<EnvironmentSubmodel_V1_0> EnvironmentSubmodels { get; set; }

        [JsonProperty("conceptDescriptions")]
        [XmlArray("conceptDescriptions")]
        [XmlArrayItem("conceptDescription")]
        public List<EnvironmentConceptDescription_V1_0> EnvironmentConceptDescriptions { get; set; }

        [IgnoreDataMember]
        [XmlIgnore]
        public Dictionary<string, IFile> SupplementalFiles;

        private string ContentRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly ManifestEmbeddedFileProvider fileProvider;

        public static JsonSerializerSettings JsonSettings;
        public static XmlReaderSettings XmlSettings;

        static AssetAdministrationShellEnvironment_V1_0()
        {
            JsonSettings = new JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Ignore
            };
            JsonSettings.Converters.Add(new StringEnumConverter());

            fileProvider = new ManifestEmbeddedFileProvider(typeof(AssetAdministrationShellEnvironment_V2_0).Assembly, "aas-spec-v1.0");

            XmlSettings = new XmlReaderSettings();
            XmlSettings.ValidationType = ValidationType.Schema;
            XmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            XmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            XmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            XmlSettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            using (var stream = fileProvider.GetFileInfo(AAS_XSD_FILENAME).CreateReadStream())
                XmlSettings.Schemas.Add(AAS_NAMESPACE, XmlReader.Create(stream));

            using (var stream = fileProvider.GetFileInfo(IEC61360_XSD_FILENAME).CreateReadStream())
                XmlSettings.Schemas.Add(IEC61360_NAMESPACE, XmlReader.Create(stream));
        }

        [JsonConstructor]
        protected AssetAdministrationShellEnvironment_V1_0()
        {
            AssetAdministrationShells = new List<IAssetAdministrationShell>();
            Submodels = new List<ISubmodel>();
            Assets = new List<IAsset>();
            ConceptDescriptions = new List<IConceptDescription>();
            SupplementalFiles = new Dictionary<string, IFile>();

            EnvironmentAssetAdministrationShells = new List<EnvironmentAssetAdministrationShell_V1_0>();
            EnvironmentAssets = new List<EnvironmentAsset_V1_0>();
            EnvironmentSubmodels = new List<EnvironmentSubmodel_V1_0>();
            EnvironmentConceptDescriptions = new List<EnvironmentConceptDescription_V1_0>();
        }

        public AssetAdministrationShellEnvironment_V1_0(params IAssetAdministrationShell[] assetAdministrationShells) : this()
        {
            foreach (var aas in assetAdministrationShells)
                AddAssetAdministrationShell(aas);

            ConvertToEnvironment();
        }

        public void AddAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            AssetAdministrationShells.Add(aas);
            Assets.Add(aas.Asset);
            if (aas.Submodels?.Count() > 0)
            {
                Submodels.AddRange(aas.Submodels.Values);
                foreach (var submodel in aas.Submodels.Values)
                {
                    ExtractAndClearConceptDescriptions(submodel.SubmodelElements);
                    ExtractSupplementalFiles(submodel.SubmodelElements);
                    ResetConstraints(submodel.SubmodelElements);
                    DeleteEvents(submodel.SubmodelElements);
                }
            }
        }

        private void ConvertToEnvironment()
        {
            foreach (var asset in Assets)
            {
                EnvironmentAsset_V1_0 envAsset = new EnvironmentAsset_V1_0()
                {
                    Administration = asset.Administration,
                    AssetIdentificationModelReference = asset.AssetIdentificationModel?.ToEnvironmentReference_V1_0(),
                    Category = asset.Category,
                    Description = asset.Description,
                    Identification = asset.Identification,
                    IdShort = asset.IdShort,
                    Kind = asset.Kind                
                };
                EnvironmentAssets.Add(envAsset);
            }
            foreach (var conceptDescription in ConceptDescriptions)
            {
                EmbeddedDataSpecification_V1_0 embeddedDataSpecification = null;
                var dataSpecification = conceptDescription.EmbeddedDataSpecifications?.FirstOrDefault();
                if(dataSpecification != null && dataSpecification.DataSpecificationContent is DataSpecificationIEC61360Content dataSpecificationContent)
                {
                    embeddedDataSpecification = new EmbeddedDataSpecification_V1_0()
                    {
                        HasDataSpecification = dataSpecification.HasDataSpecification?.ToEnvironmentReference_V1_0(),
                        DataSpecificationContent = new DataSpecificationContent_V1_0()
                        {
                            DataSpecificationIEC61360 = dataSpecificationContent.ToEnvironmentDataSpecificationIEC61360_V1_0()
                        }
                    };
                }

                EnvironmentConceptDescription_V1_0 environmentConceptDescription = new EnvironmentConceptDescription_V1_0()
                {
                    Administration = conceptDescription.Administration,
                    Category = conceptDescription.Category,
                    Description = conceptDescription.Description,
                    Identification = conceptDescription.Identification,
                    IdShort = conceptDescription.IdShort,
                    IsCaseOf = conceptDescription.IsCaseOf?.ToList()?.ConvertAll(c => c.ToEnvironmentReference_V1_0()),
                    EmbeddedDataSpecification = embeddedDataSpecification
                };

                if (EnvironmentConceptDescriptions.Find(m => m.Identification.Id == conceptDescription.Identification.Id) == null)
                    EnvironmentConceptDescriptions.Add(environmentConceptDescription);
            }
            foreach (var assetAdministrationShell in AssetAdministrationShells)
            {
                EnvironmentAssetAdministrationShell_V1_0 environmentAssetAdministrationShell = new EnvironmentAssetAdministrationShell_V1_0()
                {
                    Administration = assetAdministrationShell.Administration,
                    Category = assetAdministrationShell.Category,
                    Description = assetAdministrationShell.Description,
                    IdShort = assetAdministrationShell.IdShort,
                    Identification = assetAdministrationShell.Identification,
                    AssetReference = assetAdministrationShell.Asset?.ToEnvironmentReference_V1_0(),
                    SubmodelReferences = assetAdministrationShell.Submodels?.ToList().ConvertAll(c => c.ToEnvironmentReference_V1_0()),
                    Views = null,
                    ConceptDictionaries = null
                };
                EnvironmentAssetAdministrationShells.Add(environmentAssetAdministrationShell);
            }
            foreach (var submodel in Submodels)
            {
                EnvironmentSubmodel_V1_0 environmentSubmodel = new EnvironmentSubmodel_V1_0()
                {
                    Administration = submodel.Administration,
                    Category = submodel.Category,
                    Description = submodel.Description,
                    Identification = submodel.Identification,
                    IdShort = submodel.IdShort,
                    Kind = submodel.Kind,
                    Qualifier = null,
                    SemanticId = submodel.SemanticId?.ToEnvironmentReference_V1_0(),
                    SubmodelElements = submodel.SubmodelElements.ToList().ConvertAll(c => c.ToEnvironmentSubmodelElement_V1_0())
                };
                EnvironmentSubmodels.Add(environmentSubmodel);
            }
        }


        private void ExtractSupplementalFiles(IElementContainer<ISubmodelElement> submodelElements)
        {
            foreach (var smElement in submodelElements)
            {
                if (smElement is File file)
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

        private void ResetConstraints(IElementContainer<ISubmodelElement> submodelElements)
        {
            foreach (var smElement in submodelElements)
            {
                if(smElement.Constraints?.Count() > 0)
                    (smElement as SubmodelElement).Constraints = null;
                if(smElement is IOperation operation)
                {
                    if(operation.InputVariables?.Count > 0)
                        ResetConstraints((smElement as IOperation).InputVariables.ToElementContainer());
                    if (operation.OutputVariables?.Count > 0)
                        ResetConstraints((smElement as IOperation).OutputVariables.ToElementContainer());
                }
                else if (smElement.ModelType == ModelType.SubmodelElementCollection)
                    ResetConstraints((smElement as SubmodelElementCollection).Value);
            }
        }

        private void DeleteEvents(IElementContainer<ISubmodelElement> submodelElements)
        {
            var eventsToDelete = submodelElements.Where(s => s.ModelType == ModelType.Event || s.ModelType == ModelType.BasicEvent).ToList();
            foreach (var eventable in eventsToDelete)
                submodelElements.Delete(eventable.IdShort);
        }

        public void WriteEnvironment_V1_0(ExportType exportType, string filePath) => WriteEnvironment_V1_0(this, exportType, filePath);

        public static void WriteEnvironment_V1_0(AssetAdministrationShellEnvironment_V1_0 environment, ExportType exportType, string filePath)
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
                        logger.Error(e);
                    }
                    break;
                case ExportType.Xml:
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(AssetAdministrationShellEnvironment_V1_0), AAS_NAMESPACE);
                            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                            namespaces.Add("xsi", XmlSchema.InstanceNamespace);
                            namespaces.Add("aas", AAS_NAMESPACE);
                            namespaces.Add("IEC61360", IEC61360_NAMESPACE);
                            serializer.Serialize(writer, environment, namespaces);
                        }

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }
                    break;
                default:
                    break;
            }
        }


        public static AssetAdministrationShellEnvironment_V1_0 ReadEnvironment_V1_0(Stream stream, ExportType exportType)
        {
            AssetAdministrationShellEnvironment_V1_0 env = null;

            try
            {
                switch (exportType)
                {
                    case ExportType.Xml:
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(AssetAdministrationShellEnvironment_V1_0), AAS_NAMESPACE);

                            using (XmlReader reader = XmlReader.Create(stream, XmlSettings))
                                env = (AssetAdministrationShellEnvironment_V1_0)serializer.Deserialize(reader);
                        }
                        break;
                    case ExportType.Json:
                        {
                            using (StreamReader reader = new StreamReader(stream))
                                env = JsonConvert.DeserializeObject<AssetAdministrationShellEnvironment_V1_0>(reader.ReadToEnd(), JsonSettings);
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
                logger.Error(e, "Failed to read environment - Exception: " + e.Message);
                return null;
            }
        }


        public static AssetAdministrationShellEnvironment_V1_0 ReadEnvironment_V1_0(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(filePath);
            if (!System.IO.File.Exists(filePath))
                throw new ArgumentException(filePath + " does not exist");

            AssetAdministrationShellEnvironment_V1_0 env = null;

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
                env = ReadEnvironment_V1_0(file, exportType);

            if (env == null)
                return null;

            ConvertToAssetAdministrationShell(env);

            return env;
        }

        private static void ConvertToAssetAdministrationShell(AssetAdministrationShellEnvironment_V1_0 environment)
        {
            foreach (var envAsset in environment.EnvironmentAssets)
            {
                Asset asset = new Asset(envAsset.IdShort, envAsset.Identification)
                {
                    Administration = envAsset.Administration,
                    Category = envAsset.Category,
                    Description = envAsset.Description,
                    Kind = envAsset.Kind,
                    AssetIdentificationModel = envAsset.AssetIdentificationModelReference?.ToReference_V1_0<ISubmodel>()                  
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
                    IsCaseOf = envConceptDescription.IsCaseOf?.ConvertAll(c => c.ToReference_V1_0()),
                    EmbeddedDataSpecifications = (envConceptDescription.EmbeddedDataSpecification?.DataSpecificationContent?.DataSpecificationIEC61360 != null) ? new List<DataSpecificationIEC61360>() : null
                };
                if(conceptDescription.EmbeddedDataSpecifications != null)
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
                    SemanticId = envSubmodel.SemanticId?.ToReference_V1_0(),
                    ConceptDescription = null,                    
                };
                submodel.SubmodelElements.AddRange(envSubmodel.SubmodelElements.ConvertAll(c => c.submodelElement?.ToSubmodelElement(environment.ConceptDescriptions, submodel)));

                environment.Submodels.Add(submodel);
            }
            foreach (var envAssetAdministrationShell in environment.EnvironmentAssetAdministrationShells)
            {
                AssetAdministrationShell assetAdministrationShell = new AssetAdministrationShell(envAssetAdministrationShell.IdShort, envAssetAdministrationShell.Identification)
                {
                    Administration = envAssetAdministrationShell.Administration,
                    Category = envAssetAdministrationShell.Category,
                    DerivedFrom = envAssetAdministrationShell.DerivedFrom?.ToReference_V1_0<IAssetAdministrationShell>(),
                    Description = envAssetAdministrationShell.Description                 
                };

                IAsset asset = environment.Assets.Find(a => a.Identification.Id == envAssetAdministrationShell.AssetReference?.Keys?.FirstOrDefault()?.Value);
                assetAdministrationShell.Asset = asset;

                foreach (var envSubmodelRef in envAssetAdministrationShell.SubmodelReferences)
                {
                    ISubmodel submodel = environment.Submodels.Find(s => s.Identification.Id == envSubmodelRef.Keys?.FirstOrDefault()?.Value);
                    if (submodel != null)
                        assetAdministrationShell.Submodels.Create(submodel);
                }

                environment.AssetAdministrationShells.Add(assetAdministrationShell);
            }
        }

        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                logger.Warn("Validation warning: " + args.Message);
            else
                logger.Error("Validation error: " + args.Message + " | LineNumber: " + args.Exception.LineNumber + " | LinePosition: " + args.Exception.LinePosition);

        }    
    }
}
