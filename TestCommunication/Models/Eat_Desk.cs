using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCBProject.Models
{
    public class Eat_Desk
    {
        public string CQIDS { get; set; }
        /// <summary>
        /// code
        /// </summary>
        public string EDCode { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        public int? EDCustomer { get; set; }
        /// <summary>
        /// 桌台名称
        /// </summary>
        public string EDName { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public string EDIsEnable { get; set; }
        /// <summary>
        /// 桌台状态
        /// </summary>
        public int? EDState { get; set; }
        /// <summary>
        /// 开台时间
        /// </summary>
        public string EDOpenTime { get; set; }

        public string EDOrderNo { get; set; }
    }
}
