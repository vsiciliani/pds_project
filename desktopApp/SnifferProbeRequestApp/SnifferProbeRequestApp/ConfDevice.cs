using System;
using System.Collections.Concurrent;

namespace SnifferProbeRequestApp {
    /// <summary>
    /// Classe statica che wrappa il ConcurrentDictionary che gestisce i rilevatori connessi e configurati
    /// </summary>
    public static class ConfDevice {
        //LISTA DEVICE CONFIGUARATI
        // key = IPAddress device
        // value = informazioni su device
        public static ConcurrentDictionary<string, Device> lstConfDevices = new ConcurrentDictionary<string, Device>();

        //delegato per lanciare gli eventi dopo la modifica della lstConfDevices
        public static void OnLstConfDevicesChanged(object sender, EventArgs e) {
            EventHandler handler = LstConfDevicesChanged;
            if (handler != null) {
                handler(sender, e);
            }
        }

        //event a cui iscriversi per rilevare la modifica sulla lstConfDevices
        public static event EventHandler LstConfDevicesChanged;

        public static int getMaxXPositionDevice() {
            int pos = int.MinValue;

            foreach (var device in lstConfDevices) {
                if (device.Value.x_position > pos) pos = device.Value.x_position;
            }
            return pos;
        }

        public static int getMinXPositionDevice() {
            int pos = int.MaxValue;

            foreach (var device in lstConfDevices) {
                if (device.Value.x_position < pos) pos = device.Value.x_position;
            }
            return pos;
        }

        public static int getMaxYPositionDevice() {
            int pos = int.MinValue;

            foreach (var device in lstConfDevices) {
                if (device.Value.y_position > pos) pos = device.Value.y_position;
            }
            return pos;
        }

        public static int getMinYPositionDevice() {
            int pos = int.MaxValue;

            foreach (var device in lstConfDevices) {
                if (device.Value.y_position < pos) pos = device.Value.y_position;
            }
            return pos;
        }
    }
}
