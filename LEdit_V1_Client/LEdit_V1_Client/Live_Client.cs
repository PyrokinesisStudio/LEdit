using System;

namespace L
{
    class Change
    {
        public static void ChangeListener(object sender, WebSocketSharp.MessageEventArgs e)
        {
            String[] dataParams = e.Data.Split(' ');
            switch (dataParams[0])
            {
                case "RefreshFile":
                    string file = dataParams[1];
                    string data = e.Data.Substring(e.Data.IndexOf(dataParams[2]));
                    Misc.Global.pauseLiveUpdate = true;
                    FileMgmt.Manager.UpdateFile(Misc.Config.fullProjectPath + @"\" + file, data);
                    Misc.Global.pauseLiveUpdate = false;
                    Console.WriteLine("Refresh - File Data");
                    break;
                case "CreateFile":
                    Misc.Global.pauseLiveUpdate = true;
                    FileMgmt.Manager.CreateFile(Misc.Config.fullProjectPath + @"\" + dataParams[1]);
                    Misc.Global.pauseLiveUpdate = false;
                    Console.WriteLine("New File Addition");
                    break;
                case "CreateFolder":
                    Misc.Global.pauseLiveUpdate = true;
                    FileMgmt.Manager.CreateDirectory(Misc.Config.fullProjectPath + @"\" + dataParams[1]);
                    Misc.Global.pauseLiveUpdate = false;
                    Console.WriteLine("New Folder Addition");
                    break;
                case "DeleteFile":
                    Misc.Global.pauseLiveUpdate = true;
                    FileMgmt.Manager.DeleteFile(Misc.Config.fullProjectPath + @"\" + dataParams[1]);
                    Misc.Global.pauseLiveUpdate = false;
                    Console.WriteLine("File Deletion");
                    break;
                case "DeleteFolder":
                    Misc.Global.pauseLiveUpdate = true;
                    FileMgmt.Manager.DeleteDirectory(Misc.Config.fullProjectPath + @"\" + dataParams[1]);
                    Misc.Global.pauseLiveUpdate = false;
                    Console.WriteLine("Folder Deletion");
                    break;
            }
        }
    }
}
