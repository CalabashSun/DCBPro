using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DCBProject.Util
{
    /// <summary>
    /// 和WX.exe驱动消息交互时做的读写操作类
    /// </summary>
    public class FileUtil
    {
        /// <summary>
        ///返回WX.exe 时的写文件操作
        /// </summary>
        /// <param name="path"></param>
        /// <param name="str"></param>
        public static void WriteContent(string path, string str)
        {
            StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default);
            sw.Write(str);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// WX.exe发送消息来时，读文件的操作
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> getContent(string path)
        {
            List<string> list = new List<string>();
            using (StreamReader streamReader = new StreamReader(path, Encoding.Default))
            {
                string item;
                while ((item = streamReader.ReadLine()) != null)
                {
                    list.Add(item);
                }
                streamReader.Close();
                streamReader.Dispose();
                return list;
            }
        }
    }
}
