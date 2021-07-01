using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using DCBProject.Database;
using DCBProject.Models;
using TestCommunication.Util;

namespace DCBProject.Util
{
    //针对WX.exe发送过来的请求类型的响应函数
    //所有返回的文本一定要严格按照厂商接口文档来，尤其中间补齐的空格数
    public class DCBFunctionUtil
    {
        /// <summary>
        /// 开机的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void KJFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 登录的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void DLFunction(int stationNo, string dcbNo, List<String> requestContent)
        {
            var responseText = string.Empty;
            responseText = requestContent[0] + "\r\n";
            var userName = requestContent[1].Substring(11, 4).Trim();

            var userPwd = requestContent[1].Substring(16, 8).Trim();
            LogsHelper.WriteLog(userName);
            LogsHelper.WriteLog(userPwd);
            if (userName == "0308" && userPwd == "1")
            {
                responseText+= "1 " + "管理员";
            } 
            else
            {
                responseText += "0 " + "信息错误";
            }
            string fileName = "R" + stationNo + ".Txt";
            FileUtil.WriteContent(Application.StartupPath + @"\TXT\" + fileName, responseText);
        }
        /// <summary>
        /// 修改密码的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void XGMMFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 开台(KT)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void KTFunction(int stationNo, string dcbNo, List<String> requestContent)
        {
            var responseText = string.Empty;
            responseText = requestContent[0]+"\r\n";
            //执行开台操作
            //截取4位台号
            var info = requestContent[1];
            var taiHao = info.Substring(0, 4).Trim();
            var renShu = info.Substring(4, 2).Trim();
            string fileName = "R" + stationNo + ".Txt";
            var deskInfo = DCBOperations.DeskInfo(taiHao);
            if (deskInfo == null)
            {
                responseText += "编号为"+taiHao+"的桌台在系统中不存在";
            }
            else
            {
                if (deskInfo.EDState != null&&deskInfo.EDState != 1)
                {
                    responseText += "开台失败！"+taiHao+"台号已经开台！";
                }
                else
                {
                    DCBOperations.UpdateDeskState(deskInfo.CQIDS,renShu);
                    responseText += taiHao+"台开台成功!" + "\r\n"; ;
                    responseText += "人数为：" + renShu;
                }
            }

            
            FileUtil.WriteContent(Application.StartupPath + @"\TXT\" + fileName, responseText);
        }
        /// <summary>
        /// 换台(HT)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void HTFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 并台(BT)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void BTFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 撤台(CT)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void CTFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 修改台头(XGTT)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void XGTTFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 预订抵达(YDDD)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void YDDDFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 点菜(DC)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void DCFunction(int stationNo, string dcbNo, List<String> requestContent)
        {
            var responseText = string.Empty;
            responseText = requestContent[0] + "\r\n";
            //取桌台信息以及整单备注
            var orderInfo = requestContent[1];
            var requestData = new OrderModel();
            requestData.Taihao = orderInfo.Substring(0, 7).Trim();
            requestData.Remarks = orderInfo.Substring(19, 12).Trim();

            //记录点菜的机号加流水
            var serialNumber = dcbNo + DateTime.Now.ToString("yyyyMMdd") + orderInfo.Substring(37, 8).Trim();
            //判断序列号是否重复
            if (AddOrderData.AddOrderRecord.Count(p => p == serialNumber)>0)
            {
                responseText += "多基站重复下单！" + "\r\n";
                responseText += "请联系收银是否下单成功！" + "\r\n";
                string fileName = "R" + stationNo + ".Txt";
                FileUtil.WriteContent(Application.StartupPath + @"\TXT\" + fileName, responseText);
                return;
            }
            int lineCount = 0;
            int cpCount = 0;
            decimal currentTotalMoeny = 0;
            foreach (var cpContent in requestContent)
            {
                lineCount++;
                if(lineCount<3)continue;
                var detailInfo=new OrderDetail();
                //detailInfo.CpNumber = cpContent.Substring(8, 5).Trim();
                detailInfo.CpNumber = ToolUtil.SubstringByte(cpContent, 8, 5).Trim();
                //detailInfo.CpCount = cpContent.Substring(14, 4).Trim();
                detailInfo.CpCount = ToolUtil.SubstringByte(cpContent,14, 4).Trim();
                //detailInfo.PackageCode = cpContent.Substring(32, 2).Trim();
                detailInfo.PackageCode = ToolUtil.SubstringByte(cpContent,32, 2).Trim();
                cpCount += 1;
                //detailInfo.CpRemarks = cpContent.Substring(19, 12).Trim();
                detailInfo.CpRemarks = ToolUtil.SubstringByte(cpContent, 19, 12).Trim();
                //requestData.Renshu = cpContent.Substring(51, 2).Trim();
                requestData.Renshu = ToolUtil.SubstringByte(cpContent,51,2).Trim();
                requestData.Details.Add(detailInfo);
            }


            //获取当前餐桌的状态

            var currentDesk = DCBOperations.DeskInfo(requestData.Taihao);
            if (currentDesk == null)//如果餐桌不存在
            {
                responseText += "不存在该桌台！" + "\r\n";
                string fileName = "R" + stationNo + ".Txt";
                FileUtil.WriteContent(Application.StartupPath + @"\TXT\" + fileName, responseText);
            }
            else
            {
                var result = "";
                requestData.SeatId = currentDesk.CQIDS;
                if (currentDesk.EDState == 1 || currentDesk.EDState == null || (currentDesk.EDState == 2 && !string.IsNullOrEmpty(currentDesk.EDOrderNo)))
                {
                    requestData.OrderId = ToolUtil.getRandom();
                    //result = HttpUtil.PostAddOrder("/110302/HanZiDiner/DCB/AddOrder.asp", requestData, out currentTotalMoeny);
                    result = HttpUtil.PostAddOrderOrFood(1, requestData, out currentTotalMoeny);
                }
                else
                {
                    //加菜
                    //获取当前订单数据
                    var orderDetailInfo = DCBOperations.OrderInfo(currentDesk.EDOrderNo);
                    requestData.OrderId = orderDetailInfo.Doc_ID;
                    requestData.TotalCount = orderDetailInfo.Doc_DpCount.ToString();
                    requestData.TotalMoney = orderDetailInfo.Doc_DpSum.ToString("0.00");
                    result = HttpUtil.PostAddOrderOrFood(2, requestData, out currentTotalMoeny);
                }

                if (result.Contains("success"))
                {
                    AddOrderData.AddOrderRecord.Add(serialNumber);
                    responseText += "点菜成功！" + "\r\n";
                    responseText += $"{requestData.Taihao}台本次点菜:" + "\r\n";
                    responseText += $"{lineCount - 2}种/{cpCount}份" + "\r\n";
                    responseText += $"消费金额：{currentTotalMoeny}" + "\r\n";
                    //打印消费单
                    HttpUtil.DYOrderInfo(requestData.OrderId);
                }
                else {
                    responseText += "点菜失败！" + "\r\n";
                    responseText += result+ "\r\n";
                }

                string fileName = "R" + stationNo + ".Txt";
                FileUtil.WriteContent(Application.StartupPath + @"\TXT\" + fileName, responseText);
            }
        }
        /// <summary>
        /// 临时菜(LSC)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void LSCFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 重量确认(ZLQR)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void ZLQRFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 沽清列表(GQLB)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void GQLBFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 打印消费单(DXFD)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void DXFDFunction(int stationNo, string dcbNo, List<String> requestContent)
        {
            var responseText = string.Empty;
            responseText = requestContent[0] + "\r\n";
            //执行开台操作
            //截取4位台号
            var info = requestContent[1];
            var taiHao = info.Substring(0, 6).Trim();
            string fileName = "R" + stationNo + ".Txt";

            var orderId = DCBOperations.orderId(taiHao);
            if (string.IsNullOrEmpty(orderId)||orderId.Trim()=="")
            {
                responseText += taiHao + "台订单不存在无法打印！";
            }
            else
            {
                HttpUtil.DYXFQD(orderId);
                responseText += taiHao + "台消费单已打印！";
            }
            FileUtil.WriteContent(Application.StartupPath + @"\TXT\" + fileName, responseText);
        }


        /// <summary>
        /// 请求结帐(QQJZ)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void QQJZFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 退菜(TC)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void TCFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 勾挑(GT)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void GTFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 帐单查询(ZDCX)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void ZDCXFunction(int stationNo, string dcbNo, List<String> requestContent)
        {
            var responseText = string.Empty;
            responseText = requestContent[0] + "\r\n";

            var zhuoTai = requestContent[1].Substring(0, 7).Trim();
            
            responseText += $"{zhuoTai}的帐单信息:" + "\r\n";
            var currentDesk = DCBOperations.DeskInfo(zhuoTai);
            if (currentDesk == null)
            {
                responseText += "未查询到该台号\r\n";
            }
            else if (currentDesk.EDState != 3 || string.IsNullOrEmpty(currentDesk.EDOrderNo))
            {
                responseText += "该台还没有点菜或者未开台\r\n";
            }
            else
            {
                var cpDetails = DCBOperations.GetOrderDetailInfo(currentDesk.EDOrderNo);
                if (cpDetails.Count <= 0)
                {
                    responseText += "该台还没有点菜\r\n";
                }
                else
                {
                    var orderInfo = DCBOperations.OrderInfo(currentDesk.EDOrderNo);
                    responseText += $"消费合计:{orderInfo.Doc_DpSum}元" + "\r\n";
                    responseText += $"应付合计:{orderInfo.Doc_DpActualSum}元" + "\r\n";
                    responseText += $"开台人数：{currentDesk.EDCustomer}" + "\r\n";
                    foreach (var eatOrderDocDetail in cpDetails)
                    {
                        responseText += $"{eatOrderDocDetail.cpName}{eatOrderDocDetail.cpCount}份⊙" + "\r\n";
                    }
                }
            }
            string fileName = "R" + stationNo + ".Txt";
            FileUtil.WriteContent(Application.StartupPath + @"\TXT\" + fileName, responseText);

        }
        /// <summary>
        /// 整桌催菜(ZZCC)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void ZZCCFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 按菜品催菜(CPCC)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void CPCCFunction(int stationNo, string dcbNo, List<String> requestContent)
        {


        }

        /// <summary>
        /// 按菜类催菜(CLCC)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void CLCCFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 空闲餐台汇总(KXHZ)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void KXHZFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 按类别查空闲餐台(LBKX)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void LBKXFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 按台号查空闲(THKX)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void THKXFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 预订信息汇总(YDHZ)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void YDHZFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }
        /// <summary>
        /// 按台号查预订(THYD)的请求
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void THYDFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }

        /// <summary>
        /// 短信息(DX)
        /// </summary>
        /// <param name="stationNo">基站编号</param>
        /// <param name="dcbNo">点菜宝编号</param>
        /// <param name="requestContent">获得请求数据</param>
        public static void DXFunction(int stationNo, string dcbNo, List<String> requestContent)
        {

        }


    }
}
