using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SnifferProbeRequestApp {
    /// <summary>
    /// Classe wrappa i dati che devono essere visibili in tutte le parti del progetto
    /// </summary>
    public static class CommonData {
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

        //LISTA DEVICE NON CONFIGUARATI
        // key = IPAddress device
        // value = evento associato per la sincronizzazione tra i thread di interfaccia grafica e di gestione del socket
        public static ConcurrentDictionary<string, ManualResetEvent> lstNoConfDevices = new ConcurrentDictionary<string, ManualResetEvent>();
        public static string paramConfDevice = null;

        //delegato per lanciare gli eventi dopo la modifica della lstConfDevices
        public static void OnLstNoConfDevicesChanged(object sender, EventArgs e) {
            EventHandler handler = LstNoConfDevicesChanged;
            if (handler != null) {
                handler(sender, e);
            }
        }

        //event a cui iscriversi per rilevare la modifica sulla lstConfDevices
        public static event EventHandler LstNoConfDevicesChanged;
    }
}
