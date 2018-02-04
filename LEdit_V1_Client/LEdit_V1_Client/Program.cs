using System;
using WebSocketSharp;
using System.Threading;
using System.Configuration;
using System.Net;

namespace LEdit_V1_Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            Misc.Global.pauseLiveUpdate = true;
            SocketSetup();
        }

        public static void SocketSetup()
        {
            Misc.Global.connectionSocket = new WebSocket($"ws://{Misc.Config.ip}:{Misc.Config.port}/LE");
            Misc.Global.connectionSocket.Connect();

            Handler.MessageHandler.AppListener(L.Change.ChangeListener);

            while (true)
            {
                string msg = Console.ReadLine();

                String[] splitMsg = msg.Split(Convert.ToChar(" "));

                Console.WriteLine("Loading...");

                if (splitMsg[0] == "RunLogin")
                {
                    // Directly sends message and listens for a response
                    Misc.Global.connectionSocket.Send(msg); // Send Message to Server
                    Handler.MessageHandler.UserListener(splitMsg); // Listen for Reply
                }
                else if (splitMsg[0] == "Settings")
                {
                    Handler.MessageHandler.ClientActions(splitMsg);
                }
                else if (splitMsg[0] == "Help")
                {
                    Handler.MessageHandler.ClientActions(splitMsg);
                }
                else if (splitMsg[0] == "Reconnect")
                {
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

namespace Misc
{
    public class Config
    {
        public static IPAddress ip = IPAddress.Parse("176.31.102.221");
        public static int port = 90;
        public static string projectFolder = "sp";
        public static string fullProjectPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{projectFolder}";
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
        public static bool pauseLiveUpdate = false;
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
            Thread autoFileActionThread = new Thread(new ThreadStart(ActionRunner.ScanRunner.ChangeScanner));
            autoFileActionThread.Start();
            Console.WriteLine("Started");
            Console.ForegroundColor = ConsoleColor.Green;
            ActionRunner.Index.IndexFiles(Misc.Config.fullProjectPath);
            Global.pauseLiveUpdate = false;
        }
    }
}
