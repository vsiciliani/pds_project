using System.Threading;

namespace SnifferProbeRequestApp {
    public class Device {
        public Device(string ipAddress, int codDevice, int x_position, int y_position, int openSocket, ManualResetEvent evento) {
            this.ipAddress = ipAddress;
            this.codDevice = codDevice;
            this.x_position = x_position;
            this.y_position = y_position;
            this.openSocket = openSocket;
            this.evento = evento;
        }

        public string ipAddress { get; set; }
        public int codDevice { get; set; }
        public int x_position { get; set; }
        public int y_position { get; set; }
        public int openSocket { get; set; }
        public ManualResetEvent evento { get; set; }
    }
}
