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
using System;
using System.IO;

namespace BaSyx.Utils.Settings
{
    public class FileWatcher
    {
        private readonly FileSystemWatcher fileSystemWatcher;
        private readonly string filePath;

        public delegate void FileChanged(string fullPath);
        private readonly FileChanged FileChangedHandler;

        public FileWatcher(string pathToFile, FileChanged fileChanged)
        {
            if (string.IsNullOrEmpty(pathToFile))
                throw new ArgumentNullException("pathToFile");
            else if (!File.Exists(pathToFile))
                throw new InvalidOperationException(pathToFile + "does not exist");

            filePath = pathToFile;
            FileChangedHandler = fileChanged;

            fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = Path.GetDirectoryName(pathToFile);
            fileSystemWatcher.Filter = Path.GetFileName(pathToFile);
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;

            fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);

            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.Out.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            if (e.ChangeType == WatcherChangeTypes.Changed && filePath == e.FullPath)
                FileChangedHandler(e.FullPath);
        }
    }
}
