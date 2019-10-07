using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SnifferProbeRequestApp.valueClass {
    /// <summary>
    /// Classe statica che wrappa il ConcurrentDictionary che gestisce i rilevatori connessi ma non ancora configurati
    /// </summary>
    public static class NoConfDevice {
        //LISTA DEVICE NON CONFIGUARATI
        // key = IPAddress device
        // value = evento associato per la sincronizzazione tra i thread di interfaccia grafica e di gestione del socket
        public static ConcurrentDictionary<string, ManualResetEvent> lstNoConfDevices = new ConcurrentDictionary<string, ManualResetEvent>();
        
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
