﻿using System;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

namespace FileMgmt
{
    class Manager
    {
        public static void RenameFile(string oldPath, string newPath)
        {
            string data = ReadFile(oldPath);
            CreateAndPopulateFile(newPath, data);
            DeleteFile(oldPath);
        }

        public static void RenameFolder(string oldPath, string newPath)
        {
            CreateDirectory(newPath);
            DeleteDirectory(oldPath);
        }

        public static bool FileExists(string fileToCheck)
        {
            return File.Exists(fileToCheck);

        }

        public static bool DirExists(string folderToCheck)
        {
            return Directory.Exists(folderToCheck);
        }

        public static void MoveFile(string from, string to)
        {
            string data = ReadFile(from); // Read the data
            CreateAndPopulateFile(to, data); // Create and populate the new file
            DeleteFile(from); // Delete the old file
        }

        public static void CreateAndPopulateFile(string file, string data)
        {
            CreateFile(file);
            UpdateFile(file, data);
        }

        public static void CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public static bool DeleteDirectory(string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                    return true;
                }
            }
            catch (IOException)
            {
                Thread.Sleep(250);
                DeleteDirectory(directory);
            }
            return false;
        }

        public static void CreateFile(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    using (FileStream fs = new FileStream(file, FileMode.Truncate))
                    {
                        // Truncated
                        fs.Close();
                    }
                }
                catch
                {
                    Thread.Sleep(250);
                    CreateFile(file);
                }
            } else
            {
                try
                {
                    using (FileStream fs = new FileStream(file, FileMode.Create))
                    {
                        // Created
                        fs.Close();
                    }
                }
                catch
                {
                    Thread.Sleep(250);
                    CreateFile(file);
                }
            }
        }

        public static bool DeleteFile(string file)
        {
            // Wait for file to become available
            while (!Checker.FileAvailable(file))
            {
                Thread.Sleep(100);
            }
            // Continue

            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch
            {
                Thread.Sleep(250);
                DeleteFile(file);
            }
            return true;
        }

        public static bool UpdateFile(string file, string data)
        {
            // Wait for file to become available
            while (!Checker.FileAvailable(file))
            {
                Thread.Sleep(100);
            }
            // Continue
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Truncate))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(data);
                        sw.Close();
                    }
                    fs.Close();
                }
            }
            catch
            {
                Thread.Sleep(250);
                UpdateFile(file, data);
            }
            return true;
        }

        static int count = 0;
        public static string ReadFile(string file)
        {
            // Wait for file to become available
            while (!Checker.FileAvailable(file))
            {
                Thread.Sleep(100);
            }
            string data = "None";
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        data = sr.ReadToEnd();
                        sr.Close();
                    }
                    fs.Close();
                }
            }
            catch
            {
                Thread.Sleep(250);
                if (count < 30)
                {
                    count++;
                    ReadFile(file);
                } else
                {
                    count = 0;
                    return null;
                }
            }
            return data;
        }
    }

    public class Checker
    {
        public static bool FileAvailable(string file)
        {
            if (File.Exists(file))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (IOException)
                {
                    return false;
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            }
            return true;
        }
    }
}
