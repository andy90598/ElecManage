using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElecManage.Models
{
    public class BroadcastModel
    {
        public List<int> Data { get; set; }
        public string Label { get; set; }
        public BroadcastModel()
        {
            Data = new List<int>();
        }
    }
}
