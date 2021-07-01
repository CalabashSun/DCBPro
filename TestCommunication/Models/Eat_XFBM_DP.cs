using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCBProject.Models
{
    public class Eat_XFBM_DP
    {
        public string DP_ID { get; set; }

        public string Number_Code { get; set; }

        public string Dp_TcCode { get; set; }

        public string DP_Name { get; set; }

        public string DP_Abbreviation { get; set; }

        public string DP_PY { get; set; }

        public string DP_Unit { get; set; }

        public decimal? DP_Price { get; set; }

        public decimal? DP_MemberPrice { get; set; }

        public string DP_KindIdP { get; set; }

        public string DP_KindIdC { get; set; }

        public string DP_KindP_Name { get; set; }

        public string DP_KindC_Name { get; set; }

        public bool? IsDiscount { get; set; }

        public bool? IsTimePricedDish { get; set; }

        public bool? IsModification { get; set; }

        public bool? IsLimited { get; set; }

        public bool? IsServiceFee { get; set; }

        public bool? IsConsignment { get; set; }

        public bool? IsPackage { get; set; }

        public bool? IsTiming { get; set; }

        public string DP_Creater { get; set; }

        public bool? DP_IsDelete { get; set; }

        public DateTime? DP_CreateTime { get; set; }

        public DateTime? DP_UpdateTime { get; set; }

        public string DP_PrinterId { get; set; }

        public string DP_FoodReq { get; set; }

        public bool? DP_IsBan { get; set; }

        public bool? Dp_IsSingleCut { get; set; }

        public decimal? PrintOrder { get; set; }

        public string TVId { get; set; }

        public string BL_Code { get; set; }

    }
}
