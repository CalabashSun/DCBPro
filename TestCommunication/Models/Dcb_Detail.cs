using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DCBProject.Models
{
    [Table("Dcb_Detail")]
    public class Dcb_Detail
    {
        [ExplicitKey]
        public string Detail_Id { get; set; }

        public string Detail_No { get; set; }

        public string Station_Id { get; set; }

        public string Station_No { get; set; }
    }
}
