using System;

namespace SnifferProbeRequestApp.valueClass {
    class DevicePosition {
        public DevicePosition(string sourceAddress, double xPosition, double yPosition, DateTime time) {
            this.sourceAddress = sourceAddress;
            this.time = time;
            this.xPosition = xPosition;
            this.yPosition = yPosition;
        }

        public string sourceAddress { get; set; }
        public double xPosition { get; set; }
        public double yPosition { get; set; }
        public DateTime time { get; set; }
    }
}
