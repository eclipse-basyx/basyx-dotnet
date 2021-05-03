/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Export;
using BaSyx.Utils.PathHandling;
using CommandLine;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;

namespace BaSyx.CLI.Options
{
    [Verb("refine", HelpText = "Refine Asset Adminstration Shell XML and JSON serialization according to schema")]
    class RefineOptions
    {
        [Option('i', Required = true, HelpText = "Input file (e.g. .aasx, .xml, .json)")]
        public string InputFileName { get; set; }

        [Option('o', Required = true, HelpText = "Output file (e.g. .aasx, .xml, .json)")]
        public string OutputFileName { get; set; }

        [Option('v', Required = false, HelpText = "Re-Validate at the end")]
        public bool Revalidate { get; set; }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static int RunRefineAndReturnExitCode(RefineOptions opts)
        {
            string fileExtension = Path.GetExtension(opts.InputFileName);
            switch (fileExtension)
            {
                case ".aasx":
                    {
                        try
                        {
                            string tempEnvironmentFile = Path.GetTempFileName() + ".xml";
                            DirectoryInfo tempDirInfo = Directory.CreateDirectory("tempSupplementaryFiles");
                            Dictionary<Uri, string> files = new Dictionary<Uri, string>();
                            Identifier aasIdentifier = null;
                            FileInfo thumbnail = null;

                            using (AASX aasx = new AASX(opts.InputFileName))
                            {
                                AssetAdministrationShellEnvironment_V2_0 env = aasx.GetEnvironment_V2_0();

                                ModifyEnvironment(env);

                                aasIdentifier = env.AssetAdministrationShells.First().Identification;
                                env.WriteEnvironment_V2_0(ExportType.Xml, tempEnvironmentFile);

                                foreach (var file in aasx.SupplementaryFiles)
                                {
                                    using (Stream stream = file.GetStream(FileMode.Open, FileAccess.Read))
                                    {
                                        string[] splitted = file.Uri.ToString().Split(new char[] { '/' });
                                        string fileName = splitted[splitted.Length - 1];
                                        string filePath = Path.Combine(tempDirInfo.FullName, fileName);

                                        using (FileStream dest = File.Open(filePath, FileMode.OpenOrCreate))
                                            stream.CopyTo(dest);

                                        files.Add(file.Uri, filePath);
                                    }
                                }
                                PackagePart thumbnailPackagePart = aasx.GetThumbnailAsPackagePart();
                                if (thumbnailPackagePart != null)
                                {
                                    string thumbnailFilePath = tempDirInfo.FullName + thumbnailPackagePart.Uri.ToString();
                                    thumbnail = thumbnailPackagePart.GetStream(FileMode.Open, FileAccess.Read).ToFile(thumbnailFilePath);
                                }
                            }

                            using (AASX aasx_new = new AASX(opts.OutputFileName))
                            {
                                aasx_new.AddEnvironment(aasIdentifier, tempEnvironmentFile);
                                foreach (var file in files)
                                {
                                    aasx_new.AddFileToAASX(file.Key.ToString(), file.Value);
                                }
                                if (thumbnail != null)
                                    aasx_new.AddThumbnail(thumbnail.FullName);
                            }

                            if (opts.Revalidate)
                            {
                                logger.Info($"Validate {opts.OutputFileName}...");
                                using (AASX aasx_new = new AASX(opts.OutputFileName))
                                {
                                    AssetAdministrationShellEnvironment_V2_0 env = aasx_new.GetEnvironment_V2_0();
                                }
                            }
                            //Clear all temporary ressources
                            Directory.Delete(tempDirInfo.FullName, true);
                            File.Delete(tempEnvironmentFile);

                            return 0;
                        }
                        catch (Exception e)
                        {
                            logger.Error(e, e.Message);
                            return -1;
                        }
                    }
                default:
                    logger.Error($"Unable to parse file extension ${fileExtension}");
                    return -1;
            }
        }

        private static void ModifyEnvironment(AssetAdministrationShellEnvironment_V2_0 env)
        {
            foreach (var cd in env.EnvironmentConceptDescriptions)
            {
                foreach (var cdemb in cd.EmbeddedDataSpecifications)
                {
                    if (cdemb.DataSpecification?.Keys?.Count == 0)
                        cdemb.DataSpecification = new EnvironmentReference_V2_0()
                        {
                            Keys = new List<EnvironmentKey_V2_0>()
                            {
                                new EnvironmentKey_V2_0()
                                {
                                    IdType = KeyType_V2_0.IRI,
                                    Local = false,
                                    Type = KeyElements_V2_0.GlobalReference,
                                    Value = "http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0"
                                }
                            }
                        };

                    if (cdemb.DataSpecificationContent?.DataSpecificationIEC61360?.PreferredName?.Count == 0)
                        cdemb.DataSpecificationContent.DataSpecificationIEC61360.PreferredName = null;
                }
            }
        }
    }

    
}
