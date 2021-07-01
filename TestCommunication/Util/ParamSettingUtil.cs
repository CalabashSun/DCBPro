using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DCBProject.Database;

namespace DCBProject.Util
{
    public class ParamSettingUtil
    {
        /// <summary>
        /// 生成基站文件
        /// </summary>
        public static void GenerateBaseStation()
        {
            var stations = StaionInfo.Stations();
            var responseText = "";
            foreach (var dcbStation in stations)
            {
                responseText += dcbStation.Station_No + " " + ToolUtil.IsNull(dcbStation.Station_Port) +
                               ToolUtil.IsNull(dcbStation.Station_Ip)+ "\r\n";
            }
            string fileName = "基站.txt";
            FileUtil.WriteContent(Application.StartupPath + @"\TXT\" + fileName, responseText);
        }
        /// <summary>
        /// 生成点菜宝文件
        /// </summary>
        public static void GenerateDcb()
        {
            var dcbs = StaionInfo.AllDetails();
            var responseText = "";
            foreach (var dcbDetail in dcbs)
            {
                responseText += dcbDetail.Detail_No + " " + dcbDetail.Station_No+ "\r\n";
            }
            string fileName = "点菜机.txt";
            FileUtil.WriteContent(Application.StartupPath + @"\TXT\" + fileName, responseText);
        }
    }
}
