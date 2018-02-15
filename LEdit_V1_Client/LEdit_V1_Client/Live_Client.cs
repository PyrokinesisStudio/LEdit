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
                // TO DO
                case "RefreshFile":
                    string file = dataParams[1];
                    string fileToModify = Misc.Config.fullProjectPath + @"\" + file;
                    string data = e.Data.Substring(e.Data.IndexOf(dataParams[2]));
                    if (FileMgmt.Manager.ReadFile(fileToModify) != data)
                    {
                        Watcher.Watcher.ignore.Add(fileToModify);
                        FileMgmt.Manager.UpdateFile(fileToModify, data);
                    }
                    Console.WriteLine("File Refresh");
                    break;
                case "CreateFile":
                    string fileToCreate = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    if (!FileMgmt.Manager.FileExists(fileToCreate))
                    {
                        Watcher.Watcher.ignore.Add(fileToCreate);
                        FileMgmt.Manager.CreateFile(fileToCreate);
                        Console.WriteLine("New File Addition");
                    }
                    break;
                case "CreateFolder":
                    string folderToCreate = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    if (!FileMgmt.Manager.DirExists(folderToCreate))
                    {
                        Watcher.Watcher.ignore.Add(folderToCreate);
                        FileMgmt.Manager.CreateDirectory(folderToCreate);
                        Console.WriteLine("New Folder Addition");
                    }
                    break;
                case "DeleteFile":
                    string fileToRemove = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    if (FileMgmt.Manager.FileExists(fileToRemove))
                    {
                        Watcher.Watcher.ignore.Add(fileToRemove);
                        FileMgmt.Manager.DeleteFile(fileToRemove);
                        Console.WriteLine("File Deletion");
                    }
                    break;
                case "DeleteFolder":
                    string folderToRemove = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    if (FileMgmt.Manager.DirExists(folderToRemove))
                    {
                        Watcher.Watcher.ignore.Add(folderToRemove);
                        FileMgmt.Manager.DeleteDirectory(folderToRemove);
                        Console.WriteLine("Folder Deletion");
                    }
                    break;
            }
        }
    }
}
