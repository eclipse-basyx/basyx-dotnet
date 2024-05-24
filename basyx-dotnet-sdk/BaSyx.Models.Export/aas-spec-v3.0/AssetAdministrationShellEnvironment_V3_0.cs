/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
using BaSyx.Models.Connectivity;

namespace BaSyx.Models.Export
{
    [DataContract]
    [XmlType(AnonymousType = true, Namespace = AAS_NAMESPACE)]
    [XmlRoot(ElementName = "environment", Namespace = AAS_NAMESPACE, IsNullable = false)]
    public class AssetAdministrationShellEnvironment_V3_0
    {
        public const string AAS_NAMESPACE = "https://admin-shell.io/aas/3/0";
        public const string AAS_XSD_FILENAME = "AAS_V3_0.xsd";

        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "assetAdministrationShells", Order = 0)]
        [XmlIgnore, JsonIgnore]
        public List<IAssetAdministrationShell> AssetAdministrationShells { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "submodels", Order = 2)]
        [XmlIgnore, JsonIgnore]
        public List<ISubmodel> Submodels { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "conceptDescriptions", Order = 3)]
        [XmlIgnore, JsonIgnore]
        public List<IConceptDescription> ConceptDescriptions { get; }

        [JsonProperty("assetAdministrationShells")]
        [XmlArray("assetAdministrationShells")]
        [XmlArrayItem("assetAdministrationShell")]
        public List<EnvironmentAssetAdministrationShell_V3_0> EnvironmentAssetAdministrationShells { get; set; }

        [JsonProperty("submodels")]
        [XmlArray("submodels")]
        [XmlArrayItem("submodel")]
        public List<EnvironmentSubmodel_V3_0> EnvironmentSubmodels { get; set; }

        [JsonProperty("conceptDescriptions")]
        [XmlArray("conceptDescriptions")]
        [XmlArrayItem("conceptDescription")]
        public List<EnvironmentConceptDescription_V3_0> EnvironmentConceptDescriptions { get; set; }

        [IgnoreDataMember]
        [XmlIgnore]
        public Dictionary<string, IFileElement> SupplementalFiles;

        private string ContentRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static readonly ILogger logger = LoggingExtentions.CreateLogger<AssetAdministrationShellEnvironment_V3_0>();
        private static readonly ManifestEmbeddedFileProvider fileProvider;

        public static JsonSerializerSettings JsonSettings;
        public static XmlReaderSettings XmlSettings;

        private static Action<ValidationEventArgs> _userValidationCallback;

        static AssetAdministrationShellEnvironment_V3_0()
        {
            JsonSettings = new JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Ignore,                
            };
            JsonSettings.Converters.Add(new StringEnumConverter());

            fileProvider = new ManifestEmbeddedFileProvider(typeof(AssetAdministrationShellEnvironment_V3_0).Assembly, Path.Combine("aas-spec-v3.0", "Resources"));

            XmlSettings = new XmlReaderSettings();
            XmlSettings.ValidationType = ValidationType.Schema;
            XmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            XmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            XmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            XmlSettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);

            using (var stream = fileProvider.GetFileInfo(AAS_XSD_FILENAME).CreateReadStream())
                XmlSettings.Schemas.Add(AAS_NAMESPACE, XmlReader.Create(stream));     
        }

        [JsonConstructor]
        public AssetAdministrationShellEnvironment_V3_0()
        {
            AssetAdministrationShells = new List<IAssetAdministrationShell>();
            Submodels = new List<ISubmodel>();
            ConceptDescriptions = new List<IConceptDescription>();
            SupplementalFiles = new Dictionary<string, IFileElement>();

            EnvironmentAssetAdministrationShells = new List<EnvironmentAssetAdministrationShell_V3_0>();
            EnvironmentSubmodels = new List<EnvironmentSubmodel_V3_0>();
            EnvironmentConceptDescriptions = new List<EnvironmentConceptDescription_V3_0>();
        }

        public AssetAdministrationShellEnvironment_V3_0(params IAssetAdministrationShell[] assetAdministrationShells) : this()
        {
            foreach (var aas in assetAdministrationShells)
                AddAssetAdministrationShell(aas);

            ConvertToEnvironment();
        }

        public void AddAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            AssetAdministrationShells.Add(aas);
            if (aas.Submodels?.Count() > 0)
            {
                Submodels.AddRange(aas.Submodels.Values);
                foreach (var submodel in aas.Submodels.Values)
                {
                    ExtractAndClearConceptDescriptions(submodel.SubmodelElements);
                    ExtractSupplementalFiles(submodel.SubmodelElements);
                }
            }
        }

        private void ConvertToEnvironment()
        {
            foreach (var conceptDescription in ConceptDescriptions)
            {
                EmbeddedDataSpecification_V3_0 embeddedDataSpecification = null;
                var dataSpecification = conceptDescription.EmbeddedDataSpecifications?.FirstOrDefault();
                if (dataSpecification != null && dataSpecification.DataSpecificationContent is DataSpecificationIEC61360Content dataSpecificationContent)
                {
                    embeddedDataSpecification = new EmbeddedDataSpecification_V3_0()
                    {
                        DataSpecification = dataSpecification.DataSpecification?.ToEnvironmentReference_V3_0(),
                        DataSpecificationContent = new DataSpecificationContent_V3_0()
                        {
                            DataSpecificationIEC61360 = dataSpecificationContent.ToEnvironmentDataSpecificationIEC61360_V3_0()
                        }
                    };
                }

                EnvironmentConceptDescription_V3_0 environmentConceptDescription = new EnvironmentConceptDescription_V3_0();
                environmentConceptDescription.Category = conceptDescription.Category;
                environmentConceptDescription.Description = conceptDescription.Description?.ToEnvironmentLangStringSet();
                environmentConceptDescription.Id = conceptDescription.Id;
                environmentConceptDescription.IdShort = conceptDescription.IdShort;
                environmentConceptDescription.IsCaseOf = conceptDescription.IsCaseOf?.ToList().ConvertAll(c => c.ToEnvironmentReference_V3_0());
                environmentConceptDescription.EmbeddedDataSpecifications = new List<EmbeddedDataSpecification_V3_0> { embeddedDataSpecification };

                if (conceptDescription.Administration != null)
                {
                    environmentConceptDescription.Administration = new EnvironmentAdministrativeInformation_V3_0()
                    {
                        Version = conceptDescription.Administration.Version,
                        Revision = conceptDescription.Administration.Revision
                    };
                }

                if (EnvironmentConceptDescriptions.Find(m => m.Id == conceptDescription.Id) == null)
                    EnvironmentConceptDescriptions.Add(environmentConceptDescription);
            }
            foreach (var assetAdministrationShell in AssetAdministrationShells)
            {
                EnvironmentAssetAdministrationShell_V3_0 environmentAssetAdministrationShell = new EnvironmentAssetAdministrationShell_V3_0();
                environmentAssetAdministrationShell.Category = assetAdministrationShell.Category;
                environmentAssetAdministrationShell.Description = assetAdministrationShell.Description?.ToEnvironmentLangStringSet();
                environmentAssetAdministrationShell.Id = assetAdministrationShell.Id;
                environmentAssetAdministrationShell.IdShort = assetAdministrationShell.IdShort;

                if (assetAdministrationShell.AssetInformation != null)
                {
                    environmentAssetAdministrationShell.AssetInformation = new EnvironmentAssetInformation_V3_0()
                    {
                        AssetKind = assetAdministrationShell.AssetInformation.AssetKind,
                        AssetType = assetAdministrationShell.AssetInformation.AssetType,
                        GlobalAssetId = assetAdministrationShell.AssetInformation.GlobalAssetId,
                        SpecificAssetIds = assetAdministrationShell.AssetInformation.SpecificAssetIds?.ToList().ConvertAll(c => new EnvironmentSpecificAssetId_V3_0()
                        {
                            Name = c.Name,
                            ExternalSubjectId = c.ExternalSubjectId?.ToEnvironmentReference_V3_0(),
                            SemanticId = c.SemanticId?.ToEnvironmentReference_V3_0(),
                            SupplementalSemanticIds = c.SupplementalSemanticIds?.ToList().ConvertAll(d => d.ToEnvironmentReference_V3_0()),
                            Value = c.Value
                        }),                        
                    };
                    if(assetAdministrationShell.AssetInformation.DefaultThumbnail != null)
                    {
                        environmentAssetAdministrationShell.AssetInformation.DefaultThumbnail = new EnvironmentResource_V3_0()
                        {
                            ContentType = assetAdministrationShell.AssetInformation.DefaultThumbnail?.ContentType,
                            Path = assetAdministrationShell.AssetInformation.DefaultThumbnail?.Path
                        };
                    }
                }
                if (assetAdministrationShell.Administration != null)
                {
                    environmentAssetAdministrationShell.Administration = new EnvironmentAdministrativeInformation_V3_0()
                    {
                        Version = assetAdministrationShell.Administration.Version,
                        Revision = assetAdministrationShell.Administration.Revision
                    };
                }

                environmentAssetAdministrationShell.SubmodelReferences = new List<EnvironmentReference_V3_0>();
                foreach (var submodel in assetAdministrationShell.Submodels)
                    environmentAssetAdministrationShell.SubmodelReferences.Add(submodel.ToEnvironmentReference_V3_0());

                EnvironmentAssetAdministrationShells.Add(environmentAssetAdministrationShell);
            }
            foreach (var submodel in Submodels)
            {
                EnvironmentSubmodel_V3_0 environmentSubmodel = new EnvironmentSubmodel_V3_0();
                environmentSubmodel.Category = submodel.Category;
                environmentSubmodel.Description = submodel.Description?.ToEnvironmentLangStringSet();
                environmentSubmodel.Id = submodel.Id;
                environmentSubmodel.IdShort = submodel.IdShort;
                environmentSubmodel.Kind = submodel.Kind;
                environmentSubmodel.Qualifier = null; //TODO
                environmentSubmodel.SemanticId = submodel.SemanticId?.ToEnvironmentReference_V3_0();
                if(submodel.Administration != null)
                {
                    environmentSubmodel.Administration = new EnvironmentAdministrativeInformation_V3_0()
                    {
                        Version = submodel.Administration.Version,
                        Revision = submodel.Administration.Revision
                    };
                }

                environmentSubmodel.SubmodelElements = new List<SubmodelElementType_V3_0>();
                foreach (var submodelElement in submodel.SubmodelElements)
                    environmentSubmodel.SubmodelElements.Add(submodelElement.ToEnvironmentSubmodelElement_V3_0());


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
                        if (!destinationPath.StartsWith(AASX_V3_0.AASX_FOLDER))
                            destinationPath = AASX_V3_0.AASX_FOLDER + destinationPath;

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
                    (smElement as SubmodelElement).SemanticId = new Reference(new Key(KeyType.ConceptDescription, smElement.ConceptDescription.Id));
                    (smElement as SubmodelElement).ConceptDescription = null;
                    (smElement as SubmodelElement).EmbeddedDataSpecifications = null;
                }
                if (smElement.ModelType == ModelType.SubmodelElementCollection)
                    ExtractAndClearConceptDescriptions((smElement as SubmodelElementCollection).Value);
            }
        }

        public void SetContentRoot(string contentRoot) => ContentRoot = contentRoot;

        public void WriteEnvironment_V3_0(ExportType exportType, string filePath) => WriteEnvironment_V3_0(this, exportType, filePath);

        public static void WriteEnvironment_V3_0(AssetAdministrationShellEnvironment_V3_0 environment, ExportType exportType, string filePath)
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
                            XmlSerializer serializer = new XmlSerializer(typeof(AssetAdministrationShellEnvironment_V3_0), AAS_NAMESPACE);
                            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                            namespaces.Add("xsi", XmlSchema.InstanceNamespace);
                            namespaces.Add("aas", AAS_NAMESPACE);
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

        public static AssetAdministrationShellEnvironment_V3_0 ReadEnvironment_V3_0(Stream stream, ExportType exportType)
        {
            AssetAdministrationShellEnvironment_V3_0 env = null;

            try
            {
                switch (exportType)
                {
                    case ExportType.Xml:
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(AssetAdministrationShellEnvironment_V3_0), AAS_NAMESPACE);

                            using (XmlReader reader = XmlReader.Create(stream, XmlSettings))
                                env = (AssetAdministrationShellEnvironment_V3_0)serializer.Deserialize(reader);
                        }
                        break;
                    case ExportType.Json:
                        {
                            using (StreamReader reader = new StreamReader(stream))
                                env = JsonConvert.DeserializeObject<AssetAdministrationShellEnvironment_V3_0>(reader.ReadToEnd(), JsonSettings);
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


        public static AssetAdministrationShellEnvironment_V3_0 ReadEnvironment_V3_0(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(filePath);
            if (!System.IO.File.Exists(filePath))
                throw new ArgumentException(filePath + " does not exist");

            AssetAdministrationShellEnvironment_V3_0 env = null;

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
                env = ReadEnvironment_V3_0(file, exportType);

            return env;
        }

        private static void ConvertToAssetAdministrationShell(AssetAdministrationShellEnvironment_V3_0 environment)
        {
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
                    Id = envConceptDescription.Id,
                    IdShort = envConceptDescription.IdShort,
                    IsCaseOf = envConceptDescription.IsCaseOf?.ConvertAll(c => c.ToReference_V3_0()),
                };
                if (envConceptDescription.Description?.Count() > 0)
                    conceptDescription.Description = new LangStringSet(envConceptDescription.Description.ConvertAll(l => new LangString(l.Language, l.Text)));

                foreach (var envEmbeddedDataSpecification in envConceptDescription.EmbeddedDataSpecifications)
                {
                    var dataSpecIec61360 = envEmbeddedDataSpecification.DataSpecificationContent?.DataSpecificationIEC61360?.ToDataSpecificationIEC61360();
                    if(dataSpecIec61360 != null)
                    {
						(conceptDescription.EmbeddedDataSpecifications as List<IEmbeddedDataSpecification>).Add(dataSpecIec61360);
					}
                }
                environment.ConceptDescriptions.Add(conceptDescription);
            }
            foreach (var envSubmodel in environment.EnvironmentSubmodels)
            {
                Submodel submodel = new Submodel(envSubmodel.IdShort, envSubmodel.Id)
                {
                    Administration = new AdministrativeInformation()
                    {
                        Revision = envSubmodel.Administration?.Revision,
                        Version = envSubmodel.Administration?.Version
                    },
                    Category = envSubmodel.Category,
                    Kind = envSubmodel.Kind,
                    SemanticId = envSubmodel.SemanticId?.ToReference_V3_0(),
                    ConceptDescription = null
                };

                if (envSubmodel.Description?.Count() > 0)
                    submodel.Description = new LangStringSet(envSubmodel.Description?.ConvertAll(l => new LangString(l.Language, l.Text)));

				List<ISubmodelElement> smElements = envSubmodel.SubmodelElements?.ConvertAll(c => c.ToSubmodelElement(environment.ConceptDescriptions, submodel));
                if (smElements != null)
                    foreach (var smElement in smElements)
                        submodel.SubmodelElements.Create(smElement);

                environment.Submodels.Add(submodel);
            }
            foreach (var envAssetAdministrationShell in environment.EnvironmentAssetAdministrationShells)
            {
                AssetAdministrationShell assetAdministrationShell = new AssetAdministrationShell(envAssetAdministrationShell.IdShort, envAssetAdministrationShell.Id)
                {
                    Administration = new AdministrativeInformation()
                    {
                        Revision = envAssetAdministrationShell.Administration?.Revision,
                        Version = envAssetAdministrationShell.Administration?.Version
                    },
					AssetInformation = new AssetInformation()
					{
						GlobalAssetId = envAssetAdministrationShell.AssetInformation?.GlobalAssetId,
						AssetKind = envAssetAdministrationShell.AssetInformation.AssetKind,
                        SpecificAssetIds = envAssetAdministrationShell.AssetInformation.SpecificAssetIds?.ConvertAll(s => new SpecificAssetId()
                        {
                            ExternalSubjectId = s.ExternalSubjectId?.ToReference_V3_0(),
                            Name = s.Name,
                            SemanticId = s.SemanticId?.ToReference_V3_0(),
                            SupplementalSemanticIds = s.SupplementalSemanticIds?.ConvertAll(r => r.ToReference_V3_0()),
                            Value = s.Value
                        })
					},
				    Category = envAssetAdministrationShell.Category,
                    DerivedFrom = envAssetAdministrationShell.DerivedFrom?.ToReference_V3_0<IAssetAdministrationShell>()
                };

                if (envAssetAdministrationShell.Description?.Count() > 0)
                    assetAdministrationShell.Description = new LangStringSet(envAssetAdministrationShell.Description?.ConvertAll(l => new LangString(l.Language, l.Text)));

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
