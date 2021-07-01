using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCBProject.Models
{
    public class Eat_OrderDoc_Detail
    {
        public string Detail_ID { get; set; }

        public string Doc_ID { get; set; }

        public string Doc_SeatNumber { get; set; }

        public string Cp_Code { get; set; }

        public string Cp_Name { get; set; }

        public decimal Cp_Quantity { get; set; }
        public decimal Actual_Quantity { get; set; }
        public decimal Cp_UnitPrice { get; set; }

        public decimal Cp_SumPrice { get; set; }
        public decimal Cp_Actual_UnitPrice { get; set; }
        public decimal Cp_Actual_SumPrice { get; set; }
        public decimal Cp_Hide_UnitPrice { get; set; }
        public decimal Cp_Hide_SumPrice { get; set; }
        public DateTime Detail_AddTime { get; set; }
        public bool Detail_IsPackage { get; set; }
        public bool Detail_IsPackage_Parent { get; set; }
        public string Package_ID { get; set; }
        public string Package_Name { get; set; }
        public string Detail_Remarks { get; set; }
        public string Doc_Remarks { get; set; }
        public string Cp_Printer { get; set; }

        public bool Detail_IsDel { get; set; }
        public int Detail_Discount_Type { get; set; }
        public string Detail_Discount_CategoryId { get; set; }
        public string Detail_Discount_Reason { get; set; }
        public bool Detail_Is_Printed { get; set; }


        public decimal ReturnedCount { get; set; }
        public int ReturnStatus { get; set; }
        public string ReturnDetailId { get; set; }
        public bool Cp_IsSingleCut { get; set; }

    }

    public class ShowOrderDetail
    {
        public string cpName { get; set; }

        public string cpCount { get; set; }
    }
}
