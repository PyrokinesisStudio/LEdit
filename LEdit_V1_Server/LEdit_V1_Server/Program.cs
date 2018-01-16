using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
using MySql.Data;
using MySql.Data.MySqlClient;
using CryptSharp;
using System.Threading;
using System.Data;
using System.Collections.Specialized;

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
                    Send(ActionRunner.RunFileDataSender(dataParams));
                    break;
                case "UploadFileData":
                    data = e.Data.Substring(e.Data.IndexOf(dataParams[4]));
                    status = ActionRunner.RunFileUpdate(dataParams, dataParams[3], data);
                    Server_Variables.Sockets.server.WebSocketServices["/LE"].Sessions.Broadcast($"RefreshFile {dataParams[3]} {data}");
                    Send(status);
                    break;
                case "CreateNewFile":
                    data = e.Data.Substring(e.Data.IndexOf(dataParams[4]));
                    status = ActionRunner.RunFileUpload(dataParams, dataParams[3], data);
                    Server_Variables.Sockets.server.WebSocketServices["/LE"].Sessions.Broadcast($"CreateFile {dataParams[3]}");
                    Send(status);
                    break;
                case "DeleteFile":
                    status = ActionRunner.RunFileDeleter(dataParams);
                    Send(status);
                    Server_Variables.Sockets.server.WebSocketServices["/LE"].Sessions.Broadcast($"DeleteFile {dataParams[3]}");
                    break;
                case "CreateNewFolder":
                    status = ActionRunner.RunFileDeleter(dataParams);
                    Send(status);
                    Server_Variables.Sockets.server.WebSocketServices["/LE"].Sessions.Broadcast($"CreateFolder {dataParams[3]}");
                    break;
                case "DeleteFolder":
                    status = ActionRunner.RunFileDeleter(dataParams);
                    Send(status);
                    Server_Variables.Sockets.server.WebSocketServices["/LE"].Sessions.Broadcast($"DeleteFolder {dataParams[3]}");
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

        public static string RunFileIndexSender(String[] dataParams)
        {
            string username = dataParams[1];
            string password = dataParams[2];

            if (Auth.Login(username, password))
            {
                WebClient dataRetriever = new WebClient();
                string data = dataRetriever.DownloadString($"http://sv-01.shiftdev.co.uk/api/api.php?action=RequestIndex&type=files");
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
                string data = dataRetriever.DownloadString($"http://sv-01.shiftdev.co.uk/api/api.php?action=RequestIndex&type=folders");
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
                string data = dataRetriever.DownloadString($"http://sv-01.shiftdev.co.uk/api/api.php?action=RequestFileData&file={dataParams[3]}");
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
                string data = dataRetriever.DownloadString($"http://sv-01.shiftdev.co.uk/api/api.php?action=CreateFolder&folder={dataParams[3]}&username={username}");
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
                string data = dataRetriever.DownloadString($"http://sv-01.shiftdev.co.uk/api/api.php?action=DeleteFolder&folder={dataParams[3]}&username={username}");
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
                string data = dataRetriever.DownloadString($"http://sv-01.shiftdev.co.uk/api/api.php?action=DeleteFile&file={dataParams[3]}&username={username}");
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
                dataRetriever.UploadValues($"http://sv-01.shiftdev.co.uk/api/api.php?action=CreateFile&file={fileName}&username={username}", new NameValueCollection()
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
                dataRetriever.UploadValues($"http://sv-01.shiftdev.co.uk/api/api.php?action=UploadFileData&file={fileName}&username={username}", new NameValueCollection()
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
            String[,] storedUserData = Server_Variables.Userdata.UserData; // I'm very lazy :3 - Soy muy perezoso :3
            
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
    }

    public class Program
    {          
        static void Main(string[] args)
        {
            WebClient dataRetriever = new WebClient();
            string usernames_data = dataRetriever.DownloadString($"http://sv-01.shiftdev.co.uk/api/api.php?action=RequestUserData&request=username");
            string[] usernames = usernames_data.Split(' ');

            string passwords_data = dataRetriever.DownloadString($"http://sv-01.shiftdev.co.uk/api/api.php?action=RequestUserData&request=password");
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
/*
namespace MySQL
{
    public class Data
    {
        public static MySqlDataReader GetData(string query, MySqlConnection conn)
        {
            MySqlCommand command = new MySqlCommand(query, conn);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            
            MySqlDataReader data = command.ExecuteReader();

            return data;
        }
    }

    public class DBConnection
    {
        private DBConnection()
        {
        }

        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool Connected()
        {
            if (Connection == null)
            {
                string connstring = string.Format($"Server={Settings.MySQL_Config.dbHost}; port={Settings.MySQL_Config.dbPort}; database={Settings.MySQL_Config.dbName}; UID={Settings.MySQL_Config.dbUser}; password={Settings.MySQL_Config.dbPass}");
                connection = new MySqlConnection(connstring);
                connection.Open();
            }

            return true;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
*/

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

namespace Settings {
    // You can edit anything in this namespace appart from the variable names and everything before that on that line
    // for example for ip_addr it is public static string ip_addr = "127.0.0.1"
    // you can edit the "127.0.0.1" but nothing else on that line
    public class Socket_Config
    {
        public static string ip_addr = "127.0.0.1"; // The server IP address
        public static int port = 90; // The server port (please note you might have to add an exception for incoming traffic on Windows Firewall)
    }
}