using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnifferProbeRequestApp {
    //Questa classe wrappa i dati che devono essere visibili in tutte le parti del progetto
    public static class CommonData {
        //LISTA DEVICE CONFIGUARATI
        public static ConcurrentDictionary<String, Device> lstConfDevices = new ConcurrentDictionary<String, Device>();

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
        public static ConcurrentDictionary<String, ManualResetEvent> lstNoConfDevices = new ConcurrentDictionary<String, ManualResetEvent>();
        public static String paramConfDevice = null;

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
