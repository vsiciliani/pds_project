using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferProbeRequestApp.valueClass {
    class CountDevice {
        public CountDevice(DateTime timestamp, int count) {
            this.timestamp = timestamp;
            this.count = count;
        }

        public DateTime timestamp { get; set; }
        public int count { get; set; }
    }
}
