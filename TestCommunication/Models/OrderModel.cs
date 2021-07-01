using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCBProject.Models
{
    public class OrderModel
    {
        /// <summary>
        /// 台号
        /// </summary>
        public string Taihao { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public string Renshu { get; set; }
        /// <summary>
        /// 整单备注
        /// </summary>
        public string Remarks { get; set; }

        public string TotalCount { get; set; }

        public string TotalMoney { get; set; }

        public string SeatId { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 订单详情
        /// </summary>
        public List<OrderDetail> Details=new List<OrderDetail>();
    }

    public class OrderDetail
    {
        /// <summary>
        /// 菜品编号
        /// </summary>
        public string CpNumber { get; set; }
        /// <summary>
        /// 菜品数量
        /// </summary>
        public string CpCount { get; set; }
        /// <summary>
        /// 菜品备注
        /// </summary>
        public string CpRemarks { get; set; }

        public string PackageCode { get; set; }
    }
}
