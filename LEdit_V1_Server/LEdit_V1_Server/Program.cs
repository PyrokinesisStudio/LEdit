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

namespace LEdit_V1_Server
{
    public class LE_Functions : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            String[] dataParams = e.Data.Split(Convert.ToChar(" "));

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
                    Console.WriteLine("Sent Data Over");
                    break;
                case "UploadFileData":
                    Send(ActionRunner.RunFileUpload(dataParams[1], e.Data.Substring(15 + dataParams[1].Length)));
                    Console.WriteLine("Received data");
                    Console.WriteLine(dataParams[1]);
                    Console.WriteLine(e.Data.Substring(16 + dataParams[1].Length));
                    break;
            }
        }

        protected override void OnOpen()
        {
            Console.WriteLine("A Client has Connected");
            //Send("Connected to LE_Server V1");   
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
                List<String> fileIndex = FetchData.FetchFileIndex();
                string fileIndexTogether = "";

                foreach (string file in fileIndex)
                {
                    fileIndexTogether += file;
                }

                return (fileIndexTogether);
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
                List<String> folderIndex = FetchData.FetchDirectoryIndex();
                string folderIndexTogether = "";

                foreach (string folder in folderIndex)
                {
                    folderIndexTogether += folder;
                }
                return (folderIndexTogether);
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
                string folderIndex = FetchData.FetchFileData(dataParams[3]);

                return (folderIndex);
            }
            else
            {
                return "False";
            }
        }

        public static string RunFileUpload(string fileName, string fileData)
        {
                String[] where = new String[2];
                where[0] = "file";
                where[1] = fileName;
                SendData.UpdateData("files", "data", fileData, where);
            return "True";
        }
    }

    public class Auth
    {
        public static bool Login(string username, string password)
        {
            var dbCon = MySQL.DBConnection.Instance();

            if (dbCon.Connected())
            {
                string q = "SELECT * FROM users";

                MySqlDataReader reader = MySQL.Data.GetData(q, dbCon.Connection);

                while (reader.Read())
                {
                    if (reader.GetString(1) == username.ToUpper())
                    {
                        string storedPass = reader.GetString(2);
                        if (Crypter.CheckPassword(password, storedPass))
                        {
                            Console.WriteLine("User Auth: Success");
                            reader.Close();
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("User Auth: Failure");
                            reader.Close();
                            return false;
                        }
                    }
                }
                
                reader.Close();
                dbCon.Close();
            }
            return false;
        }
    }

    public class SendData
    {
        internal static void UpdateData(string table, string col, string data, String[] where)
        {

            var dbCon = MySQL.DBConnection.Instance();

            if (dbCon.Connected())
            {
                if (dbCon.Connection.State != ConnectionState.Open)
                {
                    dbCon.Connection.Open();
                }

                MySqlCommand q = new MySqlCommand()
                {
                    Connection = dbCon.Connection,
                    CommandText = $"UPDATE {table} SET {col}=@data WHERE {where[0]}=@where2"
            };
                q.Parameters.AddWithValue("@data", data);
                q.Parameters.AddWithValue("@where2", where[1]);
                q.Prepare();

                q.ExecuteNonQuery();

                dbCon.Close();
            }

        }
    }

    public class FetchData
    {
        internal static List<String> FetchDirectoryIndex()
        {
            List<String> dirIndex = new List<string>();

            var dbCon = MySQL.DBConnection.Instance();

            if (dbCon.Connected())
            {
                string q = "SELECT * FROM directories";
                MySqlDataReader reader = MySQL.Data.GetData(q, dbCon.Connection);

                while (reader.Read())
                {
                    dirIndex.Add(reader.GetString(1) + " ");
                }

                reader.Close();
                dbCon.Close();
            }

            return dirIndex;
        }

        internal static List<String> FetchFileIndex()
        {
            List<String> fileIndex = new List<string>();

            var dbCon = MySQL.DBConnection.Instance();

            if (dbCon.Connected())
            {
                string q = "SELECT * FROM files";
                MySqlDataReader reader = MySQL.Data.GetData(q, dbCon.Connection);

                while (reader.Read())
                {
                    fileIndex.Add(reader.GetString(1) + " ");   
                }

                reader.Close();
                dbCon.Close();
            }

            return fileIndex;
        }

        internal static string FetchFileData(string fileName)
        {
            var dbCon = MySQL.DBConnection.Instance();
            string data = "";

            if (dbCon.Connected())
            {
                if (dbCon.Connection.State != ConnectionState.Open)
                {
                    dbCon.Connection.Open();
                }

                MySqlCommand q = new MySqlCommand()
                {
                    Connection = dbCon.Connection,
                    CommandText = "SELECT * FROM files WHERE file=@fileName"
                };
                q.Parameters.AddWithValue("@fileName", fileName);
                q.Prepare();

                MySqlDataReader reader = q.ExecuteReader();


                while (reader.Read())
                {
                    data = reader.GetString(2);
                }

                reader.Close();
                dbCon.Close();
            }

            return data;
        }
    }

    public class Program
    {
        public static WebSocketServer server;

        static void Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 90;

            server = new WebSocketServer(IPAddress.Parse(ip), port, false);

            server.Start();
            server.AddWebSocketService<LE_Functions>("/LE");
            if (server.IsListening)
            {
                Console.WriteLine("Listening on  {0}:{1}, and providing path services:", ip, port);
                foreach (var path in server.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }

            Console.WriteLine("\nPress Enter to stop the server...");
            Console.ReadLine();

            server.Stop();

            Console.ReadKey();
        }

    }
}

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

        public string dbHost = "localhost";
        public string dbName = "my_db_name";
        public string dbUser = "my_db_username";
        public string dbPass = "my_db_password";
        public int dbPort = 3307; // my_db_port

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
                string connstring = string.Format($"Server={dbHost}; port={dbPort}; database={dbName}; UID={dbUser}; password={dbPass}");
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