using System;
using System.Collections.Generic;

#nullable disable

namespace ElecManage.Models
{
    public partial class Electricity
    {
        public long ElecSn { get; set; }
        public string DeviceId { get; set; }
        public DateTime Time { get; set; }
        public double? Value { get; set; }

        internal object Sum(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}
