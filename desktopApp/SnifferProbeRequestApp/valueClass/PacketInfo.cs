using System.Collections.Generic;

namespace SnifferProbeRequestApp {
    ///<summary>Classe che contiene la lista dei pacchetti catturati.
    ///Necessaria per la deserializzazione del messaggio ricevuto.</summary>
    public class PacketsInfo {
        public List<PacketInfo> listPacketInfo { get; set; }
    }

    ///<summary>Classe che modella i dati dei pacchetti catturati</summary>
    public class PacketInfo {
        public string sourceAddress { get; set; }
        public string SSID { get; set; }
        public int signalStrength { get; set; }
        public string hashCode { get; set; }
        public long timestamp { get; set; }
    }
}
