using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElecManage.Models
{
    public class ElecData
    {
        public string uuid { get; set; }
        public double value { get; set; }
        public double time { get; set; }
        public string item { get; set; }
        public string formatedTime { get; set;}
    }
}
