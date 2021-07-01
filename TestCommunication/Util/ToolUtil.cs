using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCBProject.Util
{
    public static class ToolUtil
    {
        public static string GenerageNumber()
        {
            string sarNum = DateTime.Now.ToString("yyyyMMddHHmmssms");
            Random ra=new Random();
            sarNum += ra.Next(10, 99);
            return sarNum;
        }

        public static string IsNull(string data)
        {
            return string.IsNullOrEmpty(data) ? "" : data;
        }

        public static string ConvertToStr(List<string> list)
        {
            try
            {
                var sb=new StringBuilder();
                foreach (var t in list)
                {
                    sb.Append("'");
                    sb.Append(t);
                    sb.Append("'");
                    sb.Append(",");
                }
                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary> 
        /// 获取时间戳 
        /// </summary> 
        /// <returns></returns> 
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 生成12位随机数
        /// </summary>
        /// <returns></returns>
        public static string getRandom()
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            Random random = new Random(iSeed);
            string str = "";
            for (int i = 0; i < 12; i++)
            {
                str += chars[random.Next(chars.Length)];
            }

            return str;
        }

        public static string SubstringByte(string text, int startIndex, int length)
        {
            var _encoding = System.Text.Encoding.GetEncoding("GB2312");
            byte[] bytes = _encoding.GetBytes(text);
            return _encoding.GetString(bytes, startIndex, length);
        }
    }
}
