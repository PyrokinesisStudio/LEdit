﻿using System;
using WebSocketSharp;
using System.Threading;
using System.Configuration;
using System.Net;
using SevenZip;
using System.Text.RegularExpressions;

namespace Misc
{
    public class Config
    {
        public static string ip;
        public static int port;
        public static string projectFolder;
        public static string fullProjectPath;
    }

    public class Userdata
    {
        public static string GreetName { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static bool UserReady = false;
    }

    public class Global
    {
        public static WebSocket connectionSocket;
        public static Thread msgHandler;
        // cba to update it to the normal naming standards, will do it some other time
        public static string settings;
        public static string[] settingsS;
        public static string ipaddrstring;
        public static string portstring;
        public static string projectfolder;
    }

    public class Expressions
    {
        public static void LoginExpression(string data, string username, string password)
        {
            if (data == "True")
            {
                Userdata.Username = username;
                Userdata.Password = password;
                Userdata.GreetName = Userdata.Username.Substring(0, 1).ToUpper() + Userdata.Username.Substring(1);
                Console.WriteLine("Successfully logged in");
                Console.WriteLine($"Welcome back {Userdata.GreetName}, please wait while we set things up for you.");
                Console.WriteLine("Loading...");
                FileMgmt.Manager.DeleteDirectory(Config.fullProjectPath);
                Setup.Index.IndexDirectories();

            }
            else if (data == "False")
            {
                Console.WriteLine("Failed to log in");
            }
            else
            {
                throw new Exception("A server error has occured");
            }
        }

        public static void OnFinishedLoading()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Hello {Misc.Userdata.GreetName}, welcome to LEdit V1 (Full).");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("The application has finished it's setup process, you are now free to view the files in the directory ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"{Misc.Config.fullProjectPath}\n");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("And can modify them as you wish, please remember that you can access help by any time by typing ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("`help`\n");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Also please remember to be careful as your changes submit as soon as you make them");
            Console.Write("Due to the nature of this application, we have some settings which you can configure as you wish, to access them, please type ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("`settings`\n");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Thank you!");

            Console.WriteLine("\nStarting the automatic file action scanning thread");
            // START THREAD HERE
            //Thread autoFileActionThread = new Thread(new ThreadStart(ActionRunner.ScanRunner.ChangeScanner));
            //autoFileActionThread.Start();
            Console.WriteLine("Started");
            Console.ForegroundColor = ConsoleColor.Green;
            //ActionRunner.Index.IndexFiles(Misc.Config.fullProjectPath);
            Watcher.Watcher.ConfigureWatch();
        }
    }
}

namespace LEdit_V1_Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "LEdit";
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!FileMgmt.Manager.DirExists($"{docs}\\LEdit_Data"))
            {
                FileMgmt.Manager.CreateDirectory($"{docs}\\LEdit_Data");
            }
            if (!FileMgmt.Manager.FileExists($"{docs}\\LEdit_Data\\settings.txt"))
            {
                // files are lowercases it looks nicer if they have a new line for each
                string data = "IPAddress = 127.0.0.1,\nPort = 90,\nFolder = sp,";
                FileMgmt.Manager.CreateAndPopulateFile($"{docs}\\LEdit_Data\\settings.txt", data);
            }

            Misc.Global.settings = FileMgmt.Manager.ReadFile($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\LEdit_Data\\settings.txt");
            Misc.Global.settingsS = Misc.Global.settings.Split(',');
            Misc.Global.ipaddrstring = Misc.Global.settingsS[0];
            Misc.Global.portstring = Misc.Global.settingsS[1];
            Misc.Global.projectfolder = Misc.Global.settingsS[2];

            Misc.Config.ip = Misc.Global.ipaddrstring.Substring(12);
            Misc.Config.port = Convert.ToInt32(Misc.Global.portstring.Substring(7));
            Misc.Config.projectFolder = Misc.Global.projectfolder.Substring(10);
            Misc.Config.fullProjectPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{Misc.Config.projectFolder}";

            SocketSetup();
        }

        public static void SocketSetup()
        {
            Misc.Global.connectionSocket = new WebSocket($"ws://{Misc.Config.ip}:{Misc.Config.port}/LE");
            Misc.Global.connectionSocket.Connect();
            Misc.Global.connectionSocket.Compression = WebSocketSharp.CompressionMethod.Deflate;
            Handler.MessageHandler.AppListener(L.Change.ChangeListener);

            while (true)
            {
                string msg = Console.ReadLine();

                String[] splitMsg = msg.Split(Convert.ToChar(" "));

                Console.WriteLine("Loading...");

                if (splitMsg[0] == "RunLogin")
                {
                    if (Misc.Userdata.Username == null)
                    {
                        // Directly sends message and listens for a response
                        Misc.Global.connectionSocket.Send(msg); // Send Message to Server
                        Handler.MessageHandler.UserListener(splitMsg); // Listen for Reply
                    }
                }
                else if (splitMsg[0] == "Help" || splitMsg[0] == "Settings")
                {
                    Handler.MessageHandler.ClientActions(splitMsg);
                }
                else if (splitMsg[0] == "Reconnect")
                {
                    Misc.Global.connectionSocket.Close();
                    SocketSetup();
                }
                else
                {
                    // Allows for control of message receiving
                    Handler.MessageHandler.ClientActions(splitMsg);
                }
            }
        }
    }
}