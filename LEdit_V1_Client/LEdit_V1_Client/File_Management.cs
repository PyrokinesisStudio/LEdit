using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace FileMgmt
{
    class Manager
    {
        static int tries = 0;

        public static void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
        }

        public static bool DeleteDirectory(string directory)
        {
            try
            {
                Directory.Delete(directory, true);
                tries = 0;
                return true;
            }
            catch (System.IO.IOException)
            {
                if (tries < 25)
                {
                    Thread.Sleep(250);
                    DeleteDirectory(directory);
                }
                else
                {
                    tries = 0;
                    return false;
                }
            }
            return false;
        }

        public static void CreateFile(string file)
        {
            File.Create(file);
        }

        public static bool DeleteFile(string file)
        {
            // Wait for file to become available
            while (!Checker.FileAvailable(file))
            {
                if (tries != 25)
                {
                    Thread.Sleep(100);
                    tries++;
                }
                else
                {
                    tries = 0;
                    return false;
                }
            }
            // Continue
            File.Delete(file);
            tries = 0;
            return true;
        }

        public static bool UpdateFile(string file, string data)
        {
            // Wait for file to become available
            while (!Checker.FileAvailable(file))
            {
                if (tries != 25)
                {
                    Thread.Sleep(100);
                    tries++;
                }
                else
                {
                    tries = 0;
                    return false;
                }
            }
            // Continue
            File.WriteAllText(file, data);
            tries = 0;
            return true;
        }
    }

    public class Checker
    {
        public static bool FileAvailable(string file)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return true;
        }
    }
}
