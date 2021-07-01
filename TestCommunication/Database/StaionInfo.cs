using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using DCBProject.Models;
using DCBProject.Util;

namespace DCBProject.Database
{
    public static class StaionInfo
    {

        public static List<Dcb_Station> Stations()
        {
            using (var conn=new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = "select * from Dcb_Station";
                var stations = conn.Query<Dcb_Station>(sqlString);
                return stations.ToList();
            }
        }

        public static Dcb_Station Station(string stationId)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"select * from Dcb_Station where Station_Id='{stationId}'";
                var stations = conn.QueryFirstOrDefault<Dcb_Station>(sqlString);
                return stations;
            }
        }

        public static void AddStation(Dcb_Station model)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                conn.Insert<Dcb_Station>(model);
            }
        }

        public static void DeleteStation(string stationId)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var deleteDetails = $"delete from Dcb_Detail where Station_Id='{stationId}'";
                var deleteStation = $"delete from Dcb_Station where Station_Id='{stationId}'";
                conn.Execute(deleteDetails);
                conn.Execute(deleteStation);
            }
        }

        public static void UpdateStation(Dcb_Station model)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                conn.Update<Dcb_Station>(model);
            }
        }


        public static List<Dcb_Detail> AllDetails()
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"select * from Dcb_Detail";
                var stations = conn.Query<Dcb_Detail>(sqlString);
                return stations.ToList();
            }
        }

        public static List<Dcb_Detail> Details(string stationId)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"select * from Dcb_Detail where Station_Id='{stationId}'";
                var stations = conn.Query<Dcb_Detail>(sqlString);
                return stations.ToList();        
            }
        }
        public static Dcb_Detail Detail(string detailId)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var sqlString = $"select * from Dcb_Detail where Detail_Id='{detailId}'";
                var stations = conn.QueryFirstOrDefault<Dcb_Detail>(sqlString);
                return stations;
            }
        }
        public static void AddDetail(Dcb_Detail model)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                conn.Insert<Dcb_Detail>(model);
            }
        }

        public static void DeleteDetails(string detailId)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                var deleteDetails = $"delete from Dcb_Detail where Detail_Id='{detailId}'";
                conn.Execute(deleteDetails);
            }
        }

        public static void UpdateDetail(Dcb_Detail model)
        {
            using (var conn = new SqlConnection(DBConfig.connectionString))
            {
                conn.Update<Dcb_Detail>(model);
            }
        }
    }
}
