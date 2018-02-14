using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Threading;

namespace Watcher
{
    class Watcher {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void ConfigureWatch()
        {
            string dir = Misc.Config.fullProjectPath;
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = dir;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            watcher.EnableRaisingEvents = true;
        }

        public static List<String> ignore = new List<String>();

        // Define the event handlers
        private static void OnChanged(object src, FileSystemEventArgs e)
        {
            for (int i = 0; i < ignore.Count; i++)
            {
                if (ignore[i] == e.FullPath)
                {
                    ignore.Remove(ignore[i]);
                    return;
                }
            }

            string path = e.FullPath.Substring(e.FullPath.IndexOf(Misc.Config.fullProjectPath) + Misc.Config.fullProjectPath.Length + 1);
            // Continue
            Misc.Global.connectionSocket.Send($"UploadFileData {Misc.Userdata.Username} {Misc.Userdata.Password} {path} {FileMgmt.Manager.ReadFile(e.FullPath)}");
            Handler.MessageHandler.AppListener(UploadListener);
            void UploadListener(object sender, WebSocketSharp.MessageEventArgs ee)
            {
                if (ee.Data == "True")
                {
                    Console.WriteLine("Complete");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed (1) file transfer: " + ee.Data);
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Handler.MessageHandler.CloseListener(UploadListener);
            }
            Console.WriteLine("File/Folder " + e.FullPath + " has been modified");
        }

        private static void OnCreated(object src, FileSystemEventArgs e)
        {
            Console.WriteLine("File/Folder " + e.FullPath + " has been deleted");
            for (int i = 0; i < ignore.Count; i++)
            {
                if (ignore[i] == e.FullPath)
                {
                    ignore.Remove(ignore[i]);
                    return;
                }
            }

            // Continue
            string path = e.FullPath.Substring(e.FullPath.IndexOf(Misc.Config.fullProjectPath) + Misc.Config.fullProjectPath.Length + 1);

            if (path.Contains("."))
            {
                if (path.Contains(" "))
                {
                    path = path.Replace(' ', '_');
                    ignore.Add(e.FullPath);

                    string uploadPath = Misc.Config.fullProjectPath + "\\" + path;
                    string uploadPathHolder = uploadPath;
                    string extension = Path.GetExtension(uploadPath);
                    int num = 0;
                    
                    while (File.Exists(uploadPathHolder))
                    {
                        num++;
                        uploadPathHolder = uploadPath.Substring(0, uploadPath.IndexOf(extension));
                        uploadPathHolder = uploadPathHolder + "_[" + num + "]" + extension;
                    }
                    uploadPath = uploadPathHolder;
                    ignore.Add(uploadPath);
                    FileMgmt.Manager.RenameFile(e.FullPath, uploadPath);
                    path = uploadPath.Substring(e.FullPath.IndexOf(Misc.Config.fullProjectPath) + Misc.Config.fullProjectPath.Length + 1);
                } else
                {
                    string uploadPath = Misc.Config.fullProjectPath + "\\" + path;
                    string uploadPathHolder = uploadPath;
                    string extension = Path.GetExtension(uploadPath);
                    int num = 0;

                    while (File.Exists(uploadPathHolder))
                    {
                        num++;
                        uploadPathHolder = uploadPath.Substring(0, uploadPath.IndexOf(extension));
                        uploadPathHolder = uploadPathHolder + "_[" + num + "]" + extension;
                    }
                    uploadPath = uploadPathHolder;
                    path = uploadPath.Substring(e.FullPath.IndexOf(Misc.Config.fullProjectPath) + Misc.Config.fullProjectPath.Length + 1);
                }
                string data = FileMgmt.Manager.ReadFile(Misc.Config.fullProjectPath + "\\" + path);
                if (data == "" || data == null)
                {
                    data = "// This file is empty";
                }
                Misc.Global.connectionSocket.Send($"CreateNewFile {Misc.Userdata.Username} {Misc.Userdata.Password} {path} {data}");
                Handler.MessageHandler.AppListener(UploadListener);
                void UploadListener(object sender, WebSocketSharp.MessageEventArgs ee)
                {
                    if (ee.Data == "True")
                    {
                        Console.WriteLine("Complete");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed (1) file transfer: " + ee.Data);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Handler.MessageHandler.CloseListener(UploadListener);
                }
            }
            else
            {
                if (path.Contains(" "))
                {
                    path = path.Replace(' ', '_');
                    string uploadPath = Misc.Config.fullProjectPath + "\\" + path;
                    string uploadPathHolder = uploadPath;
                    int num = 0;
                    while (Directory.Exists(uploadPathHolder))
                    {
                        num++;
                        uploadPathHolder = uploadPath + "_[" + num + "]";
                    }
                    uploadPath = uploadPathHolder;
                    ignore.Add(uploadPath);
                    FileMgmt.Manager.RenameFolder(e.FullPath, uploadPath);
                    path = uploadPath.Substring(e.FullPath.IndexOf(Misc.Config.fullProjectPath) + Misc.Config.fullProjectPath.Length + 1);
                } else
                {
                    string uploadPath = Misc.Config.fullProjectPath + "\\" + path;
                    string uploadPathHolder = uploadPath;
                    int num = 0;
                    while (Directory.Exists(uploadPathHolder))
                    {
                        num++;
                        uploadPathHolder = uploadPath + "_[" + num + "]";
                    }
                    uploadPath = uploadPathHolder;
                    path = uploadPath.Substring(e.FullPath.IndexOf(Misc.Config.fullProjectPath) + Misc.Config.fullProjectPath.Length + 1);
                }
                Misc.Global.connectionSocket.Send($"CreateNewFolder {Misc.Userdata.Username} {Misc.Userdata.Password} {path}");
                Handler.MessageHandler.AppListener(UploadListener);
                void UploadListener(object sender, WebSocketSharp.MessageEventArgs ee)
                {
                    if (ee.Data == "True")
                    {
                        Console.WriteLine("Complete");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed (1) file transfer: " + ee.Data);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Handler.MessageHandler.CloseListener(UploadListener);
                }
            }
            Console.WriteLine("File/Folder " + path + " has been created");
        }

        private static void OnDeleted(object src, FileSystemEventArgs e)
        {
            for (int i = 0; i < ignore.Count; i++)
            {
                if (ignore[i] == e.FullPath)
                {
                    ignore.Remove(ignore[i]);
                    return;
                }
            }

            // Continue

            string path = e.FullPath.Substring(e.FullPath.IndexOf(Misc.Config.fullProjectPath) + Misc.Config.fullProjectPath.Length + 1);

            if (path.Contains("."))
            {
                Misc.Global.connectionSocket.Send($"DeleteFile {Misc.Userdata.Username} {Misc.Userdata.Password} {path}");
                Handler.MessageHandler.AppListener(UploadListener);
                void UploadListener(object sender, WebSocketSharp.MessageEventArgs ee)
                {
                    if (ee.Data == "True")
                    {
                        Console.WriteLine("Complete");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed (1) file transfer: " + ee.Data);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Handler.MessageHandler.CloseListener(UploadListener);
                }
            } else
            {
                Misc.Global.connectionSocket.Send($"DeleteFolder {Misc.Userdata.Username} {Misc.Userdata.Password} {path}");
                Handler.MessageHandler.AppListener(UploadListener);
                void UploadListener(object sender, WebSocketSharp.MessageEventArgs ee)
                {
                    if (ee.Data == "True")
                    {
                        Console.WriteLine("Complete");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed (1) file transfer: " + ee.Data);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Handler.MessageHandler.CloseListener(UploadListener);
                }
            }
            Console.WriteLine("File/Folder " + path + " has been deleted");
        }

        private static void OnRenamed(object src, RenamedEventArgs e)
        {
            for (int i = 0; i < ignore.Count; i++)
            {
                if (ignore[i] == e.FullPath)
                {
                    ignore.Remove(ignore[i]);
                    return;
                }
            }
            // Continue

            string path = e.FullPath.Substring(e.FullPath.IndexOf(Misc.Config.fullProjectPath) + Misc.Config.fullProjectPath.Length + 1);
            string oldPath = e.OldFullPath.Substring(e.FullPath.IndexOf(Misc.Config.fullProjectPath) + Misc.Config.fullProjectPath.Length + 1);

            if (e.FullPath.Contains("."))
            {
                if (path.Contains(" "))
                {
                    path = path.Replace(' ', '_');
                    ignore.Add(Misc.Config.fullProjectPath + "\\" + path);
                    ignore.Add(e.FullPath);
                    FileMgmt.Manager.RenameFile(e.FullPath, Misc.Config.fullProjectPath + "\\" + path);
                }
                string data = FileMgmt.Manager.ReadFile(Misc.Config.fullProjectPath + "\\" + path);
                if (data == "" || data == null)
                {
                    data = "// This file is empty";
                }
                Misc.Global.connectionSocket.Send($"CreateNewFile {Misc.Userdata.Username} {Misc.Userdata.Password} {path} {data}");
                Handler.MessageHandler.AppListener(UploadListener);
                void UploadListener(object sender, WebSocketSharp.MessageEventArgs ee)
                {
                    if (ee.Data == "True")
                    {
                        Console.WriteLine("Complete");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed (1) file transfer: " + ee.Data);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Handler.MessageHandler.CloseListener(UploadListener);
                }
                /////////////////////////////////////////
                Misc.Global.connectionSocket.Send($"DeleteFile {Misc.Userdata.Username} {Misc.Userdata.Password} {oldPath}");
                Handler.MessageHandler.AppListener(UploadListener2);
                void UploadListener2(object sender, WebSocketSharp.MessageEventArgs ee)
                {
                    if (ee.Data == "True")
                    {
                        Console.WriteLine("Complete");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed (1) file transfer: " + ee.Data);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Handler.MessageHandler.CloseListener(UploadListener2);
                }
            } else
            {

            }

            Console.WriteLine("File/Folder {0} has been renamed to {1}", oldPath, path);
        }
    }
}