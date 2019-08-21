using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferProbeRequestApp.valueClass {
    /// <summary>
    /// Classe statica che permette una gestione della posizione del device utile per la feature che mostra i moviemnti del device
    /// </summary>
    static class CachedDevicePosition {
        public static Dictionary<int, DevicePosition> cache = new Dictionary<int, DevicePosition>();
    }
}
