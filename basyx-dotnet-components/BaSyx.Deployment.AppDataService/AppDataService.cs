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

namespace BaSyx.Deployment.AppDataService
{

    /// <summary>
    /// The AppDataService provides functionalities to configure your Applications
    /// </summary>
    public class AppDataService : IDisposable
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<AppDataService>();

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
        public string BaseStorageLocation => IsSnapped ?
            Path.Combine(SnapCommonLocation, "solutions", "activeConfiguration", _settings.ServiceConfig.AppName) :
            Environment.CurrentDirectory;

        public Action LoadAction { get; set; }
        public Action SaveAction { get; set; }

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

        public Settings GetSettings(Type type)
        {
            if (AppDataContext.Settings.TryGetValue(type.Name + ".xml", out var settings))
                return settings;
            else
            {
                AddSettings(type);
                if (AppDataContext.Settings.TryGetValue(type.Name + ".xml", out var secondAttemptSettings))
                    return secondAttemptSettings;
            }
            return default;
        }

        public T GetSettings<T>() where T : Settings
        {
           return (T)GetSettings(typeof(T));
        }

        public string GetFilePath(string fileName)
        {
            if (AppDataContext.Files.TryGetValue(fileName, out string filePath))
                return filePath;
            else
                return null;
        }

        public void AddSettings(Type settingsType)
        {
            string settingsFileName = settingsType.Name + ".xml";
            string[] destinationFiles = Directory.GetFiles(BaseStorageLocation, settingsFileName, SearchOption.AllDirectories);
            string filePathToReadFrom = null;

            if (destinationFiles.Length == 0)
            {
                logger.LogInformation($"{settingsFileName} not found at {BaseStorageLocation}");
                string[] sourceFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, settingsFileName, SearchOption.AllDirectories);
                if (sourceFiles.Length == 0)
                    throw new Exception($"{settingsFileName} not found within {AppDomain.CurrentDomain.BaseDirectory}");
                else if (sourceFiles.Length > 1)
                    throw new Exception($"{settingsFileName} found multiple times in {AppDomain.CurrentDomain.BaseDirectory}");
                else
                {
                    string sourceFilePath = sourceFiles[0];
                    string relativeSourceFilePath = sourceFilePath.Replace(AppDomain.CurrentDomain.BaseDirectory, string.Empty);
                    string destinationFilePath = Path.Combine(BaseStorageLocation, relativeSourceFilePath);
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
                throw new Exception($"{settingsFileName} found multiple times in {BaseStorageLocation} | {string.Join(";", destinationFiles)}");
            }
            else
                filePathToReadFrom = destinationFiles[0];

            Settings settings = Settings.LoadSettingsFromFile(filePathToReadFrom, settingsType);
            AppDataContext.Settings.Add(settingsFileName, settings);
            AppDataContext.Files.Add(settingsFileName, filePathToReadFrom);
            logger.LogInformation($"File {settingsFileName} loaded successfully");
        }        

        public void AddFile(string fileName)
        {
            fileName = Path.GetFileName(fileName);

            string[] destinationFiles = Directory.GetFiles(BaseStorageLocation, fileName, SearchOption.AllDirectories);
            string filePathToReadFrom = null;

            if (destinationFiles.Length == 0)
            {
                logger.LogInformation($"{fileName} not found at {BaseStorageLocation}");
                string[] sourceFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, fileName, SearchOption.AllDirectories);
                if (sourceFiles.Length == 0)
                    throw new Exception($"{fileName} not found within {AppDomain.CurrentDomain.BaseDirectory}");
                else if (sourceFiles.Length > 1)
                    throw new Exception($"{fileName} found multiple times in {AppDomain.CurrentDomain.BaseDirectory}");
                else
                {
                    string sourceFilePath = sourceFiles[0];
                    string relativeSourceFilePath = sourceFilePath.Replace(AppDomain.CurrentDomain.BaseDirectory, string.Empty);
                    string destinationFilePath = Path.Combine(BaseStorageLocation, relativeSourceFilePath);
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
                throw new Exception($"{fileName} found multiple times in {BaseStorageLocation} | {string.Join(";", destinationFiles)}");
            }
            else
                filePathToReadFrom = destinationFiles[0];

            AppDataContext.Files.Add(fileName, filePathToReadFrom);
            logger.LogInformation($"File {filePathToReadFrom} loaded successfully");
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
            //if (!Debugger.IsAttached)
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
