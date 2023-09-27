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
using BaSyx.Utils.Assembly;
using BaSyx.Utils.Extensions;
using BaSyx.Utils.FileSystem;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings
{
    public static class SettingsExtensions
    {
        public static T As<T>(this Settings settings) where T : Settings
        {
            return settings as T;
        }
    }

    public abstract class Settings : ISettings
    {
        [XmlIgnore]
        public string Name => this.GetType().Name;
        [XmlIgnore]
        public string FilePath { get; set; }
        [XmlIgnore]
        public Dictionary<string, string> Miscellaneous { get; set; } = new Dictionary<string, string>();
        [XmlElement]
        public ServiceType OperationMode { get; set; }
        [XmlElement(IsNullable = true)]
        public ServerConfiguration ServerConfig { get; set; } = new ServerConfiguration();
        [XmlElement(IsNullable = true)]
        public ClientConfiguration ClientConfig { get; set; } = new ClientConfiguration();
        [XmlElement(IsNullable = true)]
        public PathConfiguration PathConfig { get; set; } = new PathConfiguration();
        [XmlElement(IsNullable = true)]
        public ProxyConfiguration ProxyConfig { get; set; } = new ProxyConfiguration();

        [XmlElement]
        public string ExecutionPath
        {
            get
            {
                if (_executionPath == null)
                    _executionPath = Environment.CurrentDirectory;
                return _executionPath;
            }
            set 
            {
                _executionPath = value.ReplaceWithEnvironmentVariable();
            }
        }

        public static string WorkingDirectory => Environment.CurrentDirectory;

        public const string FileExtension = ".xml";
        public const string MiscellaneousConfig = "Miscellaneous";

        public static SettingsCollection SettingsCollection { get; } = new SettingsCollection();

        private static readonly ILogger logger = LoggingExtentions.CreateLogger<Settings>();
       
        private FileWatcher _fileWatcher;
        private string _executionPath;

        protected Settings()
        { }

        public static void AutoLoadSettings(string settingsSuffix = "*Settings.xml")
        {
            string[] files = Directory.GetFiles(WorkingDirectory, settingsSuffix, SearchOption.TopDirectoryOnly);
            if (files?.Length > 0)
            {
                List<System.Reflection.Assembly> assemblies = AssemblyUtils.GetLoadedAssemblies();

                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        XDocument doc = XDocument.Load(files[i]);
                        string rootName = doc.Root.Name.LocalName;
                        IEnumerable<Type> settingsTypes = assemblies
                            .SelectMany(a => a.GetTypes())
                            .Where(t => t.Name == rootName);

                        if (settingsTypes?.Count() > 0)
                            foreach (var settingsType in settingsTypes)
                            {
                                try
                                {
                                    Settings setting = LoadSettingsFromFile(files[i], settingsType);
                                    if (setting != null)
                                        SettingsCollection.Add(setting);
                                    else
                                        throw new InvalidOperationException("LoadSettingsFromFile returned null");
                                }
                                catch (Exception exp)
                                {
                                    logger.LogInformation(exp, "Cannot load settings of type: " + rootName + " because type is either never used or not referenced");
                                    continue;
                                }
                            }
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e, "Cannot load settings file: " + files[i]);
                    }
                }
            }
        }

        public virtual void ConfigureSettingsWatcher(FileSystemChanged settingsFileChangedHandler)
        {
            _fileWatcher = new FileWatcher(FilePath, settingsFileChangedHandler);
        }       

        public static Settings LoadSettingsFromFile(string filePath, Type settingsType)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                logger.LogWarning("Settings file does not exist: " + filePath);
                return null;
            }

            try
            {
                Settings settings = null;

                XmlSerializer serializer = new XmlSerializer(settingsType);
                using(FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    settings = (Settings)serializer.Deserialize(stream);

                if (settings != null)
                {
                    var miscElement = XElement.Load(filePath).Element(MiscellaneousConfig);
                    if (miscElement != null)
                        settings.Miscellaneous = miscElement.Elements().Where(e => !e.HasElements).ToDictionary(e => e.Name.LocalName, e => e.Value);

                    settings.FilePath = filePath;

                    if(logger.IsEnabled(LogLevel.Debug))
                        logger.LogDebug("Settings loaded: " + JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented));

                    return settings;
                }
                logger.LogWarning("No settings of Type " + settingsType.Name + " loaded: " + filePath);
                return null;

            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not load " + filePath);
                return null;
            }
        } 
        
        public static T LoadSettingsFromFile<T>(string filePath) where T : Settings, new()
        {
            return (T)LoadSettingsFromFile(filePath, typeof(T));
        }

        public static T LoadSettingsByName<T>(string name) where T : Settings
        {
            Settings settings = SettingsCollection.Find(s => s.Name == typeof(T).Name);
            if (settings != null)
                return (T)settings;
            return null;
        }

        public void SaveSettings(string filePath, Type settingsType)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(settingsType);
                using(FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    serializer.Serialize(stream, this);

                FilePath = filePath;

                logger.LogInformation("Settings saved: " + filePath);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not serialize to " + filePath);
            }
        }
    }

    public abstract class Settings<T> : Settings where T : Settings, new()
    {
        public static string FileName => typeof(T).Name + FileExtension;
     
        protected Settings() : base()
        { }

        public static T CreateSettings() => new T();
       
        public void SaveSettings() => SaveSettings(FilePath);

        public void SaveSettings(string filePath) => SaveSettings(filePath, typeof(T));          

        public static T LoadSettings()
        {
            Settings settings = LoadSettingsByName(typeof(T).Name);
            if (settings == null)
            {
                string settingsFilePath = Path.Combine(WorkingDirectory, FileName);
                settings = LoadSettingsFromFile(settingsFilePath);
            }
            if(settings != null)
                return (T)settings;
            return null;
        }

        public static T LoadSettingsByName(string name) => LoadSettingsByName<T>(name);
        public static T LoadSettingsFromFile(string filePath) => LoadSettingsFromFile<T>(filePath);
    }
}
