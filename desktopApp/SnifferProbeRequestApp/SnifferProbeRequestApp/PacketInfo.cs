using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferProbeRequestApp
{
    public class PacketsInfo
    {
        public List<PacketInfo> listPacketInfo { get; set; }
    }

    public class PacketInfo
    {
        public string sourceAddress { get; set; }
        public string SSID { get; set; }
        public Int32 signalStrength { get; set; }
        public string hashCode { get; set; }
        public long timestamp { get; set; }
    }
}
