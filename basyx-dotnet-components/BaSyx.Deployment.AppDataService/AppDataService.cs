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
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.Settings;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace BaSyx.Deployment.AppDataService
{

    /// <summary>
    /// The AppDataService provides functionalities to configure your Applications
    /// </summary>
    public class AppDataService : IDisposable
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<AppDataService>();

		public static bool IsVirtual {get; private set;} = false;
		public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsX86 => RuntimeInformation.OSArchitecture == Architecture.X86;
        public static bool IsX64 => RuntimeInformation.OSArchitecture == Architecture.X64;
        public static bool IsArm64 => RuntimeInformation.OSArchitecture == Architecture.Arm64;
        public static bool IsArm => RuntimeInformation.OSArchitecture == Architecture.Arm;
        public static bool IsAzure => Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%") != "%WEBSITE_HOSTNAME%"; 

        public static readonly string DEFAULT_WORKING_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory;
        
        public AppDataContext AppDataContext { get; set; }

        /// <summary>
        /// Indicates whether the application is snapped or not (running inside linux snappy environment)
        /// </summary>
        /// <returns></returns>
        public static bool IsSnapped => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SNAP"));

        /// <summary>
        /// Gets the SNAP_COMMON location
        /// </summary>
        public static string SnapCommonLocation => Environment.GetEnvironmentVariable("SNAP_COMMON");

        /// <summary>
        /// Gets the SNAP_DATA location
        /// </summary>
        public static string SnapDataLocation => Environment.GetEnvironmentVariable("SNAP_DATA");

        private bool disposedValue;

        /// <summary>
        /// Gets the base storage location for the application
        /// Snapped (Linux): $SNAP_COMMON/solutions/activeConfiguration/{{appName}}
        /// Windows: Current directory + default relative path (optional)
        /// </summary>
        public string BaseStorageLocation => GetBaseStorageLocation(_settings.ServiceConfig.AppName);

        public Action LoadAction { get; set; }
        public Action SaveAction { get; set; }
        public IConfiguration Configuration { get; private set; }

        private readonly AppDataServiceSettings _settings;

        public static AppDataService Singleton { get; set; }

        public AppDataService(AppDataServiceSettings settings)
        {
            _settings = settings ?? AppDataServiceSettings.LoadSettings();

            EnsureStorageLocation();

            AppDataContext = new AppDataContext()
            {
                HostName = Dns.GetHostName(),
                IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
                IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
                OSArchitecture = RuntimeInformation.OSArchitecture,
                TimeStamp = DateTime.Now,
                Settings = new Dictionary<string, Settings>(),
                Files = new Dictionary<string, string>()
            };

            Singleton = this;
        }     

        private static string GetBaseStorageLocation(string appName)
        {
            return IsSnapped ?
			    Path.Combine(SnapCommonLocation, "solutions", "activeConfiguration", appName) :
			    Environment.CurrentDirectory;
		}

        private AppDataService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static AppDataService Create(string appName, string settingsJsonFile = "appsettings.json", string[] cmdLineArgs = null)
        {
			SetDefaultWorkingDirectory();
			IConfiguration configuration = LoadConfiguration(appName, settingsJsonFile, cmdLineArgs);
            return Create(configuration);
        }

        public static AppDataService Create(IConfiguration configuration)
        {
            try
            {
                SetDefaultWorkingDirectory();
                AppDataServiceSettings appDataServiceSettings = configuration.GetSection("AppDataServiceSettings").Get<AppDataServiceSettings>();
                AppDataService appDataService = new AppDataService(appDataServiceSettings);
                AppDataService.IsVirtual = appDataServiceSettings.ServiceConfig.IsVirtual;
                appDataService.LoadConfiguration(configuration);
                appDataService.LoadAction = RestartAppAction();
                var started = appDataService.Start();
                if (!started.Success)
                    throw new Exception("Error starting AppDataService");
                return appDataService;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error starting AppDataService");
                throw;
            }
        }

		public static IConfiguration LoadConfiguration(string appName, string settingsJsonFile = "appsettings.json", string[] cmdLineArgs = null)
		{
			string baseStorageLocation = GetBaseStorageLocation(appName);
			string settingsFilePath = GetOrCreateTargetFilePath(settingsJsonFile, baseStorageLocation);

			var config = new ConfigurationBuilder();
			config.AddCommandLine(cmdLineArgs);
			config.AddEnvironmentVariables();
			config.AddJsonFile(settingsFilePath, false, true);
			return config.Build();
		}

		public void LoadConfiguration(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public T GetValue<T>(string key)
        {
            return Configuration.GetValue<T>(key);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            value = Configuration.GetValue<T>(key);
            return value != null;
        }
  
        public bool LoadSettings<T>(string key) where T: Settings
        {
            AddSettings(typeof(T), key);
            return AppDataContext.Settings.ContainsKey(typeof(T).Name);
        }        

        public Settings GetSettings(Type type, string key = null)
        {
            if (AppDataContext.Settings.TryGetValue(type.Name, out var settings))
                return settings;
            else
            {
                AddSettings(type, key);
                if (AppDataContext.Settings.TryGetValue(type.Name, out var secondAttemptSettings))
                    return secondAttemptSettings;
            }
            return default;
        }

        public T GetSettings<T>(string key = null) where T : Settings
        {
            return (T)GetSettings(typeof(T), key);
        }

        public Settings GetXmlSettings(Type type)
        {
            if (AppDataContext.Settings.TryGetValue(type.Name + ".xml", out var settings))
                return settings;
            else
            {
                AddXmlSettings(type);
                if (AppDataContext.Settings.TryGetValue(type.Name + ".xml", out var secondAttemptSettings))
                    return secondAttemptSettings;
            }
            return default;
        }

        public T GetXmlSettings<T>() where T : Settings
        {
           return (T)GetXmlSettings(typeof(T));
        }

        public string GetFilePath(string fileName)
        {
            if (AppDataContext.Files.TryGetValue(fileName, out string filePath))
                return filePath;
            else
                return null;
        }

        public void AddSettings(Type settingsType, string key = null)
        {
            Settings settings = ReadSettings(settingsType, key);
			AppDataContext.Settings.Add(settingsType.Name, settings);
		}

		public T ReadSettings<T>(string key = null) where T : Settings
		{
			return (T)ReadSettings(typeof(T), key);
		}

		public Settings ReadSettings(Type settingsType, string key = null)
		{
			if (string.IsNullOrEmpty(key))
				key = settingsType.Name;

			Settings settings = (Settings)Configuration.GetSection(key).Get(settingsType);		
			logger.LogInformation($"{settingsType.Name} loaded successfully");
            return settings;
		}

		public void AddXmlSettings(Type settingsType)
        {
            string settingsFileName = settingsType.Name + ".xml";
            string filePathToReadFrom = GetOrCreateTargetFilePath(settingsFileName, BaseStorageLocation);

            Settings settings = Settings.LoadSettingsFromFile(filePathToReadFrom, settingsType);
            AppDataContext.Settings.Add(settingsFileName, settings);
            AppDataContext.Files.Add(settingsFileName, filePathToReadFrom);
            logger.LogInformation($"File {settingsFileName} loaded successfully");
        }        

        public void AddFile(string fileName)
        {
            string filePathToReadFrom = GetOrCreateTargetFilePath(fileName, BaseStorageLocation);
            AppDataContext.Files.Add(fileName, filePathToReadFrom);
            logger.LogInformation($"File {filePathToReadFrom} loaded successfully");
        }

        private static string GetOrCreateTargetFilePath(string fileName, string baseStorageLocation)
        {
			fileName = Path.GetFileName(fileName);

			string[] destinationFiles = Directory.Exists(baseStorageLocation) ? 
                Directory.GetFiles(baseStorageLocation, fileName, SearchOption.AllDirectories) : new string[0];
			string filePathToReadFrom = null;

			if (destinationFiles.Length == 0)
			{
				logger.LogInformation($"{fileName} not found at {baseStorageLocation}");
				string[] sourceFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, fileName, SearchOption.AllDirectories);
				if (sourceFiles.Length == 0)
					throw new Exception($"{fileName} not found within {AppDomain.CurrentDomain.BaseDirectory}");
				else if (sourceFiles.Length > 1)
					throw new Exception($"{fileName} found multiple times in {AppDomain.CurrentDomain.BaseDirectory}");
				else
				{
					string sourceFilePath = sourceFiles[0];
					string relativeSourceFilePath = sourceFilePath.Replace(AppDomain.CurrentDomain.BaseDirectory, string.Empty);
					string destinationFilePath = Path.Combine(baseStorageLocation, relativeSourceFilePath);
					try
					{
						string folderPath = Path.GetDirectoryName(destinationFilePath);
						Directory.CreateDirectory(folderPath);
						File.Copy(sourceFilePath, destinationFilePath, true);
						filePathToReadFrom = destinationFilePath;
						logger.LogInformation($"File {destinationFilePath} copied successfully");
					}
					catch (Exception e)
					{
						logger.LogError(e, $"Unable to copy file to {destinationFilePath}");
					}
				}
			}
			else if (destinationFiles.Length > 1)
			{
				logger.LogWarning($"Multiple files found: {string.Join(";", destinationFiles)}");
				throw new Exception($"{fileName} found multiple times in {baseStorageLocation} | {string.Join(";", destinationFiles)}");
			}
			else
				filePathToReadFrom = destinationFiles[0];
            return filePathToReadFrom;
		}
        

        public Result Start()
        {
            if (!IsSnapped)
                return new Result(true, new InfoMessage("Not snapped - skipping AppDataListener"));

            try
            {
                while (!Directory.Exists(BaseStorageLocation))
                {
                    logger.LogInformation("Waiting for active-solution interface...");
                    Thread.Sleep(1000);
                }
                // Check if the process is running inside a snap 
                logger.LogInformation($"Running inside snap: {IsSnapped}");

                logger.LogInformation("AppDataListener started successfully");
                return new Result(true);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, $"Start failed");
                return new Result(exc);
            }            
        }

        public Result Stop()
        {
            return new Result(true);
        }

        internal Result Load()
        {
            logger.LogInformation($"Storage location for configuration files: {BaseStorageLocation}");

            if (!EnsureStorageLocation().Success)
                return new Result(false, new ErrorMessage($"Unable to load configuration"));

            try
            {
                LoadAction?.Invoke();
                logger.LogInformation($"Load action completed succesfully");
                return new Result(true);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, $"Load action failed");
                return new Result(exc);
            }
        }

        internal Result Save()
        {
            if (!EnsureStorageLocation().Success)
                return new Result(false, new ErrorMessage("Unable to save configuration"));
           
            try
            {
                SaveAction?.Invoke();
                logger.LogInformation($"Save action completed succesfully");
                return new Result(true);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, $"Save function failed");
                return new Result(exc);
            }
        }

        private Result EnsureStorageLocation()
        {
            if (!Directory.Exists(BaseStorageLocation))
            {
                try
                {
                    Directory.CreateDirectory(BaseStorageLocation);
                    logger.LogInformation($"Created storage location: '{BaseStorageLocation}'");
                }
                catch (Exception exc)
                {
                    string errMsg = $"Creating storage location '{BaseStorageLocation}' failed!";
                    logger.LogError(exc, errMsg);
                    return new Result(exc);
                }
            }

            return new Result(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public static void SetDefaultWorkingDirectory()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            logger.LogInformation($"Program launched from (Environment.CurrentDirectory): {Environment.CurrentDirectory}");
            logger.LogInformation($"Program physical location (AppDomain.CurrentDomain.BaseDirectory) : {AppDomain.CurrentDomain.BaseDirectory}");
            logger.LogInformation($"Program base directory (AppContext.BaseDirectory): {AppContext.BaseDirectory}");
            logger.LogInformation($"Program process path (Environment.ProcessPath): {Environment.ProcessPath}");
        }

        public static Action RestartAppAction(int milliseconds = 30000)
        {
            return new Action(() =>
                Task.Run(async () =>
                {
                    await Task.Delay(milliseconds);
                    Environment.ExitCode = 0;
                    Environment.Exit(Environment.ExitCode);
                }));
        }        
    }
}
