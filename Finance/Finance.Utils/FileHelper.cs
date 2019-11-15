using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Finance.Utils
{
    public class FileHelper
    {
        public static void SafeRead(Stream stream, byte[] data)
        {
            int offset = 0;
            int remaining = data.Length;
            // 只要有剩余的字节就不停的读
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                    throw new EndOfStreamException("文件读取到" + read.ToString() + "失败！");
                // 减少剩余的字节数
                remaining -= read;
                // 增加偏移量
                offset += read;
            }
        }

        //private BitmapImage LoadResources(string name)
        //{
        //    try
        //    {
        //        BitmapImage myBitmapImage = new BitmapImage();
        //        myBitmapImage.BeginInit();
        //        FileStream fs = File.OpenRead(@"Icons\" + name);
        //        byte[] data = new byte[fs.Length];
        //        int offset = 0;
        //        int remaining = data.Length;
        //        只要有剩余的字节就不停的读
        //        while (remaining > 0)
        //        {
        //            int read = fs.Read(data, offset, remaining);
        //            if (read <= 0)
        //                throw new EndOfStreamException("文件读取到" + read.ToString() + "失败！");
        //            减少剩余的字节数
        //           remaining -= read;
        //            增加偏移量
        //           offset += read;
        //        }
        //        myBitmapImage.StreamSource = new MemoryStream(data);
        //        myBitmapImage.EndInit();
        //        return myBitmapImage;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message + "\r\n" + ex.ToString());
        //    }
        //    return null;
        //}

        public static string Read(string path)
        {
            StreamReader objReader = new StreamReader(path);
            string sLine = "";
            StringBuilder sb = new StringBuilder();
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null && !sLine.Equals(""))
                    sb.AppendLine(sLine);
            }
            objReader.Close();
            return sb.ToString();
        }

        public static void Write(string str, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(str);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 获取文件价下所有的文件名
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern">*.txt</param>
        /// <returns></returns>
        public static List<string> GetFilesName(string path,string searchPattern)
        {
            var files = Directory.GetFiles(path, searchPattern);
            return new List<string>(files);
        }

        public static void FileExpiry(string path, string searchPattern, long expirySeconds)
        {
            var lst = GetFilesName(path, searchPattern);
            lst.ForEach(f=> {
                FileInfo fi = new FileInfo(f);
                TimeSpan ts = DateTime.Now - fi.LastWriteTime;               
                if (ts.TotalSeconds >= expirySeconds)
                {
                    fi.Delete();
                }
            });
        }

        public static bool FileExist(string fileName)
        {
            return System.IO.File.Exists(fileName);
        }


        public static void CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static bool CopyFile(string srcFileName, string destFileName)
        {
            if (FileExist(srcFileName))
            {
                CheckPath(destFileName.Substring(0, destFileName.LastIndexOf('\\')));
                File.Copy(srcFileName, destFileName, true);
                return true;
            }
            return false;
        }

        public static string FileSuffix(string fileName)
        {
            return Path.GetExtension(fileName);
        }
    }
}
