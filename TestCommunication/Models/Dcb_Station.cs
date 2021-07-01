using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DCBProject.Models
{
    [Table("Dcb_Station")]
    public class Dcb_Station
    {
        [ExplicitKey]
        public string Station_Id { get; set; }

        public string Station_No { get; set; }

        public string Station_Port { get; set; }

        public string Station_Ip { get; set; }
    }



    public class StationList
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
