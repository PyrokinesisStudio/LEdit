using System;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;
using CryptSharp;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Collections.Generic;
using Server_Variables;

namespace LEdit_V1_Server
{
	public class LE_Functions : WebSocketBehavior
	{
		protected override void OnMessage(MessageEventArgs e)
		{
			string[] dataParams = e.Data.Split(' ');

			switch (dataParams[0])
			{
				case "RunLogin":
					Send(ActionRunner.RunLogin(dataParams, isUAC: false).ToString());
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
					SendTask(ActionRunner.RunFileUpdate(dataParams, dataParams[3], Other.MiscFunctions.StringDecompressBytes(bytes)));
					BroadcastMessage($"RefreshFile {dataParams[3]} {dataParams[1]} {bytes}");
					break;
				case "CreateNewFile":
					var data = e.Data.Substring(e.Data.IndexOf(dataParams[4]));
					SendTask(ActionRunner.RunFileUpload(dataParams, dataParams[3], data));
					BroadcastMessage($"CreateFile {dataParams[1]} {dataParams[3]} {data}");
					break;
				case "DeleteFile":
					SendTask(ActionRunner.RunFileDeleter(dataParams));
					BroadcastMessage($"DeleteFile {dataParams[1]} {dataParams[3]}");
					break;
				case "CreateNewFolder":
					SendTask(ActionRunner.RunFolderCreator(dataParams));
					BroadcastMessage($"CreateFolder {dataParams[1]} {dataParams[3]}");
					break;
				case "DeleteFolder":
					SendTask(ActionRunner.RunFolderDeleter(dataParams));
					BroadcastMessage($"DeleteFolder {dataParams[1]} {dataParams[3]}");
					break;
				case "RunLoginUC":
					Send(ActionRunner.RunLogin(dataParams, isUAC: true).ToString());
					break;
			}
		}

		private void BroadcastMessage(string message) => Task.Factory.StartNew(() => Sessions.Broadcast(message));
		private void SendTask(string status) => Task.Factory.StartNew(() => Send(status));

		protected override void OnOpen()
		{
			Console.WriteLine("A Client has Connected");
			Send("Connected to LE_Server V1");
		}

		protected override void OnClose(CloseEventArgs e)
		{
			Console.WriteLine($"Client has exited: {e.Code}");
		}
	}

	public class ActionRunner
	{
		private static WebClient client = new WebClient();

		public static bool RunLogin(string[] dataParams, bool isUAC)
		{
			string username = null;
			string password = null;

			try
			{
				username = dataParams[1];
				password = dataParams[2];
				if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
					throw new IndexOutOfRangeException("Null Credentials");
			}
			catch (IndexOutOfRangeException)
			{
				Console.WriteLine("Login Failed - Data sent was incomplete");
				return false;
			}

			bool tf = true;
			if (isUAC) tf &= Auth.CheckRank(username) == 0;

			return tf && Auth.Login(username, password);
		}

		private static string Download(string address, string username, string password) =>
			Auth.Login(username, password) ? client.DownloadString(address) : bool.FalseString;

		private static string Upload(string address, string username, string password, string fileData) =>
			Auth.Login(username, password) ? System.Text.Encoding.UTF8.GetString(client.UploadValues(address, new NameValueCollection() { { "data", Uri.EscapeDataString(fileData) } })) : "Bad Login";

		public static string RunFileIndexSender(string[] dataParams) => Download($"{Settings.API.api_link}?action=RequestIndex&type=files", dataParams[1], dataParams[2]);
		public static string RunFolderIndexSender(string[] dataParams) => Download($"{Settings.API.api_link}?action=RequestIndex&type=folders", dataParams[1], dataParams[2]);
		public static string RunFileDataSender(string[] dataParams) => Download($"{Settings.API.api_link}?action=RequestFileData&file={dataParams[3]}", dataParams[1], dataParams[2]);
		public static string RunFolderCreator(string[] dataParams) => Download($"{Settings.API.api_link}?action=CreateFolder&folder={dataParams[3]}&username={dataParams[1]}", dataParams[1], dataParams[2]);
		public static string RunFolderDeleter(string[] dataParams) => Download($"{Settings.API.api_link}?action=DeleteFolder&folder={dataParams[3]}&username={dataParams[1]}", dataParams[1], dataParams[2]);
		public static string RunFileDeleter(string[] dataParams) => Download($"{Settings.API.api_link}?action=DeleteFile&file={dataParams[3]}&username={dataParams[1]}", dataParams[1], dataParams[2]);

		public static string RunFileUpload(string[] dataParams, string fileName, string fileData) =>
			Upload($"{Settings.API.api_link}?action=CreateFile&file={fileName}&username={dataParams[1]}", dataParams[1], dataParams[2], fileData);

		public static string RunFileUpdate(string[] dataParams, string fileName, string fileData) =>
			Upload($"{Settings.API.api_link}?action=UploadFileData&file={fileName}&username={dataParams[1]}", dataParams[1], dataParams[2], fileData);
	}

	public class Auth
	{
		public static bool Login(string username, string password) =>
			(Server_Variables.Userdata.UserData.TryGetValue(username.ToUpperInvariant(), out var cryptPass))
			? Crypter.CheckPassword(password, cryptPass)
			: false;

		public static int CheckRank(string username) =>
			Convert.ToInt32(new WebClient().DownloadString($"{Settings.API.api_link}?action=CheckRank&user={username}"));
	}

	public class Program
	{
		static void Main(string[] args)
		{
			var dataRetriever = new WebClient();
			string usernames_data = dataRetriever.DownloadString($"{Settings.API.api_link}?action=RequestUserData&request=username");
			string[] usernames = usernames_data.Split(' ');

			string passwords_data = dataRetriever.DownloadString($"{Settings.API.api_link}?action=RequestUserData&request=password");
			string[] passwords = passwords_data.Split(' ');

			Userdata.UserData = new Dictionary<string, string>(usernames.Length);

			for (int i = 0; i < usernames.Length; i++)
			{
				Userdata.UserData.Add(usernames[i], passwords[i]);
			}

			Sockets.server = new WebSocketServer(IPAddress.Parse(Settings.Socket_Config.ip_addr), Settings.Socket_Config.port, false);
			Sockets.server.Start();
			Sockets.server.AddWebSocketService<LE_Functions>("/LE");
			if (Sockets.server.IsListening)
			{
				Console.WriteLine("Listening on  {0}:{1}, and providing path services:", Settings.Socket_Config.ip_addr, Settings.Socket_Config.port);
				foreach (var path in Sockets.server.WebSocketServices.Paths)
					Console.WriteLine("- {0}", path);
			}
			Console.WriteLine("\nPress Enter to stop the server...");
			Console.ReadLine();

			Sockets.server.Stop();
			Console.WriteLine("Server has been stopped. Press any key to exit.");
			Console.ReadKey();
		}

	}
}

namespace Server_Variables
{
	public class Userdata
	{
		public static Dictionary<string, string> UserData { get; set; }
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
	public static class Socket_Config
	{
		public static string ip_addr = "176.31.102.221"; // The server IP address
		public static int port = 90; // The server port (please note you might have to add an exception for incoming traffic on Windows Firewall)
	}

	public static class API
	{
		public static string api_link = "http://sv-01.shiftdev.co.uk/api/api.php"; // Link to the api.php file that you installed on your web server
	}
}