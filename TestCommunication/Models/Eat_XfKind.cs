using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCBProject.Models
{
    public class Eat_XfKind
    {
        public string Eat_XfKind_ID { get; set; }

        public string code { get; set; }

        public string Eat_XfKind_Cate_Name { get; set; }

        public string Eat_XfKind_Parent_ID { get; set; }

        public bool? Eat_XfKind_IsEnable { get; set; }

        public bool? Eat_XfKind_IsDelete { get; set; }

        public string Eat_XfKind_Creater { get; set; }

        public DateTime? Eat_XfKind_CreateTime { get; set; }

        public DateTime Eat_XfKind_UpdateTime { get; set; }

        public int? Eat_XfKind_Sort { get; set; }

    }

}
