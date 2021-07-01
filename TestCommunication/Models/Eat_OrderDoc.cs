using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCBProject.Models
{
    public class Eat_OrderDoc
    {
        public string Doc_ID { get; set; }

        public string Doc_SeatNumber { get; set; }

        public int Doc_People { get; set; }

        public DateTime? Doc_StartTime { get; set; }

        public DateTime? Doc_EndTime { get; set; }

        public int Doc_Status { get; set; }

        public bool Doc_IsDel { get; set; }

        public string Doc_Remarks { get; set; }

        public int Doc_DpCount { get; set; }

        public decimal Doc_DpSum { get; set; }

        public decimal Doc_DpActualSum { get; set; }

        public decimal Doc_DpPayMoney { get; set; }

        public DateTime? Doc_OpenTime { get; set; }

    }
}
