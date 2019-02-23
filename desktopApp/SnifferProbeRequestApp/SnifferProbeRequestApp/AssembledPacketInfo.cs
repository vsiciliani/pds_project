using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferProbeRequestApp
{
    class AssembledPacketInfo
    {
        public AssembledPacketInfo(string sourceAddress, string SSID, string hashCode, Int64 timestamp, int x_position, int y_position)
        {
            this.sourceAddress = sourceAddress;
            this.SSID = SSID;
            this.hashCode = hashCode;
            this.timestamp = timestamp;
            this.x_position = x_position;
            this.y_position = y_position;
        }

        public string sourceAddress { get; set; }
        public string SSID { get; set; }
        public string hashCode { get; set; }
        public Int64 timestamp { get; set; }
        public Int32 x_position { get; set; }
        public Int32 y_position { get; set; }
    }
}
