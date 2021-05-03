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
using System;
using System.IO;

namespace BaSyx.Utils.FileHandling
{
    public delegate void FileSystemChanged(string fullPath, WatcherChangeTypes type);
    
    public class FileWatcher : IDisposable
    {
        private readonly FileSystemChanged FileChangedHandler;
        private bool disposedValue;

        public FileSystemWatcher FileSystemWatcher { get; }
        public string ObservedDirectory { get; }

        public FileWatcher(string fileOrDirectory, FileSystemChanged fileSystemChanged, string filter = null)
        {
            if (string.IsNullOrEmpty(fileOrDirectory))
                throw new ArgumentNullException(nameof(fileOrDirectory));
            if(!File.Exists(fileOrDirectory) && !Directory.Exists(fileOrDirectory))
                throw new InvalidOperationException($"File or Directory {fileOrDirectory} does not exist");
            if (fileSystemChanged == null)
                throw new ArgumentNullException(nameof(fileSystemChanged));

            ObservedDirectory = fileOrDirectory;
            FileChangedHandler = fileSystemChanged;

            if (string.IsNullOrEmpty(filter) && File.Exists(fileOrDirectory))
                filter = fileOrDirectory;
            else
                filter = "*.*";

            FileSystemWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(fileOrDirectory),
                Filter = filter,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Size
            };
            FileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
            FileSystemWatcher.Deleted += new FileSystemEventHandler(OnChanged);
            FileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            
            FileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            FileChangedHandler(e.FullPath, e.ChangeType);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FileSystemWatcher.EnableRaisingEvents = false;
                    FileSystemWatcher.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
