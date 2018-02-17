using System;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;
using CryptSharp;
using System.Collections.Specialized;
using SevenZip;

namespace LEdit_V1_Server
{
    public class LE_Functions : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            String[] dataParams = e.Data.Split(Convert.ToChar(" "));
            string data;
            string status;

            switch (dataParams[0])
            {
                case "RunLogin":
                    if (ActionRunner.RunLogin(dataParams))
                    {
                        Send("True");
                    }
                    else
                    {
                        Send("False");
                    }
                    break;

                case "RequestFileIndex":
                    Send(ActionRunner.RunFileIndexSender(dataParams));
                    break;
                case "RequestDirectoryIndex":
                    Send(ActionRunner.RunFolderIndexSender(dataParams));
                    break;
                case "RequestFileData":
                    Send(Other.MiscFunctions.StringCompressBytes(ActionRunner.RunFileDataSender(dataParams)));
                    break;
                case "UploadFileData":
                    string bytes = e.Data.Substring(e.Data.IndexOf(dataParams[4]));
                    string str = Other.MiscFunctions.StringDecompressBytes(bytes);
                    status = ActionRunner.RunFileUpdate(dataParams, dataParams[3], str);
                    Send(status);
                    Sessions.Broadcast($"RefreshFile {dataParams[3]} {dataParams[1]} {bytes}"); 
                    break;
                case "CreateNewFile":
                    data = e.Data.Substring(e.Data.IndexOf(dataParams[4]));
                    status = ActionRunner.RunFileUpload(dataParams, dataParams[3], data);
                    Send(status);
                    Sessions.Broadcast($"CreateFile {dataParams[1]} {dataParams[3]} {data}");
                    break;
                case "DeleteFile":
                    status = ActionRunner.RunFileDeleter(dataParams);
                    Send(status);
                    Sessions.Broadcast($"DeleteFile {dataParams[1]} {dataParams[3]}");
                    break;
                case "CreateNewFolder":
                    status = ActionRunner.RunFolderCreator(dataParams);
                    Send(status);
                    Sessions.Broadcast($"CreateFolder {dataParams[1]} {dataParams[3]}");
                    break;
                case "DeleteFolder":
                    status = ActionRunner.RunFolderDeleter(dataParams);
                    Send(status);
                    Sessions.Broadcast($"DeleteFolder {dataParams[1]} {dataParams[3]}");
                    break;
                case "RunLoginUC":
                    if (ActionRunner.RunLoginUAC(dataParams)) {
                        status = "True";
                    } else
                    {
                        status = "False";
                    }
                    Send(status);
                    break;
            }
        }

        protected override void OnOpen()
        {
            Console.WriteLine("A Client has Connected");
            Send("Connected to LE_Server V1");   
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine($"Client has exited: {e.Code}");
            return;
        }
    }


    public class ActionRunner
    {
        public static bool RunLogin(String[] dataParams)
        {
            bool tf = true;
            string username = null;
            string password = null;
            try
            {
                username = dataParams[1];
                password = dataParams[2];
                if (username == null || username == "" || username == " ")
                {
                    throw new IndexOutOfRangeException("Null Credentials");
                }
                if (password == null || password == "" || password == " ")
                {
                    throw new IndexOutOfRangeException("Null Credentials");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Login Failed - Data sent was incomplete");
                tf = false;
            }

            if (tf == true)
            {
                if (Auth.Login(username, password))
                {
                    tf = true;
                }
                else
                {
                    tf = false;
                }
            }
            return tf;
        }

        public static bool RunLoginUAC(String[] dataParams)
        {
            bool tf = true;
            string username = null;
            string password = null;
            try
            {
                username = dataParams[1];
                password = dataParams[2];
                if (username == null || username == "" || username == " ")
                {
                    throw new IndexOutOfRangeException("Null Credentials");
                }
                if (password == null || password == "" || password == " ")
                {
                    throw new IndexOutOfRangeException("Null Credentials");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Login Failed - Data sent was incomplete");
                tf = false;
            }

            if (tf)
            {
                if (!Auth.Login(username, password))
                {
                    tf = false;
                }
            }

            if (tf)
            {
                int rank = Auth.CheckRank(username);
                if (rank == 0)
                {
                    tf = true;
                } else
                {
                    tf = false;
                }
            }
            return tf;
        }

        public static string RunFileIndexSender(String[] dataParams)
        {
            string username = dataParams[1];
            string password = dataParams[2];

            if (Auth.Login(username, password))
            {
                WebClient dataRetriever = new WebClient();
                string data = dataRetriever.DownloadString($"{Settings.API.api_link}?action=RequestIndex&type=files");
                return (data);
            }
            else
            {
                return "False";
            }
        }

        public static string RunFolderIndexSender(String[] dataParams)
        {
            string username = dataParams[1];
            string password = dataParams[2];

            if (Auth.Login(username, password))
            {
                WebClient dataRetriever = new WebClient();
                string data = dataRetriever.DownloadString($"{Settings.API.api_link}?action=RequestIndex&type=folders");
                return (data);
            }
            else
            {
                return "False";
            }
        }

        public static string RunFileDataSender(String[] dataParams)
        {
            string username = dataParams[1];
            string password = dataParams[2];

            if (Auth.Login(username, password))
            {
                WebClient dataRetriever = new WebClient();
                string data = dataRetriever.DownloadString($"{Settings.API.api_link}?action=RequestFileData&file={dataParams[3]}");
                return (data);
            }
            else
            {
                return "False";
            }
        }

        public static string RunFolderCreator(String[] dataParams)
        {
            string username = dataParams[1];
            string password = dataParams[2];
            string path = dataParams[3];

            if (Auth.Login(username, password))
            {
                WebClient dataRetriever = new WebClient();
                string data = dataRetriever.DownloadString($"{Settings.API.api_link}?action=CreateFolder&folder={dataParams[3]}&username={username}");
                return (data);
            }
            else
            {
                return "False";
            }
        }

        public static string RunFolderDeleter(String[] dataParams)
        {
            string username = dataParams[1];
            string password = dataParams[2];

            if (Auth.Login(username, password))
            {
                WebClient dataRetriever = new WebClient();
                string data = dataRetriever.DownloadString($"{Settings.API.api_link}?action=DeleteFolder&folder={dataParams[3]}&username={username}");
                return (data);
            }
            else
            {
                return "False";
            }
        }

        public static string RunFileDeleter(String[] dataParams)
        {
            string username = dataParams[1];
            string password = dataParams[2];

            if (Auth.Login(username, password))
            {
                WebClient dataRetriever = new WebClient();
                string data = dataRetriever.DownloadString($"{Settings.API.api_link}?action=DeleteFile&file={dataParams[3]}&username={username}");
                return (data);
            }
            else
            {
                return "False";
            }
        }

        public static string RunFileUpload(String[] dataParams, string fileName, string fileData)
        {
            string username = dataParams[1];
            string password = dataParams[2];
            if (Auth.Login(username, password))
            {
                WebClient dataRetriever = new WebClient();
                byte[] response =
                dataRetriever.UploadValues($"{Settings.API.api_link}?action=CreateFile&file={fileName}&username={username}", new NameValueCollection()
                {
                        { "data", System.Uri.EscapeDataString(fileData) }
                });

                string result = System.Text.Encoding.UTF8.GetString(response);
                if (result == "True")
                {
                    return "True";
                }
                else
                {
                    return result;
                }
            }
            else
            {
                return "Bad Login";
            }
        }

        public static string RunFileUpdate(String[] dataParams, string fileName, string fileData)
        {
            string username = dataParams[1];
            string password = dataParams[2];
            if (Auth.Login(username, password))
            {
                WebClient dataRetriever = new WebClient();
                byte[] response =
                dataRetriever.UploadValues($"{Settings.API.api_link}?action=UploadFileData&file={fileName}&username={username}", new NameValueCollection()
                {
                        { "data", System.Uri.EscapeDataString(fileData) }
                });

                string result = System.Text.Encoding.UTF8.GetString(response);
                if (result == "True")
                {
                    return "True";
                }
                else
                {
                    return result;
                }
            }
            else
            {
                return "Bad Login";
            }
        }
    }


    public class Auth
    {
        public static bool Login(string username, string password)
        {
            username = username.ToUpper();
            String[,] storedUserData = Server_Variables.Userdata.UserData; // I'm very lazy :3 - Estoy muy perezoso :3
            
            for (int i = 0; i < storedUserData.Length; i++)
            {
                if (storedUserData[i, 0] == username)
                {
                    if (Crypter.CheckPassword(password, storedUserData[i, 1]))
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static int CheckRank(string username)
        {
            WebClient dataRetriever = new WebClient();
            string rank = dataRetriever.DownloadString($"{Settings.API.api_link}?action=CheckRank&user={username}");
            return Convert.ToInt32(rank);
        }
    }

    public class Program
    {          
        static void Main(string[] args)
        {
            WebClient dataRetriever = new WebClient();
            string usernames_data = dataRetriever.DownloadString($"{Settings.API.api_link}?action=RequestUserData&request=username");
            string[] usernames = usernames_data.Split(' ');

            string passwords_data = dataRetriever.DownloadString($"{Settings.API.api_link}?action=RequestUserData&request=password");
            string[] passwords = passwords_data.Split(' ');

            Server_Variables.Userdata.UserData = new String[usernames.Length, 2];

            for (int i = 0; i < usernames.Length; i++)
            {
                Server_Variables.Userdata.UserData[i, 0] = usernames[i];
                Server_Variables.Userdata.UserData[i, 1] = passwords[i];
            }

            Server_Variables.Sockets.server = new WebSocketServer(IPAddress.Parse(Settings.Socket_Config.ip_addr), Settings.Socket_Config.port, false);
            Server_Variables.Sockets.server.Start();
            Server_Variables.Sockets.server.AddWebSocketService<LE_Functions>("/LE");
            if (Server_Variables.Sockets.server.IsListening)
            {
                Console.WriteLine("Listening on  {0}:{1}, and providing path services:", Settings.Socket_Config.ip_addr, Settings.Socket_Config.port);
                foreach (var path in Server_Variables.Sockets.server.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }
            Console.WriteLine("\nPress Enter to stop the server...");
            Console.ReadLine();

            Server_Variables.Sockets.server.Stop();

            Console.ReadKey();
        }

    }
}

namespace Server_Variables
{
    public class Userdata
    {
        public static String[,] UserData { get; set; }
    }

    public class Sockets
    {
        public static WebSocketServer server;
    }
}

namespace Settings
{
    // You can edit anything in this namespace appart from the variable names and everything before that on that line
    // for example for ip_addr it is public static string ip_addr = "127.0.0.1"
    // you can edit the "127.0.0.1" but nothing else on that line
    public class Socket_Config
    {
        public static string ip_addr = "176.31.102.221"; // The server IP address
        public static int port = 90; // The server port (please note you might have to add an exception for incoming traffic on Windows Firewall)
    }

    public class API
    {
        public static string api_link = "http://sv-01.shiftdev.co.uk/api/api.php"; // Link to the api.php file that you installed on your web server
    }
}