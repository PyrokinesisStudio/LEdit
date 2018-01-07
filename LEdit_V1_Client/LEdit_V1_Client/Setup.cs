using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Setup
{
    public class Index
    {
        public static String[] files;
        public static String[] directories;

        public static void IndexDirectories()
        {
            Misc.Global.connectionSocket.Send($"RequestDirectoryIndex {Misc.Userdata.Username} {Misc.Userdata.Password}");

            Handler.MessageHandler.AppListener(IndexDirectories_OnReceive); // Run the function IndexDirectories_OnReceive when the server has replied
        }

        public static void IndexDirectories_OnReceive(object sender, WebSocketSharp.MessageEventArgs e)
        {
            Console.WriteLine("Indexing Directories");

            directories = e.Data.Split(Convert.ToChar(" ")); // Split the directories and save them to the array

            Handler.MessageHandler.CloseListener(IndexDirectories_OnReceive); // Close the event listener

            Console.WriteLine("Directory Indexing: DONE");
            IndexFiles();
        }

        public static void IndexFiles()
        {
            Console.WriteLine("Indexing Files");

            Misc.Global.connectionSocket.Send($"RequestFileIndex {Misc.Userdata.Username} {Misc.Userdata.Password}");
            
            Handler.MessageHandler.AppListener(IndexFiles_OnReceive);
        }

        public static void IndexFiles_OnReceive(object sender, WebSocketSharp.MessageEventArgs e)
        {
            string data = e.Data.Substring(0, e.Data.Length - 1);
            files = data.Split(Convert.ToChar(" "));

            Handler.MessageHandler.CloseListener(IndexFiles_OnReceive);

            Console.WriteLine("Indexing Files: DONE");
            CreateFolders();
        }

        public static void CreateFolders()
        {
            Console.WriteLine("Creating Folders");

            if (!Directory.Exists(Misc.Config.fullProjectPath))
            {
                Directory.CreateDirectory(Misc.Config.fullProjectPath); // Creates the project dir if it doesn't exist
            }

            Console.WriteLine("Creating/Skipping Home Folder: DONE");

            foreach (string dir in directories)
            {
                if (!Directory.Exists(Misc.Config.fullProjectPath + dir))
                {
                    Directory.CreateDirectory(Misc.Config.fullProjectPath + @"\" + dir); // Creates the dirs in the directories array providing they don't exist
                }
            }

            Console.WriteLine("Creating/Skipping Folders: DONE");

            CreateFiles();
        }

        public static void CreateFiles()
        {
            Console.WriteLine("Creating and Populating Files");

            foreach (string file in files)
            {
                if (!File.Exists(Misc.Config.fullProjectPath + @"\" + file))
                    File.Create(Misc.Config.fullProjectPath + @"\" + file);
            }

            RequestFileData();
        }

        public static void RequestFileData()
        {
            DataToRunThrough = files.Count();
            DataRanThrough = 0;

            Misc.Global.connectionSocket.Send($"RequestFileData {Misc.Userdata.Username} {Misc.Userdata.Password} {files[DataRanThrough]}");

            Handler.MessageHandler.AppListener(RequestFileData_OnReceive);
        }

        public static int DataToRunThrough { get; set; }
        public static int DataRanThrough { get; set; }

        public static void RequestFileData_OnReceive(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (DataRanThrough < DataToRunThrough)
            {
                Misc.Global.connectionSocket.Send($"RequestFileData {Misc.Userdata.Username} {Misc.Userdata.Password} {files[DataRanThrough]}");
                string progress = DataRanThrough + "/" + DataToRunThrough;
                Console.WriteLine($"Loading (x/y): {progress}");
                fileData.Add(e.Data);
                DataRanThrough++;
            } else
            {
                Handler.MessageHandler.CloseListener(RequestFileData_OnReceive);
                Console.WriteLine("Requesting Data: DONE");

                PopulateFiles(files, fileData);

                Console.WriteLine("Populating Files: DONE");

                Console.WriteLine("Creating File Index");

                ActionRunner.Index.IndexFiles(Misc.Config.fullProjectPath);

                Console.WriteLine("Creating File Index: DONE");

                Console.ReadKey();

                Misc.Userdata.UserReady = true;

                Misc.Expressions.OnFinishedLoading();
            }
        }

        public static List<String> fileData = new List<String>();

        public static void PopulateFiles(String[] fileNames, List<String> fileData)
        {
            Console.WriteLine("Populating Files");
            for (int key = 0; key < files.Count(); key++)
            {
                string progress = key + "/" + DataToRunThrough;
                File.WriteAllText($"{Misc.Config.fullProjectPath}\\{files[key]}", fileData[key]);
                Console.WriteLine($"Loading (x/y): {progress}");
                key++;
            }

        }
    }
}
