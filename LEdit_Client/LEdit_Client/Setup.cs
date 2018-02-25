using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Setup
{
    public class Index
    {
        public static String[,] files;
        public static String[] directories;

        public static void IndexDirectories()
        {
            Misc.Global.connectionSocket.Send($"RequestDirectoryIndex {Misc.Userdata.Username} {Misc.Userdata.Password}");

            Handler.MessageHandler.AppListener(IndexDirectories_OnReceive); // Run the function IndexDirectories_OnReceive when the server has replied

            void IndexDirectories_OnReceive(object sender, WebSocketSharp.MessageEventArgs e)
            {
                Console.WriteLine("Indexing Directories");
                                    
                directories = e.Data.Replace('\\', Path.DirectorySeparatorChar).Split(' '); // Split the directories and save them to the array

                Handler.MessageHandler.CloseListener(IndexDirectories_OnReceive); // Close the event listener

                Console.WriteLine("Directory Indexing: DONE");
                IndexFiles();
            }
        }



        public static void IndexFiles()
        {
            Console.WriteLine("Indexing Files");

            Misc.Global.connectionSocket.Send($"RequestFileIndex {Misc.Userdata.Username} {Misc.Userdata.Password}");

            Handler.MessageHandler.AppListener(IndexFiles_OnReceive);

            void IndexFiles_OnReceive(object sender, WebSocketSharp.MessageEventArgs e)
            {
                if (e.Data.Length != 0)
                {
                    string data = e.Data.Substring(0, e.Data.Length); // no idea what this exactly does??
                    //data.Split(Convert.ToChar(" "));
                    string[] splitData = data.Split(Convert.ToChar(" "));
                    files = new String[splitData.Length, 2];
                    for (int i = 0; i < files.Length / 2; i++)
                    {
                        files[i, 0] = splitData[i];
                        files[i, 0] = files[i, 0].Replace('\\', Path.DirectorySeparatorChar);
                    }
                }

                Handler.MessageHandler.CloseListener(IndexFiles_OnReceive);

                Console.WriteLine("Indexing Files: DONE");
                CreateFolders();
            }
        }

        public static void CreateFolders()
        {
            Console.WriteLine("Creating Folders");

            if (!Directory.Exists(Misc.Config.fullProjectPath))
            {
                Directory.CreateDirectory(Misc.Config.fullProjectPath); // Creates the project dir if it doesn't exist
            }

            Console.WriteLine("Creating/Skipping Home Folder: DONE");

            for (int i = 0; i < directories.Length; i++) {
                if (!Directory.Exists(Misc.Config.fullProjectPath + directories[i]))
                {
                    Directory.CreateDirectory(Misc.Config.fullProjectPath + System.IO.Path.DirectorySeparatorChar + directories[i]); // Creates the dirs in the directories array providing they don't exist
                }
            }

            Console.WriteLine("Creating/Skipping Folders: DONE");

            CreateFiles();
        }

        public static void CreateFiles()
        {
            Console.WriteLine("Creating and Populating Files");

            if (files != null)
            {
                for (int i = 0; i < files.Length / 2; i++)
                {
                    FileMgmt.Manager.CreateFile(Misc.Config.fullProjectPath + Path.DirectorySeparatorChar + files[i, 0]);
                }
            }

            RequestFileData();
        }

        public static void RequestFileData()
        {
            try
            {
                DataToRunThrough = files.Length / 2;
            }
            catch
            {
                Misc.Userdata.UserReady = true;

                Misc.Expressions.OnFinishedLoading();
                return;
            }
            DataRanThrough = 0;
            Misc.Global.connectionSocket.Send($"RequestFileData {Misc.Userdata.Username} {Misc.Userdata.Password} {files[DataRanThrough, 0]}");
            Handler.MessageHandler.AppListener(RequestFileData_OnReceive);
        }

        public static int DataToRunThrough { get; set; }
        public static int DataRanThrough { get; set; }

        public static void RequestFileData_OnReceive(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (DataRanThrough < DataToRunThrough)
            {
                string progress = DataRanThrough + "/" + DataToRunThrough;
                Console.WriteLine($"Loading (x/y): {progress}");
                files[DataRanThrough, 1] = Other.MiscFunctions.StringDecompressBytes(e.Data);
                DataRanThrough++;
                if (DataRanThrough != DataToRunThrough)
                    Misc.Global.connectionSocket.Send($"RequestFileData {Misc.Userdata.Username} {Misc.Userdata.Password} {files[DataRanThrough, 0]}");
                else
                    FinishLoad();
            }
        }

        public static void FinishLoad()
        {
            Handler.MessageHandler.CloseListener(RequestFileData_OnReceive);
            Console.WriteLine("Requesting Data: DONE");

            PopulateFiles();

            Console.WriteLine("Populating Files: DONE");

            Console.WriteLine("Creating File Index");

            //ActionRunner.Index.IndexFiles(Misc.Config.fullProjectPath);

            Console.WriteLine("Creating File Index: DONE");

            Misc.Userdata.UserReady = true;

            Misc.Expressions.OnFinishedLoading();
        }

        public static List<String> fileData = new List<String>();

        public static void PopulateFiles()
        {
            Console.WriteLine("Populating Files");
            for (int i = 0; i < files.Length / 2; i++)
            {
                string progress = i + "/" + DataToRunThrough;

                FileMgmt.Manager.UpdateFile($"{Misc.Config.fullProjectPath}{Path.DirectorySeparatorChar}{files[i, 0]}", files[i, 1]);
                Console.WriteLine($"Loading (x/y): {progress}");
            }
        }
    }
}
