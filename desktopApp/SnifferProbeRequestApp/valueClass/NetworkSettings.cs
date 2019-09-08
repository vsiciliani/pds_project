namespace SnifferProbeRequestApp.valueClass {

    public class NetworkSettings {
        public NetworkSettings(int servicePort) {
        this.servicePort = servicePort;
    }

    public int servicePort { get; set; }
    }
    
}
