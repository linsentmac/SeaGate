 //-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.IO;

using UnityEngine;

namespace LARSuite {
    /// <summary>
    /// File Manager.
    /// </summary>
    public class FileManager : Singleton<FileManager> {
        /// <summary>
        /// Init global settings, must be called on main thread.
        /// </summary>
        public void Init() {
            if (PersistentDataPath == null) {
                PersistentDataPath = Application.persistentDataPath + "/Lotus/";
            }

            if (TemporaryCachePath == null) {
                TemporaryCachePath = Application.temporaryCachePath + "/Lotus/";
            }
        }

        /// <summary>
        /// A folder to save persistant data.
        /// </summary>
        public string PersistentDataPath { get; private set; }

        /// <summary>
        /// A folder to save temporary data.
        /// </summary>
        public string TemporaryCachePath { get; private set; }

        /// <summary>
        /// Private Constructor. Obey the Singleton pattern.
        /// </summary>
        private FileManager() { }

        /// <summary>
        /// Open file at a given path, create directory if the path 
        /// does not exist in "Create" mode.
        /// </summary>
        /// <param name="path">File path to be open.</param>
        /// <param name="mode">Open mode.</param>
        public Stream OpenFile(string path, FileMode mode) {
            if(mode == FileMode.Create || mode == FileMode.OpenOrCreate) {
                if (!Directory.Exists(Path.GetDirectoryName(path))) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
            }

            return File.Open(path, mode);
        }

        /// <summary>
        /// Delete file.
        /// </summary>
        /// <param name="path">File path to be delete.</param>
        public void DeleteFile(string path) {
            File.Delete(path);
        }

    }
}
