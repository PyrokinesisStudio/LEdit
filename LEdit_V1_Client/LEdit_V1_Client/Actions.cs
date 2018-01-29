using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

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
                    indexedFileList.Add(new IndexedFiles
                    {
                        path = file,
                        hash = FileMgmt.Manager.CheckFileHash(file)
                    });
                    //Console.WriteLine("File: " + file);
                    //Console.WriteLine(BitConverter.ToString(CheckFileHash(file)));
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
        public static void ChangeScanner()
        {
            while (filesToDelete.Count == 0 && foldersToDelete.Count == 0 && newFiles.Count == 0 && editedFiles.Count == 0 && newFolders.Count == 0)
            {
                CheckForNewFilesFolders(Misc.Config.fullProjectPath);

                CheckFileFolderDelete(Misc.Config.fullProjectPath);
                System.Threading.Thread.Sleep(150);
            }

            Console.WriteLine("Change Detected");

            // Just saying, the order of this is vital to its functionality
            // Due to the way the renaming works

            if (newFiles.Count > 0)
            {
                while (Misc.Global.pauseLiveUpdate)
                {
                    Thread.Sleep(100);
                }
                foreach (string file in newFiles)
                {
                    string uploadPath = file.Substring(file.IndexOf(Misc.Config.projectFolder) + Misc.Config.projectFolder.Length + 1);
                    string newPath = file;
                    if (file.Contains(" "))
                    {
                        string replacedUploadPath = uploadPath.Replace(" ", "_");
                        string substr = file.Substring(0, file.IndexOf(Misc.Config.projectFolder) + Misc.Config.projectFolder.Length + 1);
                        newPath = substr + replacedUploadPath;
                        FileMgmt.Manager.MoveFile(file, newPath);
                        uploadPath = replacedUploadPath;
                    }

                    string text = FileMgmt.Manager.ReadFile(newPath);
                    if (text == "" || text == null)
                    {
                        text = "// This file is empty";
                    }

                    Console.WriteLine("Detected new file: " + file);
                    Misc.Global.connectionSocket.Send($"CreateNewFile {Misc.Userdata.Username} {Misc.Userdata.Password} {uploadPath} {text}");
                    FileStream fs = new FileStream(newPath, FileMode.Open);
                    fs.Close();
                    Handler.MessageHandler.AppListener(UploadListener);

                    void UploadListener(object sender, WebSocketSharp.MessageEventArgs e)
                    {
                        if (e.Data == "True")
                        {
                            Console.WriteLine("Complete");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Failed (1) file transfer: " + e.Data);
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Handler.MessageHandler.CloseListener(UploadListener);
                    }
                }
                newFiles.Clear();
            }

            if (editedFiles.Count > 0)
            {
                foreach (string file in editedFiles)
                {
                    string uploadPath = file.Substring(file.IndexOf(Misc.Config.projectFolder) + Misc.Config.projectFolder.Length + 1);
//                    uploadPath = uploadPath.Replace(" ", "_"); -- shouldn't need this

                    Console.WriteLine("Detected modified file: " + file);
                    string data = "//EMPTY";
                    if (FileMgmt.Manager.ReadFile(file) != "" && FileMgmt.Manager.ReadFile(file) != null)
                    {
                        data = FileMgmt.Manager.ReadFile(file);
                    } 

                    Misc.Global.connectionSocket.Send($"UploadFileData {Misc.Userdata.Username} {Misc.Userdata.Password} {uploadPath} {data}");

                    Handler.MessageHandler.AppListener(UploadListener);

                    void UploadListener(object sender, WebSocketSharp.MessageEventArgs e)
                    {
                        if (e.Data == "True")
                        {
                            Console.WriteLine("Complete");
                        } else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Failed (1) file transfer: " + e.Data);
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Handler.MessageHandler.CloseListener(UploadListener);
                    }
                }
                editedFiles.Clear();
            }

            if (newFolders.Count > 0)
            {
                foreach (string folder in newFolders)
                {
                    string uploadPath = folder.Substring(folder.IndexOf(Misc.Config.projectFolder) + Misc.Config.projectFolder.Length + 1);
                    if (folder.Contains(" "))
                    {
                        string replacedUploadPath = uploadPath.Replace(" ", "_");
                        string substr = folder.Substring(0, folder.IndexOf(Misc.Config.projectFolder) + Misc.Config.projectFolder.Length + 1);
                        string fullPath = substr + replacedUploadPath;
                        Directory.Move(folder,  fullPath);
                        uploadPath = replacedUploadPath;
                    }

                    Console.WriteLine("Detected new folder: " + folder);
                    Misc.Global.connectionSocket.Send($"CreateNewFolder {Misc.Userdata.Username} {Misc.Userdata.Password} {uploadPath}");
                    Handler.MessageHandler.AppListener(UploadListener);
                    void UploadListener(object sender, WebSocketSharp.MessageEventArgs e)
                    {
                        if (e.Data == "True")
                        {
                            Console.WriteLine("Complete");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Failed (1) file transfer: " + e.Data);
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Handler.MessageHandler.CloseListener(UploadListener);
                    }
                }
                newFolders.Clear();
            }

            if (filesToDelete.Count > 0)
            {
                foreach (string file in filesToDelete)
                {
                    string uploadPath = file.Substring(file.IndexOf(Misc.Config.projectFolder) + Misc.Config.projectFolder.Length + 1);
                    Console.WriteLine("Detected deleted file: " + file);
                    Misc.Global.connectionSocket.Send($"DeleteFile {Misc.Userdata.Username} {Misc.Userdata.Password} {uploadPath}");
                    Handler.MessageHandler.AppListener(UploadListener);

                    void UploadListener(object sender, WebSocketSharp.MessageEventArgs e)
                    {
                        if (e.Data == "True")
                        {
                            Console.WriteLine("Complete");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Failed (1) file transfer: " + e.Data);
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Handler.MessageHandler.CloseListener(UploadListener);
                    }
                }
                filesToDelete.Clear();
            }

            if (foldersToDelete.Count > 0)
            {
                foreach (string folder in foldersToDelete)
                {
                    string uploadPath = folder.Substring(folder.IndexOf(Misc.Config.projectFolder) + Misc.Config.projectFolder.Length + 1);
                    Console.WriteLine("Detected deleted folder: " + folder);
                    Misc.Global.connectionSocket.Send($"DeleteFolder {Misc.Userdata.Username} {Misc.Userdata.Password} {uploadPath}");
                    Handler.MessageHandler.AppListener(UploadListener);

                    void UploadListener(object sender, WebSocketSharp.MessageEventArgs e)
                    {
                        if (e.Data == "True")
                        {
                            Console.WriteLine("Complete");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Failed (1) file transfer: " + e.Data);
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Handler.MessageHandler.CloseListener(UploadListener);
                    }
                }
                foldersToDelete.Clear();
            }

            newFileIndex.Clear();
            newFolderIndex.Clear();
            Index.indexedFileList.Clear();
            Index.indexedDirectoriesList.Clear();
            Index.IndexFiles(Misc.Config.fullProjectPath);

            ChangeScanner();
        }

        public static List<String> filesToDelete = new List<String>();
        public static List<String> foldersToDelete = new List<String>();

        public static List<String> newFiles = new List<String>();
        public static List<String> newFolders = new List<String>();

        public static List<String> editedFiles = new List<String>();

        public static List<String> newFileIndex = new List<String>();
        public static List<String> newFolderIndex = new List<String>();

        // CAN ONLY BE RAN AFTER INITIAL INDEX
        public static void CheckForNewFilesFolders(string f) 
        {
            foreach (string file in Directory.GetFiles(f)) // Checks the files
            {
                newFileIndex.Add(file); // Add to the new file index
                bool fileExists = false; 
                bool fileEdited = false;

                // Goes through every indexed file in the indexed file list and checks it against the current file above
                try
                {
                    foreach (IndexedFiles indexedFile in Index.indexedFileList)
                    {
                        if (file == indexedFile.path)
                        {
                            // The file exists
                            fileExists = true;
                            string hash = "NA";
                            try
                            {
                                hash = BitConverter.ToString(FileMgmt.Manager.CheckFileHash(file));
                            }
                            catch (System.ArgumentNullException)
                            {
                                Console.WriteLine("Retrying refresh");
                                Thread.Sleep(150);
                                Console.WriteLine("Re-Indexing");
                                Index.IndexFiles(f);
                                Console.WriteLine("Done indexing, retrying");
                                CheckForNewFilesFolders(f);
                            }


                            if (hash == BitConverter.ToString(indexedFile.hash))
                            {

                            }
                            else
                            {
                                // The file has been edited
                                fileEdited = true;
                            }
                        }
                    }
                } catch (System.InvalidOperationException)
                {
                    CheckForNewFilesFolders(f);
                }

                // Adds the appropriate items to the lists declared above
                if (!fileExists)
                {
                    newFiles.Add(file);
                } else if (fileEdited == true)
                {
                    editedFiles.Add(file);
                }
            }
            foreach (string sdir in Directory.GetDirectories(f)) // Checks the directories
            {
                newFolderIndex.Add(sdir);
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

                    CheckForNewFilesFolders(sdir);
                }
                catch
                {
                    // Do Nothing
                }
            }
        }

        // CAN ONLY BE RAN AFTER CheckForNewFilesFolders
        public static void CheckFileFolderDelete(string f)
        {
            foreach (IndexedFiles oldFile in Index.indexedFileList)
            {
                bool fileFound = false;
                foreach (string file in newFileIndex)
                {
                    if (oldFile.path == file)
                    {
                        fileFound = true;
                    }
                }

                if (!fileFound)
                {
                    Console.WriteLine("Found file to delete");
                    filesToDelete.Add(oldFile.path);
                }
            }
            newFileIndex.Clear(); // Clear the index for it to be repopulated

            foreach (string oldDir in Index.indexedDirectoriesList)
            {
                bool dirFound = false;
                foreach (string newDir in newFolderIndex)
                {
                    if (oldDir == newDir) {
                        dirFound = true;
                    }
                }

                if (!dirFound)
                {
                    foldersToDelete.Add(oldDir);
                }
            }
            newFolderIndex.Clear(); // Clear the index for it to be repopulated
        }
    }
}

