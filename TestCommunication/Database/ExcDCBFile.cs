using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DCBProject.Models;
using DCBProject.Util;
using TestCommunication.Models;

namespace DCBProject.Database
{
    public static class ExcDCBFile
    {
        public static void CreatPackageFile()
        {
            //生成菜品套餐TXT
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = "select * from Eat_XFBM_DP where IsPackage=1 and DP_IsDelete=0 order by Dp_TcCode";
                var packages = conn.Query<Eat_XFBM_DP>(sqlString).ToList();
                var packageChildString = "select * from Eat_XFBM_Package where Pac_Enable=1 and Pac_IsDel=0";
                var pacChilds = conn.Query<Eat_XFBM_Package>(packageChildString).ToList();
                var packageCodeString = "select * from Eat_XFBM_PackageCate where Cate_Enable=1 and Cate_IsDel=0";
                var pacCodes= conn.Query<Eat_XFBM_PackageCate>(packageCodeString).ToList();
                if (packages.Count > 0)
                {
               
                    var packageFile = LogsHelper.AppPath() + "\\TXT\\菜品套餐表.txt";
                    FileStream fs;
                    StreamWriter sw;
                    if (System.IO.File.Exists(packageFile))
                    {
                        //存在则删除文件
                        File.Delete(packageFile);
                    }
                    fs = new FileStream(packageFile, FileMode.Create, FileAccess.Write);
                    sw=new StreamWriter(fs,Encoding.GetEncoding("GB2312"));


                    var packageDetailFile = LogsHelper.AppPath() + "\\TXT\\菜品套餐内容表.txt";
                    FileStream fs2;
                    StreamWriter sw2;
                    if (System.IO.File.Exists(packageDetailFile))
                    {
                        //存在则删除文件
                        File.Delete(packageDetailFile);
                    }
                    fs2 = new FileStream(packageDetailFile, FileMode.Create, FileAccess.Write);
                    sw2 = new StreamWriter(fs2, Encoding.GetEncoding("GB2312"));

                    foreach (var eatXfbmDp in packages)
                    {
                        sw2.WriteLine(eatXfbmDp.Dp_TcCode + HandleFiveCode(eatXfbmDp.Number_Code) 
                                      + CompletionString("1", 9)
                                      + CompletionString(eatXfbmDp.DP_Price.Value.ToString("0.00"), 9)
                                      + CompletionString(eatXfbmDp.DP_Unit, 4)
                                      + "1"
                                      + "77");
                        var packageChild = pacChilds.Where(p => p.Pac_DPPac_ID == eatXfbmDp.DP_ID);
                        foreach (var eatXfbmPackage in packageChild)
                        {
                            sw2.WriteLine(eatXfbmDp.Dp_TcCode+ HandleFiveCode(eatXfbmPackage.Child_DP_Number)+CompletionString(eatXfbmPackage.Child_DP_Count.ToString("0"),9)
                                          +CompletionString("0.00",9)
                                          +CompletionString(eatXfbmPackage.Child_DP_Unit,4)
                                          +(eatXfbmPackage.Child_IsChoosed?"1":"0")
                                          +pacCodes.First(p=>p.Cate_ID==eatXfbmPackage.Pac_Cate_ID).Cate_Code);

                        }

                        sw.WriteLine(eatXfbmDp.Dp_TcCode+ CompletionString(eatXfbmDp.DP_Name,20,1));
                    }
                    sw.Close();
                    fs.Close();
                    sw2.Close();
                    fs2.Close();
                }
            }
        }

        public static void CreatPackageByView()
        {
            //生成菜品套餐TXT
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = "select * from View_DCB_CPTC";
                var packages = conn.Query<View_DCB_CPTC>(sqlString).ToList();
                var packageChildString = "select * from View_DCB_TCNR";
                var pacChilds = conn.Query<View_DCB_TCNR>(packageChildString).ToList();
                if (packages.Count > 0)
                {

                    var packageFile = LogsHelper.AppPath() + "\\TXT\\菜品套餐表.txt";
                    FileStream fs;
                    StreamWriter sw;
                    if (System.IO.File.Exists(packageFile))
                    {
                        //存在则删除文件
                        File.Delete(packageFile);
                    }
                    fs = new FileStream(packageFile, FileMode.Create, FileAccess.Write);
                    sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));


                    var packageDetailFile = LogsHelper.AppPath() + "\\TXT\\菜品套餐内容表.txt";
                    FileStream fs2;
                    StreamWriter sw2;
                    if (System.IO.File.Exists(packageDetailFile))
                    {
                        //存在则删除文件
                        File.Delete(packageDetailFile);
                    }
                    fs2 = new FileStream(packageDetailFile, FileMode.Create, FileAccess.Write);
                    sw2 = new StreamWriter(fs2, Encoding.GetEncoding("GB2312"));

                    foreach (var eatXfbmDp in packages)
                    {
                        var packageChild = pacChilds.Where(p => p.tch == eatXfbmDp.bh);
                        foreach (var eatXfbmPackage in packageChild)
                        {
                            sw2.WriteLine(eatXfbmPackage.tch+eatXfbmPackage.ch+eatXfbmPackage.sl+eatXfbmPackage.dj + eatXfbmPackage.dw+ eatXfbmPackage.qsxz+eatXfbmPackage.tcz);

                        }
                        sw.WriteLine(eatXfbmDp.bh+eatXfbmDp.mc);
                    }
                    sw.Close();
                    fs.Close();
                    sw2.Close();
                    fs2.Close();
                }
            }
        }

        public static void CreatDishesFile()
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = "select * from Eat_XFBM_DP where DP_IsDelete=0";
                var dpInfos = conn.Query<Eat_XFBM_DP>(sqlString).ToList();
                var cateString = "select * from Eat_XfKind where Eat_XfKind_Parent_ID ='' and Eat_XfKind_IsDelete=0 and Eat_XfKind_IsEnable=1";
                var cateInfos = conn.Query<Eat_XfKind>(cateString).ToList();
                if (dpInfos.Count > 0)
                {
                    var dpInfoFile = LogsHelper.AppPath() + "\\TXT\\菜品表.TXT";
                    FileStream fs;
                    StreamWriter sw;
                    if (System.IO.File.Exists(dpInfoFile))
                    {
                        //存在则删除文件
                        File.Delete(dpInfoFile);
                    }
                    fs = new FileStream(dpInfoFile, FileMode.Create, FileAccess.Write);
                    sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
                    foreach (var eatXfbmDp in dpInfos)
                    {
                        sw.WriteLine(HandleFiveCode(eatXfbmDp.Number_Code) 
                                     +cateInfos.First(p=>p.Eat_XfKind_ID==eatXfbmDp.DP_KindIdP).code
                                     + CompletionString(eatXfbmDp.DP_Name, 20,1)
                                     +CompletionString(eatXfbmDp.DP_Price.Value.ToString("0.00"),9)
                                     +CompletionString(eatXfbmDp.DP_Unit,4)
                                     +CompletionString(eatXfbmDp.DP_Unit, 4)
                                     +"0"
                                     + CompletionString("", 45)
                                     + CompletionString(eatXfbmDp.DP_PY, 10));
                    }
                    sw.Close();
                    fs.Close();

                }
            }
        }

        public static string HandleFiveCode(string cpCode)
        {
            if (cpCode.Length > 5)
            {
                return cpCode.Substring(cpCode.Length - 5);
            }
            else if(cpCode.Length==5)
            {
                return cpCode;
            }
            else
            {
                cpCode += "     ";
                return cpCode.Substring(0,5);
            }
        }

        public static string CompletionString(string original,int length,int type=2)
        {
            var space = "";
            for (int i = 0; i < length; i++)
            {
                space += " ";
            }

            var result = original+space;
            var temp = result;
            int j=0, k = 1;
            for (int i = 0; i < temp.Length; i++)
            {
                if (Regex.IsMatch(temp.Substring(i, 1), @"[\u4e00-\u9fa5]")||temp.Substring(i,1)=="（" || temp.Substring(i, 1) == "）")
                {
                    j += 2;
                }
                else
                {
                    j += 1;
                }

                if (j < length)
                {
                    k += 1;
                }

                if (j >= length)
                {
                    temp = temp.Substring(0, k);
                    break;
                }
            }
            //把内容和空格替换
            if (type == 2)
            {
                var frontSpace = "";
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp.Substring(i, 1) == " ")
                    {
                        frontSpace += " ";
                    }
                }
                temp = frontSpace + temp.TrimEnd();
            }

            
            return temp;
        }
    }
}
