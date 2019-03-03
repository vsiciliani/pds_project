using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferProbeRequestApp
{
    public class Device
    {
        public string ipAddress { get; set; }
        public Int32 codDevice { get; set; }
        public Int32 x_position { get; set; }
        public Int32 y_position { get; set; }

        public Device(string ipAddress, Int32 codDevice, Int32 x_position, Int32 y_position)
        {
            this.ipAddress = ipAddress;
            this.codDevice = codDevice;
            this.x_position = x_position;
            this.y_position = y_position;
        }

        
    }
}
