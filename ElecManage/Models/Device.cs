using System;
using System.Collections.Generic;

#nullable disable

namespace ElecManage.Models
{
    public partial class Device
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public long DeviceVolt { get; set; }
        public double? DeviceKWh { get; set; }
    }
}
