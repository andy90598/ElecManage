using System;
using System.Collections.Generic;

#nullable disable

namespace ElecManage.Models
{
    public partial class Electricity
    {
        public long ElecSn { get; set; }
        public string DeviceId { get; set; }
        public string Time { get; set; }
        public double? Value { get; set; }
    }
}
