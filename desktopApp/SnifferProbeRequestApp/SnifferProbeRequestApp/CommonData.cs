using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SnifferProbeRequestApp {
    //Questa classe wrappa i dati che devono essere visibili in tutte le parti del progetto
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
