using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenZip;

namespace Other
{
    class MiscFunctions
    {
        public static string StringCompressBytes(string original)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(original);
            byte[] compressedBytes = SevenZipCompressor.CompressBytes(byteData);
            string bytes = "";
            foreach (byte by in compressedBytes)
            {
                bytes += " " + by;
            }
            bytes = bytes.Substring(1);
            return bytes;
        }

        public static string StringDecompressBytes(string bytes)
        {
            string[] splitStringBytes = bytes.Split(' ');
            byte[] restoredBytes = new byte[splitStringBytes.Length];
            for (int i = 0; i < splitStringBytes.Length; i++)
            {
                int byteNum = Convert.ToInt32(splitStringBytes[i]);
                restoredBytes[i] = (byte)byteNum;
            }
            string str = Encoding.UTF8.GetString(SevenZipExtractor.ExtractBytes(restoredBytes));
            return str;
        }

        public static bool LiveClientResponse(string data)
        {
            if (data.Contains("RefreshFile") || data.Contains("CreateFile") || data.Contains("CreateFolder") || data.Contains("DeleteFile") || data.Contains("DeleteFolder"))
            {
                return true;
            }
            return false;
        }
    }
}
