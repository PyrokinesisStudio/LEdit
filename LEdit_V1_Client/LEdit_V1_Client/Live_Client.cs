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
                    Misc.Global.pauseLiveUpdate = true;
                    string fileToModify = Misc.Config.fullProjectPath + @"\" + file;
                    FileMgmt.Manager.UpdateFile(fileToModify, data);
                    for (int i = 0; i < ActionRunner.Index.indexedFileList.Count; i++)
                    {
                        if (ActionRunner.Index.indexedFileList[i].path == fileToModify)
                        {
                            //ActionRunner.Index.indexedFileList[i].hash = FileMgmt.Manager.CheckFileHash(fileToModify);
                            ActionRunner.Index.indexedFileList.RemoveAt(i);
                            ActionRunner.Index.indexedFileList.Add(new ActionRunner.IndexedFiles {
                                hash = FileMgmt.Manager.CheckFileHash(fileToModify),
                                path = fileToModify
                            });
                        }
                    }
                    Misc.Global.pauseLiveUpdate = false;
                    Console.WriteLine("Refresh - File Data");
                    break;
                case "CreateFile":
                    Misc.Global.pauseLiveUpdate = true;
                    string fileToCreate = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    FileMgmt.Manager.CreateFile(fileToCreate);
                    ActionRunner.Index.indexedFileList.Add(new ActionRunner.IndexedFiles
                    {
                        path = fileToCreate,
                        hash = FileMgmt.Manager.CheckFileHash(fileToCreate)
                    });
                    Misc.Global.pauseLiveUpdate = false;
                    Console.WriteLine("New File Addition");
                    break;
                case "CreateFolder":
                    Misc.Global.pauseLiveUpdate = true;
                    string folderToCreate = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    FileMgmt.Manager.CreateDirectory(folderToCreate);
                    ActionRunner.Index.indexedDirectoriesList.Add(folderToCreate);
                    Misc.Global.pauseLiveUpdate = false;
                    Console.WriteLine("New Folder Addition");
                    break;
                case "DeleteFile":
                    Misc.Global.pauseLiveUpdate = true;
                    string folderToDelete = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    FileMgmt.Manager.DeleteFile(folderToDelete);
                    for (int i = 0; i < ActionRunner.Index.indexedFileList.Count; i++)
                    {
                        if (ActionRunner.Index.indexedFileList[i].path == folderToDelete)
                        {
                            ActionRunner.Index.indexedFileList.RemoveAt(i);
                            break;
                        }
                    }
                    Misc.Global.pauseLiveUpdate = false;
                    Console.WriteLine("File Deletion");
                    break;
                case "DeleteFolder":
                    Misc.Global.pauseLiveUpdate = true;
                    string folderToRemove = Misc.Config.fullProjectPath + @"\" + dataParams[1];
                    FileMgmt.Manager.DeleteDirectory(folderToRemove);
                    ActionRunner.Index.indexedDirectoriesList.Remove(folderToRemove);
                    Misc.Global.pauseLiveUpdate = false;
                    Console.WriteLine("Folder Deletion");
                    break;
            }
        }
    }
}
