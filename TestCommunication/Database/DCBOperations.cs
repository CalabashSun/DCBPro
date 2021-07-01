using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Dapper;
using DCBProject.Models;
using DCBProject.Util;

namespace DCBProject.Database
{
    public static class DCBOperations
    {

        #region Eat_Desk
        /// <summary>
        /// 桌台信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Eat_Desk DeskInfo(string code)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"select * from Eat_Desk where EDCode='{code}' and IsDel=0";
                var stations = conn.QueryFirstOrDefault<Eat_Desk>(sqlString);
                return stations;
            }
        }

        /// <summary>
        /// 开台
        /// </summary>
        /// <param name="cqId"></param>
        public static void UpdateDeskState(string cqId,string renShu)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"update Eat_Desk set EDState=2,EDCustomer='{renShu}' where CQIDS='{cqId}'";
                conn.Execute(sqlString);
            }
        }
        #endregion

        #region Order

        public static List<KHYQ> CpRemarks()
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"select * from View_DCB_KHYQ";
                return conn.Query<KHYQ>(sqlString).ToList();
            }
        }

        public static List<Eat_XFBM_DP> CpInfos(string cpNumbers)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var numberArrary = cpNumbers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var inData = ToolUtil.ConvertToStr(numberArrary);
                var sqlString = $"select * from Eat_XFBM_DP where BL_Code in({inData})";
                return conn.Query<Eat_XFBM_DP>(sqlString).ToList();
            }
        }


        public static Eat_OrderDoc OrderInfo(string orderId)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"select * from Eat_OrderDoc where Doc_ID='{orderId}'";
                return conn.QueryFirst<Eat_OrderDoc>(sqlString);
            }
        }


        public static string orderId(string deskId)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"select EDOrderNo from Eat_Desk where EDCode='{deskId}'";
                return conn.QueryFirstOrDefault<string>(sqlString);
            }
        }


        public static string PackageName(string tcId)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"select DP_Name from Eat_XFBM_DP where Dp_TcCode='{tcId}'";
                return conn.QueryFirstOrDefault<string>(sqlString);
            }
        }

        public static string AddOrder(List<Eat_OrderDoc_Detail> models,Eat_OrderDoc orderDoc) 
        {
            var strSql = new StringBuilder();
            strSql.Append("insert into Eat_OrderDoc_Detail (Cp_Printer,Detail_ID,Doc_ID,Doc_SeatNumber,Cp_Code,Cp_Name,Cp_Quantity,Actual_Quantity,Cp_UnitPrice");
            strSql.Append(",Cp_SumPrice,Cp_Actual_UnitPrice,Cp_Actual_SumPrice,Detail_AddTime,Detail_Remarks,Detail_IsDel,Doc_Remarks");
            strSql.Append(",Cp_Hide_UnitPrice,Cp_Hide_SumPrice,Package_ID,Detail_IsPackage,Detail_IsPackage_Parent,Package_Name,Cp_IsSingleCut");
            strSql.Append(",InitialCount,InitialSum,OrderResource,Detail_Is_Printed) values ");

            foreach (var item in models)
            {
                strSql.Append($"('{item.Cp_Printer}','{item.Detail_ID}','{item.Doc_ID}','{item.Doc_SeatNumber}','{item.Cp_Code}','{item.Cp_Name}','{item.Cp_Quantity}','{item.Actual_Quantity}','{item.Cp_UnitPrice}'");
                strSql.Append($",'{item.Cp_SumPrice}','{item.Cp_Actual_UnitPrice}','{item.Cp_Actual_SumPrice}','{item.Detail_AddTime}','{item.Detail_Remarks}','0','{item.Doc_Remarks}'");
                strSql.Append($",'{item.Cp_Hide_UnitPrice}','{item.Cp_Hide_SumPrice}','{item.Package_ID}','{item.Detail_IsPackage}','{item.Detail_IsPackage_Parent}','{item.Package_Name}','{item.Cp_IsSingleCut}'");
                strSql.Append($",'{item.Cp_Quantity}','{item.Cp_SumPrice}','4','0'),");
            }
            var inserSql = strSql.ToString();
            var sql = inserSql.Substring(0, inserSql.LastIndexOf(','));
            var isSuccess = 0;
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                //判断桌台状态是否变更
                var orderDeskString = $"select * from Eat_Desk where CQIDS='{orderDoc.Doc_SeatNumber}' and IsDel=0";
                var orderDeskInfo = conn.QueryFirstOrDefault<Eat_Desk>(orderDeskString);
                if (orderDeskInfo.EDState == 1 || orderDeskInfo.EDState == null || (orderDeskInfo.EDState == 2 && !string.IsNullOrEmpty(orderDeskInfo.EDOrderNo)))
                {
                    int result = conn.Execute(sql, null);
                    var orderSql = "insert into Eat_OrderDoc (Doc_ID,Doc_SeatNumber,Doc_People,Doc_StartTime,Doc_Status,Doc_IsDel,Doc_Remarks,Doc_DpCount,Doc_DpSum,Doc_DpActualSum,Doc_OpenTime,OrderResource,LastOrderResource) values ";
                    orderSql += $"('{orderDoc.Doc_ID}','{orderDoc.Doc_SeatNumber}','{orderDoc.Doc_People}','{orderDoc.Doc_StartTime}','{orderDoc.Doc_Status}','0','{orderDoc.Doc_Remarks}','{orderDoc.Doc_DpCount}','{orderDoc.Doc_DpSum}','{orderDoc.Doc_DpSum}','{DateTime.Now}','4','4')";
                    result = conn.Execute(orderSql, null);

                    var deskSql = $"update Eat_Desk set EDOrderNo='{orderDoc.Doc_ID}',EDOrderMoney='{orderDoc.Doc_DpSum}',EDState='3',EDCustomer='{orderDoc.Doc_People}',EDOpenTime='{DateTime.Now}' where CQIDS='{orderDoc.Doc_SeatNumber}'";
                    result = conn.Execute(deskSql, null);

                    var updatePrice = $"exec updateSumPrice '{orderDoc.Doc_ID}'";
                    conn.Execute(updatePrice, null);
                    isSuccess = 1;
                }

            }
            return isSuccess==1?"success":"deskError";
        }


        public static string AddFood(List<Eat_OrderDoc_Detail> models, Eat_OrderDoc orderDoc)
        {
            var strSql = new StringBuilder();
            strSql.Append("insert into Eat_OrderDoc_Detail (Cp_Printer,Detail_ID,Doc_ID,Doc_SeatNumber,Cp_Code,Cp_Name,Cp_Quantity,Actual_Quantity,Cp_UnitPrice");
            strSql.Append(",Cp_SumPrice,Cp_Actual_UnitPrice,Cp_Actual_SumPrice,Detail_AddTime,Detail_Remarks,Detail_IsDel,Doc_Remarks");
            strSql.Append(",Cp_Hide_UnitPrice,Cp_Hide_SumPrice,Package_ID,Detail_IsPackage,Detail_IsPackage_Parent,Package_Name,Cp_IsSingleCut");
            strSql.Append(",InitialCount,InitialSum,OrderResource,Detail_Is_Printed) values ");

            foreach (var item in models)
            {
                strSql.Append($"('{item.Cp_Printer}','{item.Detail_ID}','{item.Doc_ID}','{item.Doc_SeatNumber}','{item.Cp_Code}','{item.Cp_Name}','{item.Cp_Quantity}','{item.Actual_Quantity}','{item.Cp_UnitPrice}'");
                strSql.Append($",'{item.Cp_SumPrice}','{item.Cp_Actual_UnitPrice}','{item.Cp_Actual_SumPrice}','{item.Detail_AddTime}','{item.Detail_Remarks}','0','{item.Doc_Remarks}'");
                strSql.Append($",'{item.Cp_Hide_UnitPrice}','{item.Cp_Hide_SumPrice}','{item.Package_ID}','{item.Detail_IsPackage}','{item.Detail_IsPackage_Parent}','{item.Package_Name}','{item.Cp_IsSingleCut}'");
                strSql.Append($",'{item.Cp_Quantity}','{item.Cp_SumPrice}','4','0'),");
            }
            var inserSql = strSql.ToString();
            var sql = inserSql.Substring(0, inserSql.LastIndexOf(','));
            var isSuccess = 0;
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                //判断桌台状态是否变更
                var orderDeskString = $"select * from Eat_Desk where CQIDS='{orderDoc.Doc_SeatNumber}' and IsDel=0";
                var orderDeskInfo = conn.QueryFirstOrDefault<Eat_Desk>(orderDeskString);
                if (orderDeskInfo.EDState == 1 || orderDeskInfo.EDState == null || (orderDeskInfo.EDState == 2 && !string.IsNullOrEmpty(orderDeskInfo.EDOrderNo)))
                {
                    isSuccess = 0;
                }
                else
                {
                    int result = conn.Execute(sql, null);
                    var orderSql = $"update Eat_OrderDoc set Doc_Remarks='{orderDoc.Doc_Remarks}',Doc_DpCount='{orderDoc.Doc_DpCount}',Doc_DpSum='{orderDoc.Doc_DpSum}',Doc_DpActualSum='{orderDoc.Doc_DpSum}',LastOrderResource='4' where Doc_Id='{orderDoc.Doc_ID}'";
                    result = conn.Execute(orderSql, null);

                    var deskSql = $"update Eat_Desk set EDOrderNo='{orderDoc.Doc_ID}',EDOrderMoney='{orderDoc.Doc_DpSum}',EDCustomer='{orderDoc.Doc_People}' where CQIDS='{orderDoc.Doc_SeatNumber}'";
                    result = conn.Execute(deskSql, null);

                    var updatePrice = $"exec updateSumPrice '{orderDoc.Doc_ID}'";
                    conn.Execute(updatePrice, null);
                    isSuccess = 1;
                }

            }
            return isSuccess == 1 ? "success" : "deskError";
        }
        #endregion

        #region Record

        public static List<Eat_OrderDoc_Detail> GetOrderDetails(string orderId)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"select * from Eat_OrderDoc_Detail where Doc_ID='{orderId}'";
                return conn.Query<Eat_OrderDoc_Detail>(sqlString).ToList();
            }
        }


        public static List<ShowOrderDetail> GetOrderDetailInfo(string orderId)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = new StringBuilder();
                sqlString.Append("select dbo.f_cut(Cp_Name+'            ',10) as cpName");
                sqlString.Append($" ,REVERSE(dbo.f_cut(REVERSE('    '+CONVERT(varchar,Cp_Quantity)),6)) as cpCount");
                sqlString.Append($" from dbo.Eat_OrderDoc_Detail where Doc_ID='{orderId}'");
                return conn.Query<ShowOrderDetail>(sqlString.ToString()).ToList();
            }
        }

        #endregion
    }
}
