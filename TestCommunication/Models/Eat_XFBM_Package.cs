using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCBProject.Models
{
    public class Eat_XFBM_Package
    {
        public string Pac_ID { get; set; }

        public string Pac_DPPac_ID { get; set; }

        public string Pac_DPPac_Code { get; set; }

        public string Pac_Cate_ID { get; set; }

        public string Child_DP_ID { get; set; }

        public string Child_DP_Number { get; set; }

        public string Child_DP_Name { get; set; }

        public decimal Child_DP_Count { get; set; }

        public string Child_DP_Unit { get; set; }

        public decimal Child_DP_Price { get; set; }

        public bool Child_Dp_Cut { get; set; }

        public bool Child_IsChoosed { get; set; }

        public DateTime Pac_CreateTime { get; set; }

        public DateTime Pac_UpdateTime { get; set; }

        public bool Pac_Enable { get; set; }

        public bool Pac_IsDel { get; set; }

        public decimal? AllocationPrice { get; set; }

    }

}
