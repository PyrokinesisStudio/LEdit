﻿using System;
using System.Configuration;
using System.IO;
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
            string docs = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\LEdit_Data\\settings.txt";
            string Settings = FileMgmt.Manager.ReadFile(docs);
            string[] SettingsS = Settings.Split(',');
            string NewSettings;
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
                    
                    SettingsS[0] = "IPAddress = " + Value + ",";
                    SettingsS[1] = SettingsS[1] + ",";
                    SettingsS[2] = SettingsS[2] + ",";
                    NewSettings  = SettingsS[0] + SettingsS[1] + SettingsS[2];
                    FileMgmt.Manager.UpdateFile(docs,NewSettings);
                    break;
                case "ChangePORT":
                    string Value1 = splitMsg[1];
                    SettingsS[0] = SettingsS[0] + ",";
                    SettingsS[1] = "Port = " + Value1 + ",";
                    SettingsS[2] = SettingsS[2] + ",";
                    NewSettings = SettingsS[0] + SettingsS[1] + SettingsS[2];
                    FileMgmt.Manager.UpdateFile(docs, NewSettings);
                    break;
                case "ChangeFOLDER":
                    string Value2 = splitMsg[1];
                    SettingsS[0] = SettingsS[0] + ",";
                    SettingsS[1] = SettingsS[1] + ",";
                    SettingsS[2] = "Folder = " + Value2 + ",";
                    NewSettings = SettingsS[0] + SettingsS[1] + SettingsS[2];
                    FileMgmt.Manager.UpdateFile(docs, NewSettings);
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
