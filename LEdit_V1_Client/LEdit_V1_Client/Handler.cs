using System;
using System.Configuration;

namespace Handler
{
    class MessageHandler
    {
        static String[] splitMsg = null;

        // Creates an event handler to run an action based on what the user has typed (client side)
        public static void UserListener(String[] splitMsgLocal)
        {
            splitMsg = splitMsgLocal;
            Misc.Global.connectionSocket.OnMessage += ListenerActions;
        }

        /* 
        
        An Example of how to use the App Listener and CloseListener Functions
        
        public static void test()
        {
            AppListener(asd);
            CloseListener(asd);
        }

        public static void asd (object sender, WebSocketSharp.MessageEventArgs e)
        {

        }

        */

        // Creates an OnMessage event handler to run the desired function (application side)
        public static void AppListener(EventHandler<WebSocketSharp.MessageEventArgs> function)
        {
            Misc.Global.connectionSocket.OnMessage += function;
        }

        // Closes the OnMessage event handler (app/client)
        public static void CloseListener(EventHandler<WebSocketSharp.MessageEventArgs> function)
        {
            Misc.Global.connectionSocket.OnMessage -= function;
        }

        private static void ListenerActions(object sender, WebSocketSharp.MessageEventArgs e)
        {
            switch (splitMsg[0])
            {
                case "RunLogin":
                    string usr;
                    string pwd;
                    if (splitMsg.Length < 3)
                    {
                        usr = "N/A";
                        pwd = "N/A";
                    }
                    else
                    {
                        usr = splitMsg[1];
                        pwd = splitMsg[2];
                    }
                    Misc.Expressions.LoginExpression(e.Data, usr, pwd);

                    CloseListener(ListenerActions);
                    break;
            }
        }

        public static void ClientActions(String[] splitMsg)
        {
            switch (splitMsg[0])
            {
                case "Help":
                    Console.WriteLine("Commands: ");
                    Console.WriteLine("Reconnect");
                    Console.WriteLine("Settings");
                    break;
                case "Settings":
                    Console.WriteLine("Current Settings");
                    Console.WriteLine("IP: {0}", Misc.Config.ip);
                    Console.WriteLine("PORT: {0}", Misc.Config.port);
                    Console.WriteLine("FOLDER: {0}", Misc.Config.projectFolder);
                    Console.WriteLine("To change a setting type: Change(setting) (value)");
                    break;
                case "ChangeIP":
                    string Value = splitMsg[1];
                    try
                    {
                        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        config.AppSettings.Settings["ip"].Value = Value;
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("ip");
                        Console.WriteLine("Restart for changes to take effect");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Run LEdit in administrator  mode to change this setting");
                    }
                    ConfigurationManager.RefreshSection("appSettings");
                    break;
                case "ChangePORT":
                    string Value1 = splitMsg[1];
                    try
                    {
                        var config1 = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        config1.AppSettings.Settings["port"].Value = Value1;
                        config1.Save(ConfigurationSaveMode.Modified);
                    }
                    catch
                    {
                        Console.WriteLine("Run LEdit in administrator  mode to change this setting");
                    }
                    ConfigurationManager.RefreshSection("appSettings");
                    break;
                case "ChangeFOLDER":
                    string Value2 = splitMsg[1];
                    try
                    {
                        var config2 = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        config2.AppSettings.Settings["ProjectFolder"].Value = Value2;
                        config2.Save(ConfigurationSaveMode.Modified);
                    }
                    catch
                    {
                        Console.WriteLine("Run LEdit in administrator  mode to change this setting");
                    }
                    ConfigurationManager.RefreshSection("appSettings");
                    break;
                /*case "DEMO_COMMAND":
                    string path = "streetrp\\streetrp.txt";
                    string data = File.ReadAllText(Misc.Config.fullProjectPath + @"\" + path);
                    Misc.Global.connectionSocket.Send($"UploadFileData {path} {data}");
                    AppListener(UploadFileData_OnReceive);
                    break; */
            }
        }

        private static void UploadFileData_OnReceive(object sender, WebSocketSharp.MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            CloseListener(UploadFileData_OnReceive);
        }

    }
}
