using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferProbeRequestApp.valueClass
{
    class ConnectionPeriod {

        public ConnectionPeriod(string sourceAddress, DateTime startTimestamp, DateTime stopTimestamp) {
            this.sourceAddress = sourceAddress;
            this.startTimestamp = startTimestamp;
            this.stopTimestamp = stopTimestamp;
        }

        public string sourceAddress { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime stopTimestamp { get; set; }
    }
}
