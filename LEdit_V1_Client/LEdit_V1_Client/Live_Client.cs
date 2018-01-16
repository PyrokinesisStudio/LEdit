using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace L
{
    class Change
    {
        public static void ChangeListener(object sender, WebSocketSharp.MessageEventArgs e)
        {
            String[] dataParams = e.Data.Split(' ');
            switch (dataParams[0])
            {
                case "Refresh":
                    string file = dataParams[1];
                    string data = e.Data.Substring(e.Data.IndexOf(dataParams[2]));
                    string localPath = Misc.Config.fullProjectPath + @"\" + file;
                    FileMgmt.Manager.UpdateFile(file, data);
                    break;
                case "CreateFile":
                    FileMgmt.Manager.CreateFile(dataParams[1]);
                    break;
                case "CreateFolder":
                    FileMgmt.Manager.CreateDirectory(dataParams[1]);
                    break;
                case "DeleteFile":
                    FileMgmt.Manager.DeleteFile(dataParams[1]);
                    break;
                case "DeleteFolder":
                    FileMgmt.Manager.DeleteDirectory(dataParams[1]);
                    break;
            }
        }
    }
}
