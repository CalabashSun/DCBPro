using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using DCBProject.Database;
using DCBProject.Models;

namespace DCBProject.Util
{
    public class HttpUtil
    {
        private static RestSharp.RestClient client = new RestSharp.RestClient(DBConfig.posApi);

        public static string Get(string url)
        {
            RestRequest request = new RestRequest(url, Method.GET);
            string str = client.Get(request).Content;
            return str;
        }

        public static string Post(string url, string content)
        {
            try
            {
                LogsHelper.WriteLog(content);
                var request = new RestRequest(url, Method.POST);
                request.Timeout = 10000;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                request.AddParameter("application/json",content, ParameterType.RequestBody);
                var str = client.Execute(request);
                return str.Content;
            }
            catch (Exception ex)
            {
                return "连接服务器出错：\r\n" + ex.Message;
            }
        }

        public static string PostForm(string url, string content)
        {
            var request = new RestRequest(url, Method.POST);
            var str = client.Execute(request);
            return str.Content;
        }
        /// <summary>
        /// 点单  ************************ 废弃方法 ****************************
        /// </summary>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string PostAddOrder(string url, OrderModel model,out decimal currentTotalMoeny)
        {
            var cpDetails = new List<Eat_OrderDoc_Detail>();
            var cpOrderInfo = new Eat_OrderDoc();
            var docId = model.OrderId;

            //构建数据
            var blNumbers = string.Empty;
            var cpRemarks = string.Empty;

            foreach (var modelDetail in model.Details)
            {
                blNumbers += modelDetail.CpNumber + ",";
                cpRemarks += modelDetail.CpRemarks + ",";
            }
            blNumbers = blNumbers.Substring(0, blNumbers.Length - 1);

            var result = "";
            //获取所点菜品
            var cpInfos = DCBOperations.CpInfos(blNumbers);
            var remarkInfo = DCBOperations.CpRemarks();
            var packageRecords=new List<PackageRecord>();


            foreach (var modelDetail in model.Details)
            {
                var cpDetail = new Eat_OrderDoc_Detail(); 
                //构建数据
                var currentCp = cpInfos.FirstOrDefault(p => p.BL_Code == modelDetail.CpNumber);

                if (currentCp != null)
                {
                    var dpOrderDetail = new Eat_OrderDoc_Detail();

                    dpOrderDetail.Detail_ID =ToolUtil.getRandom();
                    dpOrderDetail.Doc_ID = docId;
                    dpOrderDetail.Doc_SeatNumber = model.SeatId;
                    dpOrderDetail.Cp_Quantity = Convert.ToDecimal(modelDetail.CpCount);
                    dpOrderDetail.Actual_Quantity = Convert.ToDecimal(modelDetail.CpCount);
                    dpOrderDetail.Cp_Code = currentCp.Number_Code;
                    dpOrderDetail.Cp_Name = currentCp.DP_Name;
                    if (modelDetail.CpRemarks.Length >= 1)
                    {
                        dpOrderDetail.Detail_Remarks = HandleRemarks(modelDetail.CpRemarks, remarkInfo);
                    }
                    else
                    {
                        dpOrderDetail.Detail_Remarks = "";
                    }
                    dpOrderDetail.Cp_Printer = currentCp.DP_PrinterId;
                    dpOrderDetail.Cp_IsSingleCut = currentCp.Dp_IsSingleCut == null ? false : currentCp.Dp_IsSingleCut.Value;
                    dpOrderDetail.Detail_AddTime = DateTime.Now;
                    dpOrderDetail.Doc_Remarks = HandleRemarks(model.Remarks, remarkInfo);

                    //如果为套餐
                    if (!string.IsNullOrEmpty(modelDetail.PackageCode))
                    {
                        var nowPackage=new PackageRecord();
                        var cuurentPackage =packageRecords.FirstOrDefault(p => p.packageCode == modelDetail.PackageCode);
                        if (cuurentPackage == null)
                        {
                            var createPackage = new PackageRecord
                            {
                                packageId = ToolUtil.GetTimeStamp(),
                                packageCode = modelDetail.PackageCode
                            };
                            packageRecords.Add(createPackage);
                            nowPackage = createPackage;
                        }
                        else
                        {
                            nowPackage = cuurentPackage;
                        }
                        //如果为套餐父级
                        if (currentCp.Dp_TcCode == modelDetail.PackageCode)
                        {
                            dpOrderDetail.Cp_UnitPrice = currentCp.DP_Price.Value;
                            dpOrderDetail.Cp_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                            dpOrderDetail.Cp_Actual_UnitPrice = currentCp.DP_Price.Value;
                            dpOrderDetail.Cp_Actual_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                            dpOrderDetail.Cp_Hide_UnitPrice = 0.00m;
                            dpOrderDetail.Cp_Hide_SumPrice = 0.00m;
                            if (string.IsNullOrEmpty(nowPackage.packageName))
                            {
                                dpOrderDetail.Package_Name = DCBOperations.PackageName(modelDetail.PackageCode);
                            }
                            dpOrderDetail.Detail_IsPackage_Parent = true;
                            model.TotalMoney = (Convert.ToDecimal(model.TotalMoney) + Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price)).ToString("0.00");
                            model.TotalCount = (Convert.ToInt32(model.TotalCount) + Convert.ToInt32(modelDetail.CpCount)).ToString();
                        }
                        else
                        {//套餐子级
                            dpOrderDetail.Cp_UnitPrice = 0.00m;
                            dpOrderDetail.Cp_SumPrice = 0.00m;
                            dpOrderDetail.Cp_Actual_UnitPrice = 0.00m;
                            dpOrderDetail.Cp_Actual_SumPrice = 0.00m;
                            dpOrderDetail.Cp_Hide_UnitPrice = currentCp.DP_Price.Value;
                            dpOrderDetail.Cp_Hide_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                            if (string.IsNullOrEmpty(nowPackage.packageName))
                            {
                                dpOrderDetail.Package_Name = DCBOperations.PackageName(modelDetail.PackageCode);
                            }
                            dpOrderDetail.Detail_IsPackage_Parent = false;
                        }
                        dpOrderDetail.Detail_IsPackage = true;
                        dpOrderDetail.Package_ID= nowPackage.packageId;
                        dpOrderDetail.Package_Name =nowPackage.packageName;
                        cpDetails.Add(dpOrderDetail);
                    }
                    else//如果不是套餐
                    {
                        dpOrderDetail.Cp_UnitPrice = currentCp.DP_Price.Value;
                        dpOrderDetail.Cp_SumPrice= Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                        dpOrderDetail.Cp_Actual_UnitPrice = currentCp.DP_Price.Value;
                        dpOrderDetail.Cp_Actual_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                        dpOrderDetail.Cp_Hide_UnitPrice = currentCp.DP_Price.Value;
                        dpOrderDetail.Cp_Hide_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                        dpOrderDetail.Package_ID = "";
                        dpOrderDetail.Detail_IsPackage = false;
                        dpOrderDetail.Detail_IsPackage_Parent = false;
                        dpOrderDetail.Package_Name = "";
                        cpDetails.Add(dpOrderDetail);

                        model.TotalMoney =(Convert.ToDecimal(model.TotalMoney) + Convert.ToDecimal(modelDetail.CpCount) *Convert.ToDecimal(currentCp.DP_Price)).ToString("0.00");
                        model.TotalCount = (Convert.ToInt32(model.TotalCount) + Convert.ToInt32(modelDetail.CpCount)).ToString();
                    }
                }
                else
                {
                    result = modelDetail.CpNumber + "菜品信息不存在";
                    break;
                }
            }

            currentTotalMoeny =Convert.ToDecimal(model.TotalMoney);
            cpOrderInfo.Doc_ID = docId;
            cpOrderInfo.Doc_SeatNumber = model.SeatId;
            cpOrderInfo.Doc_People = string.IsNullOrEmpty(model.Renshu)?1:Convert.ToInt32(model.Renshu);
            cpOrderInfo.Doc_StartTime = DateTime.Now;
            cpOrderInfo.Doc_Status = 1;
            cpOrderInfo.Doc_Remarks = model.Remarks;
            cpOrderInfo.Doc_DpCount = Convert.ToInt32(model.TotalCount);
            cpOrderInfo.Doc_DpSum = currentTotalMoeny;
            cpOrderInfo.Doc_OpenTime = DateTime.Now;



            if (result != "")
            {
                //直接返回错误结果
                return result;
            }
            else
            {
                DCBOperations.AddOrder(cpDetails, cpOrderInfo);
                return "success";
            }
        }
        /// <summary>
        /// 加菜 ************************ 废弃方法 ****************************
        /// </summary>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string PostAddFood(string url, OrderModel model,out decimal totalMoeny)
        {
            var cpDetails = new List<Eat_OrderDoc_Detail>();
            var cpOrderInfo = new Eat_OrderDoc();
            var docId = model.OrderId;

            //构建数据
            var blNumbers = string.Empty;
            var cpRemarks = string.Empty;

            foreach (var modelDetail in model.Details)
            {
                blNumbers += modelDetail.CpNumber + ",";
                cpRemarks += modelDetail.CpRemarks + ",";
            }
            blNumbers = blNumbers.Substring(0, blNumbers.Length - 1);

            var result = "";
            //获取所点菜品
            var cpInfos = DCBOperations.CpInfos(blNumbers);
            var remarkInfo = DCBOperations.CpRemarks();
            var packageRecords = new List<PackageRecord>();


            foreach (var modelDetail in model.Details)
            {
                var cpDetail = new Eat_OrderDoc_Detail();
                //构建数据
                var currentCp = cpInfos.FirstOrDefault(p => p.BL_Code == modelDetail.CpNumber);

                if (currentCp != null)
                {
                    var dpOrderDetail = new Eat_OrderDoc_Detail();
                    dpOrderDetail.Detail_ID = ToolUtil.getRandom();
                    dpOrderDetail.Doc_ID = docId;
                    dpOrderDetail.Doc_SeatNumber = model.SeatId;
                    dpOrderDetail.Cp_Quantity = Convert.ToDecimal(modelDetail.CpCount);
                    dpOrderDetail.Actual_Quantity = Convert.ToDecimal(modelDetail.CpCount);
                    dpOrderDetail.Cp_Code = currentCp.Number_Code;
                    dpOrderDetail.Cp_Name = currentCp.DP_Name;
                    if (modelDetail.CpRemarks.Length >= 1)
                    {
                        dpOrderDetail.Detail_Remarks = HandleRemarks(modelDetail.CpRemarks, remarkInfo);
                    }
                    else
                    {
                        dpOrderDetail.Detail_Remarks = "";
                    }
                    dpOrderDetail.Cp_Printer = currentCp.DP_PrinterId;
                    dpOrderDetail.Cp_IsSingleCut = currentCp.Dp_IsSingleCut == null ? false : currentCp.Dp_IsSingleCut.Value;
                    dpOrderDetail.Detail_AddTime = DateTime.Now;
                    dpOrderDetail.Doc_Remarks = HandleRemarks(model.Remarks, remarkInfo);

                    //如果为套餐
                    if (!string.IsNullOrEmpty(modelDetail.PackageCode))
                    {
                        var nowPackage = new PackageRecord();
                        var cuurentPackage = packageRecords.FirstOrDefault(p => p.packageCode == modelDetail.PackageCode);
                        if (cuurentPackage == null)
                        {
                            var createPackage = new PackageRecord
                            {
                                packageId = ToolUtil.GetTimeStamp(),
                                packageCode = modelDetail.PackageCode
                            };
                            packageRecords.Add(createPackage);
                            nowPackage = createPackage;
                        }
                        else
                        {
                            nowPackage = cuurentPackage;
                        }
                        //如果为套餐父级
                        if (currentCp.Dp_TcCode == modelDetail.PackageCode)
                        {
                            dpOrderDetail.Cp_UnitPrice = currentCp.DP_Price.Value;
                            dpOrderDetail.Cp_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                            dpOrderDetail.Cp_Actual_UnitPrice = currentCp.DP_Price.Value;
                            dpOrderDetail.Cp_Actual_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                            dpOrderDetail.Cp_Hide_UnitPrice = 0.00m;
                            dpOrderDetail.Cp_Hide_SumPrice = 0.00m;
                            if (string.IsNullOrEmpty(nowPackage.packageName))
                            {
                                dpOrderDetail.Package_Name = DCBOperations.PackageName(modelDetail.PackageCode);
                            }
                            dpOrderDetail.Detail_IsPackage_Parent = true;
                            model.TotalMoney = (Convert.ToDecimal(model.TotalMoney) + Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price)).ToString("0.00");
                            model.TotalCount = (Convert.ToInt32(model.TotalCount) + Convert.ToInt32(modelDetail.CpCount)).ToString();
                        }
                        else
                        {//套餐子级
                            dpOrderDetail.Cp_UnitPrice = 0.00m;
                            dpOrderDetail.Cp_SumPrice = 0.00m;
                            dpOrderDetail.Cp_Actual_UnitPrice = 0.00m;
                            dpOrderDetail.Cp_Actual_SumPrice = 0.00m;
                            dpOrderDetail.Cp_Hide_UnitPrice = currentCp.DP_Price.Value;
                            dpOrderDetail.Cp_Hide_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                            if (string.IsNullOrEmpty(nowPackage.packageName))
                            {
                                dpOrderDetail.Package_Name = DCBOperations.PackageName(modelDetail.PackageCode);
                            }
                            dpOrderDetail.Detail_IsPackage_Parent = false;
                        }
                        dpOrderDetail.Detail_IsPackage = true;
                        dpOrderDetail.Package_ID = nowPackage.packageId;
                        dpOrderDetail.Package_Name = nowPackage.packageName;
                        cpDetails.Add(dpOrderDetail);
                    }
                    else//如果不是套餐
                    {
                        dpOrderDetail.Cp_UnitPrice = currentCp.DP_Price.Value;
                        dpOrderDetail.Cp_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                        dpOrderDetail.Cp_Actual_UnitPrice = currentCp.DP_Price.Value;
                        dpOrderDetail.Cp_Actual_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                        dpOrderDetail.Cp_Hide_UnitPrice = currentCp.DP_Price.Value;
                        dpOrderDetail.Cp_Hide_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                        dpOrderDetail.Package_ID = "";
                        dpOrderDetail.Detail_IsPackage = false;
                        dpOrderDetail.Detail_IsPackage_Parent = false;
                        dpOrderDetail.Package_Name = "";
                        cpDetails.Add(dpOrderDetail);

                        model.TotalMoney = (Convert.ToDecimal(model.TotalMoney) + Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price)).ToString("0.00");
                        model.TotalCount = (Convert.ToInt32(model.TotalCount) + Convert.ToInt32(modelDetail.CpCount)).ToString();
                    }
                }
                else
                {
                    result = modelDetail.CpNumber + "菜品信息不存在";
                    break;
                }
            }

            totalMoeny = Convert.ToDecimal(model.TotalMoney);
            cpOrderInfo.Doc_ID = docId;
            cpOrderInfo.Doc_SeatNumber = model.SeatId;
            cpOrderInfo.Doc_People = string.IsNullOrEmpty(model.Renshu) ? 0 : Convert.ToInt32(model.Renshu);
            cpOrderInfo.Doc_StartTime = DateTime.Now;
            cpOrderInfo.Doc_Status = 1;
            cpOrderInfo.Doc_Remarks = model.Remarks;
            cpOrderInfo.Doc_DpCount = Convert.ToInt32(model.TotalCount);
            cpOrderInfo.Doc_DpSum = totalMoeny;


            if (result != "")
            {
                //直接返回错误结果
                return result;
            }
            else
            {
                DCBOperations.AddFood(cpDetails, cpOrderInfo);
                return "success";
            }
        }


        /// <summary>
        /// 点单或者加菜
        /// </summary>
        /// <param name="AddType">添加类型 1：订单 2：加菜</param>
        /// <param name="model">订单实体类</param>
        /// <param name="totalMoeny">消费总金额</param>
        /// <returns></returns>
        public static string PostAddOrderOrFood(int AddType, OrderModel model, out decimal totalMoeny)
        {
            var cpDetails = new List<Eat_OrderDoc_Detail>();
            var cpOrderInfo = new Eat_OrderDoc();
            var docId = model.OrderId;

            //构建数据
            var blNumbers = string.Empty;
            var cpRemarks = string.Empty;

            foreach (var modelDetail in model.Details)
            {
                blNumbers += modelDetail.CpNumber + ",";
                cpRemarks += modelDetail.CpRemarks + ",";
            }
            blNumbers = blNumbers.Substring(0, blNumbers.Length - 1);

            var result = "";
            //获取所点菜品
            var cpInfos = DCBOperations.CpInfos(blNumbers);
            //判断是否存在沽清菜品 如果菜品已沽清直接返回失败
            var existBanCp = cpInfos.Count(p => p.DP_IsBan == true);
            if (existBanCp > 0)
            {
                //获取最近的一个沽清的菜品
                var banCpInfo = cpInfos.First(p => p.DP_IsBan == true);
                totalMoeny = 0.00m;
                return banCpInfo.DP_Name + " 已经被沽清了！";
            }
            else
            {
                var remarkInfo = DCBOperations.CpRemarks();
                var packageRecords = new List<PackageRecord>();
                foreach (var modelDetail in model.Details)
                {
                    var cpDetail = new Eat_OrderDoc_Detail();
                    //构建数据
                    var currentCp = cpInfos.FirstOrDefault(p => p.BL_Code == modelDetail.CpNumber);

                    if (currentCp != null)
                    {
                        var dpOrderDetail = new Eat_OrderDoc_Detail();
                        dpOrderDetail.Detail_ID = ToolUtil.getRandom();
                        dpOrderDetail.Doc_ID = docId;
                        dpOrderDetail.Doc_SeatNumber = model.SeatId;
                        dpOrderDetail.Cp_Quantity = Convert.ToDecimal(modelDetail.CpCount);
                        dpOrderDetail.Actual_Quantity = Convert.ToDecimal(modelDetail.CpCount);
                        dpOrderDetail.Cp_Code = currentCp.Number_Code;
                        dpOrderDetail.Cp_Name = currentCp.DP_Name;
                        if (modelDetail.CpRemarks.Length >= 1)
                        {
                            dpOrderDetail.Detail_Remarks = HandleRemarks(modelDetail.CpRemarks, remarkInfo);
                        }
                        else
                        {
                            dpOrderDetail.Detail_Remarks = "";
                        }
                        dpOrderDetail.Cp_Printer = currentCp.DP_PrinterId;
                        dpOrderDetail.Cp_IsSingleCut = currentCp.Dp_IsSingleCut == null ? false : currentCp.Dp_IsSingleCut.Value;
                        dpOrderDetail.Detail_AddTime = DateTime.Now;
                        dpOrderDetail.Doc_Remarks = HandleRemarks(model.Remarks, remarkInfo);

                        //如果为套餐
                        if (!string.IsNullOrEmpty(modelDetail.PackageCode))
                        {
                            var nowPackage = new PackageRecord();
                            var cuurentPackage = packageRecords.FirstOrDefault(p => p.packageCode == modelDetail.PackageCode);
                            if (cuurentPackage == null)
                            {
                                var createPackage = new PackageRecord
                                {
                                    packageId = ToolUtil.GetTimeStamp(),
                                    packageCode = modelDetail.PackageCode
                                };
                                packageRecords.Add(createPackage);
                                nowPackage = createPackage;
                            }
                            else
                            {
                                nowPackage = cuurentPackage;
                            }
                            //如果为套餐父级
                            if (currentCp.Dp_TcCode == modelDetail.PackageCode)
                            {
                                dpOrderDetail.Cp_UnitPrice = currentCp.DP_Price.Value;
                                dpOrderDetail.Cp_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                                dpOrderDetail.Cp_Actual_UnitPrice = currentCp.DP_Price.Value;
                                dpOrderDetail.Cp_Actual_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                                dpOrderDetail.Cp_Hide_UnitPrice = 0.00m;
                                dpOrderDetail.Cp_Hide_SumPrice = 0.00m;
                                if (string.IsNullOrEmpty(nowPackage.packageName))
                                {
                                    dpOrderDetail.Package_Name = DCBOperations.PackageName(modelDetail.PackageCode);
                                }
                                dpOrderDetail.Detail_IsPackage_Parent = true;
                                model.TotalMoney = (Convert.ToDecimal(model.TotalMoney) + Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price)).ToString("0.00");
                                model.TotalCount = (Convert.ToInt32(model.TotalCount) + Convert.ToInt32(modelDetail.CpCount)).ToString();
                            }
                            else
                            {//套餐子级
                                dpOrderDetail.Cp_UnitPrice = 0.00m;
                                dpOrderDetail.Cp_SumPrice = 0.00m;
                                dpOrderDetail.Cp_Actual_UnitPrice = 0.00m;
                                dpOrderDetail.Cp_Actual_SumPrice = 0.00m;
                                dpOrderDetail.Cp_Hide_UnitPrice = currentCp.DP_Price.Value;
                                dpOrderDetail.Cp_Hide_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                                if (string.IsNullOrEmpty(nowPackage.packageName))
                                {
                                    dpOrderDetail.Package_Name = DCBOperations.PackageName(modelDetail.PackageCode);
                                }
                                dpOrderDetail.Detail_IsPackage_Parent = false;
                            }
                            dpOrderDetail.Detail_IsPackage = true;
                            dpOrderDetail.Package_ID = nowPackage.packageId;
                            dpOrderDetail.Package_Name = nowPackage.packageName;
                            cpDetails.Add(dpOrderDetail);
                        }
                        else//如果不是套餐
                        {
                            dpOrderDetail.Cp_UnitPrice = currentCp.DP_Price.Value;
                            dpOrderDetail.Cp_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                            dpOrderDetail.Cp_Actual_UnitPrice = currentCp.DP_Price.Value;
                            dpOrderDetail.Cp_Actual_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                            dpOrderDetail.Cp_Hide_UnitPrice = currentCp.DP_Price.Value;
                            dpOrderDetail.Cp_Hide_SumPrice = Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price);
                            dpOrderDetail.Package_ID = "";
                            dpOrderDetail.Detail_IsPackage = false;
                            dpOrderDetail.Detail_IsPackage_Parent = false;
                            dpOrderDetail.Package_Name = "";
                            cpDetails.Add(dpOrderDetail);

                            model.TotalMoney = (Convert.ToDecimal(model.TotalMoney) + Convert.ToDecimal(modelDetail.CpCount) * Convert.ToDecimal(currentCp.DP_Price)).ToString("0.00");
                            model.TotalCount = (Convert.ToInt32(model.TotalCount) + 1).ToString();
                        }
                    }
                    else
                    {
                        result = modelDetail.CpNumber + "菜品信息不存在";
                        break;
                    }
                }

                totalMoeny = Convert.ToDecimal(model.TotalMoney);
                cpOrderInfo.Doc_ID = docId;
                cpOrderInfo.Doc_SeatNumber = model.SeatId;
                cpOrderInfo.Doc_People = string.IsNullOrEmpty(model.Renshu) ? 2 : Convert.ToInt32(model.Renshu);
                cpOrderInfo.Doc_StartTime = DateTime.Now;
                cpOrderInfo.Doc_Status = 1;
                cpOrderInfo.Doc_Remarks = model.Remarks;
                cpOrderInfo.Doc_DpCount = Convert.ToInt32(model.TotalCount);
                cpOrderInfo.Doc_DpSum = totalMoeny;
                cpOrderInfo.Doc_OpenTime = DateTime.Now;

                if (result != "")
                {
                    //直接返回错误结果
                    return result;
                }
                else
                {
                    var operateResult = "";
                    if (AddType == 1)
                    {
                        operateResult=DCBOperations.AddOrder(cpDetails, cpOrderInfo);
                    }
                    else
                    {
                        operateResult=DCBOperations.AddFood(cpDetails, cpOrderInfo);
                    }
                    return operateResult;
                }
            }
        }

        public static void DYXFQD(string orderId)
        {
            //var printUrl = "/../Printer/Printer/xiaofei.asp?Doc_ID_HC="+orderId+"&Detail_Is_Printed_HC=-1&DY=2";
            var printUrl = "/../Printer/Printer/xiaofei.asp?Doc_ID_HC="+orderId+"&Detail_Is_Printed_HC=-1&DY=1";
            Get(printUrl);
        }
        public static void DYOrderInfo(string orderId)
        {
            var printUrl = "/110302/HanZiDiner/Print/Eat_Printing_All.asp?docId="+ orderId;
            Get(printUrl);
        }
        #region Help

        /// <summary>
        /// 处理传递的备注信息
        /// </summary>
        /// <param name="remarksCode">备注编码 有可能用户自行输入</param>
        /// <param name="remarkInfos">备注信息</param>
        /// <returns></returns>
        public static string HandleRemarks(string remarksCode, List<KHYQ> remarkInfos)
        {
            if (remarksCode.Length <= 2) {
                return remarksCode;
            }
            else
            {
                try
                {
                    var length = remarksCode.Length;
                    var index = 0;
                    var currentCode = "";
                    var result = "";
                    while (length > 0)
                    {
                        currentCode = remarksCode.Substring(index, 3);
                        var currentRemarks = remarkInfos.FirstOrDefault(p => p.bh == currentCode);
                        if (currentRemarks != null)
                        {
                            result += currentRemarks.mc.Trim() + "|";
                        }

                        length -= 3;
                        index += 3;
                    }
                    
                    return  string.IsNullOrEmpty(result) ? remarksCode : result.Substring(0, result.Length - 1);
                }
                catch {
                    return remarksCode;
                }

            }
        }


        public static string EncodeToGB2312(string content)
        {
            var result= System.Web.HttpUtility.UrlEncode(content, Encoding.GetEncoding("GB2312"));
            return result;
        }



        #endregion
    }
}
