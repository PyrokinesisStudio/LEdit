using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace ActionRunner
{
    public struct IndexedFiles
    {
        public string path;
        public byte[] hash;
    };

    public class Index
    {
        public static List<IndexedFiles> indexedFileList = new List<IndexedFiles>();
        public static List<String> indexedDirectoriesList = new List<String>();

        public static void IndexFiles(string f)
        {
            foreach (string file in Directory.GetFiles(f))
            {
                using (var stream = File.OpenRead(file))
                {
                    SHA1 sha = new SHA1CryptoServiceProvider();
                    indexedFileList.Add(new IndexedFiles
                    {
                        path = file,
                        hash = sha.ComputeHash(stream)
                    });
                }
            }
            foreach (string sdir in Directory.GetDirectories(f))
            {
                try
                {
                    indexedDirectoriesList.Add(sdir);
                    IndexFiles(sdir);
                }
                catch
                {
                    // Do Nothing
                }
            }
        }
    }

    public class ScanRunner
    {
        public static List<String> newFiles = new List<String>();
        public static List<String> newFolders = new List<String>();

        public static List<String> editedFiles = new List<String>();

        public static void CheckForNewHash(string f) 
        {
            foreach (string file in Directory.GetFiles(f)) // Checks the files
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                bool fileExists = false;
                bool fileEdited = false;

                // goes through every indexed file in the indexed file list and checks it against the current file above
                foreach (IndexedFiles indexedFile in Index.indexedFileList)
                {
                    if (file == indexedFile.path)
                    {
                        // The file exists
                        fileExists = true;
                        // Opens a file stream
                        using (var stream = File.OpenRead(file))
                        {
                            if (sha.ComputeHash(stream) != indexedFile.hash)
                            {
                                // The file has been edited
                                fileEdited = true;
                            }
                        }
                    }

                    // Adds the appropriate items to the lists declared above
                    if (fileExists == false)
                    {
                        newFiles.Add(file);
                    } else if (fileEdited == true)
                    {
                        editedFiles.Add(file);
                    }
                }
            }
            foreach (string sdir in Directory.GetDirectories(f)) // Checks the directories
            {
                try
                {
                    bool directoryExists = false; 

                    // Goes through every file in the indexed directories list and checks if it exists against sdir 
                    foreach (string indexedDir in Index.indexedDirectoriesList)
                    {
                        if (indexedDir == sdir)
                        {
                            directoryExists = true;
                        }
                    }
                    
                    if (directoryExists == false)
                    {
                        newFolders.Add(sdir);
                    }

                    CheckForNewHash(sdir);
                }
                catch
                {
                    // Do Nothing
                }
            }
        }
    }
}

