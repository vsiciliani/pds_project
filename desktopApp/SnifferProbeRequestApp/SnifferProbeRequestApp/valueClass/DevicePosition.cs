using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferProbeRequestApp.valueClass
{
    class DevicePosition {
        public DevicePosition(string sourceAddress, double xPosition, double yPosition) {
            this.sourceAddress = sourceAddress;
            this.xPosition = xPosition;
            this.yPosition = yPosition;
        }

        public string sourceAddress { get; set; }
        public double xPosition { get; set; }
        public double yPosition { get; set; }

    }
}
