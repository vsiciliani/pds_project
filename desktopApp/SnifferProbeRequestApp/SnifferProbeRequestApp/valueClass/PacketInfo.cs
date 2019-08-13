using System.Collections.Generic;

namespace SnifferProbeRequestApp {
    public class PacketsInfo {
        public List<PacketInfo> listPacketInfo { get; set; }
    }

    public class PacketInfo {
        public string sourceAddress { get; set; }
        public string SSID { get; set; }
        public int signalStrength { get; set; }
        public string hashCode { get; set; }
        public long timestamp { get; set; }
    }
}
