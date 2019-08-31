namespace SnifferProbeRequestApp.valueClass {

    public class NetworkSettings {
        public NetworkSettings(bool generateNetwork, string SSID, string key, int servicePort) {
        this.generateNetwork = generateNetwork;
        this.SSID = SSID;
        this.key = key;
        this.servicePort = servicePort;
    }

    public bool generateNetwork { get; set; }
    public string SSID { get; set; }
    public string key { get; set; }
    public int servicePort { get; set; }
}
    
}
