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
                    string data = e.Data.Substring(e.Data.IndexOf(dataParams[2]));
                    string fileToModify = Misc.Config.fullProjectPath + @"\" + file;
                    if (FileMgmt.Manager.ReadFile(fileToModify) != data)
                    {
                        Misc.Global.pauseLiveUpdate = true;
                        FileMgmt.Manager.UpdateFile(fileToModify, data);
                        for (int i = 0; i < ActionRunner.Index.indexedFileList.Count; i++)
                        {
                            if (ActionRunner.Index.indexedFileList[i].path == fileToModify)
                            {
                                ActionRunner.Index.indexedFileList.RemoveAt(i);
                                ActionRunner.Index.indexedFileList.Add(new ActionRunner.IndexedFiles
                                {
                                    path = fileToModify,
                                    hash = FileMgmt.Manager.CheckFileHash(fileToModify)
                                });
                            }
                        }
                        Console.WriteLine("Refresh - File Data");
                        Misc.Global.pauseLiveUpdate = false;
                    }
                    break;
                case "CreateFile":
                    string fileToCreate = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    if (!FileMgmt.Manager.FileExists(fileToCreate))
                    {
                        Misc.Global.pauseLiveUpdate = true;
                        FileMgmt.Manager.CreateFile(fileToCreate);
                        ActionRunner.Index.indexedFileList.Add(new ActionRunner.IndexedFiles
                        {
                            path = fileToCreate,
                            hash = FileMgmt.Manager.CheckFileHash(fileToCreate)
                        });
                        Misc.Global.pauseLiveUpdate = false;
                        Console.WriteLine("New File Addition");
                    }
                    break;
                case "CreateFolder":
                    string folderToCreate = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    if (!FileMgmt.Manager.DirExists(folderToCreate))
                    {
                        Misc.Global.pauseLiveUpdate = true;
                        FileMgmt.Manager.CreateDirectory(folderToCreate);
                        ActionRunner.Index.indexedDirectoriesList.Add(folderToCreate);
                        Misc.Global.pauseLiveUpdate = false;
                        Console.WriteLine("New Folder Addition");
                    }
                    break;
                case "DeleteFile":
                    string fileToRemove = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    if (FileMgmt.Manager.FileExists(fileToRemove))
                    {
                        Misc.Global.pauseLiveUpdate = true;
                        FileMgmt.Manager.DeleteFile(fileToRemove);
                        for (int i = 0; i < ActionRunner.Index.indexedFileList.Count; i++)
                        {
                            if (ActionRunner.Index.indexedFileList[i].path == fileToRemove)
                            {
                                ActionRunner.Index.indexedFileList.RemoveAt(i);
                                break;
                            }
                        }
                        Misc.Global.pauseLiveUpdate = false;
                        Console.WriteLine("File Deletion");
                    }
                    break;
                case "DeleteFolder":
                    string folderToRemove = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    if (FileMgmt.Manager.DirExists(folderToRemove))
                    {
                        Misc.Global.pauseLiveUpdate = true;
                        FileMgmt.Manager.DeleteDirectory(folderToRemove);
                        ActionRunner.Index.indexedDirectoriesList.Remove(folderToRemove);
                        Misc.Global.pauseLiveUpdate = false;
                        Console.WriteLine("Folder Deletion");
                    }
                    break;
            }
        }
    }
}
