namespace SnifferProbeRequestApp {
    class AssembledPacketInfo {
        public AssembledPacketInfo(string sourceAddress, string SSID, string hashCode, long timestamp, double x_position, double y_position) {
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
        public long timestamp { get; set; }
        public double x_position { get; set; }
        public double y_position { get; set; }
    }
}
