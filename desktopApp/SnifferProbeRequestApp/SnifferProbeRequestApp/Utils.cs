using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;

namespace SnifferProbeRequestApp {
    static class Utils {

        static public bool IsAdmin() {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static public void RestartElevated() {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            startInfo.Verb = "runas";
            try {
                Process p = Process.Start(startInfo);
            } catch {

            }

            System.Windows.Forms.Application.Exit();
        }

        private static void startHotspot(string ssid, string key) {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe") {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process process = Process.Start(processStartInfo);

            if (process != null) {
                process.StandardInput.WriteLine("netsh wlan set hostednetwork mode=allow ssid=" + ssid + " key=" + key);
                process.StandardInput.WriteLine("netsh wlan start hosted network");
                process.StandardInput.Close();
                Console.WriteLine("Wifi network started");
            }
        }

        private static void stopHotspot() {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe") {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process process = Process.Start(processStartInfo);

            process.StandardInput.WriteLine("netsh wlan stop hostednetwork");
            process.StandardInput.Close();
            Console.WriteLine("Wifi network closed");
        }

        public class LogCategory {
            private LogCategory(string value) { Value = value; }

            public string Value { get; set; }

            public static LogCategory Info { get { return new LogCategory("Info"); } }
            public static LogCategory Warning { get { return new LogCategory("Warning"); } }
            public static LogCategory Error { get { return new LogCategory("Error"); } }
        }

        static public void logMessage(string classe, LogCategory category, string message) {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try {
                Console.WriteLine(timestamp + " | [" + category.Value + "] | " + classe + " | " + message);
            } catch (IOException) {
                return;
            }
            
        }

        ///<exception cref = "SnifferAppSocketException">Eccezione lanciata in caso di errore nell'invio dei dati sul socket</exception>
        static public void sendMessage(NetworkStream stream, IPEndPoint endPoint, string message) {
            byte[] messageToSend = Encoding.ASCII.GetBytes(message);

            try {
                stream.Write(messageToSend, 0, messageToSend.Length);
                logMessage("Utils.cs -- Send Message", LogCategory.Info, "Receiver: " + endPoint.Address.ToString() + " Message: " + message);
            } catch (IOException e) {
                string errorMessage = "Errore nell'invio dei dati sul socket";
                Utils.logMessage("Utils.cs -- SendMessage", Utils.LogCategory.Error, errorMessage);
                throw new SnifferAppSocketException(errorMessage, e);
            }
        }

        ///<exception cref = "SnifferAppSocketException">Eccezione lanciata in caso di errore nella ricezione dei dati sul socket</exception>
        static public string receiveMessage(NetworkStream stream, IPEndPoint endPoint) {
            string receivedMessage = string.Empty;
            int MAXBUFFER = 4096;

            while (true) {
                byte[] receivedBytes = new byte[MAXBUFFER];
                try {
                    Utils.logMessage("Utils.cs -- ReceviceMessage", Utils.LogCategory.Info, "Device :" + endPoint.Address.ToString() + " In attesa di dati");
                } catch(SocketException)  {
                    Utils.logMessage("Utils.cs -- ReceviceMessage", Utils.LogCategory.Info, "Device : (errore nella lettura dell'IP del device) In attesa di dati");
                }
               
                try {
                    int numBytes = stream.Read(receivedBytes, 0, receivedBytes.Length);
                    receivedMessage += Encoding.ASCII.GetString(receivedBytes, 0, numBytes);
                    if (receivedMessage.IndexOf("//n") > -1) {
                        break;
                    }
                } catch (IOException e) {
                    string errorMessage = "Errore nella ricezione dei dati sul socket";
                    Utils.logMessage("Utils.cs -- ReceiveMessage", Utils.LogCategory.Error, errorMessage);
                    throw new SnifferAppSocketException(errorMessage, e);
                }
            }

            try {
                Utils.logMessage("Utils.cs -- ReceviceMessage", Utils.LogCategory.Info, "Sender: " + endPoint.Address.ToString() + " Ricevuto: " + receivedMessage.Replace("//n", ""));
            } catch (SocketException) {
                Utils.logMessage("Utils.cs -- ReceviceMessage", Utils.LogCategory.Info, "Sender: (errore nella lettura dell'IP del device) Ricevuto: " + receivedMessage.Replace("//n", ""));
            }
            return receivedMessage;
        }

        ///<exception cref = "SnifferAppSocketException">Eccezione lanciata in caso di errore nell'invio dei dati sul socket</exception>
        static public void syncClock(Socket socket) {
            //invio i secondi del timestamp
            DateTime dt1970 = new DateTime(1970, 1, 1);
            long secToSend = (long)(DateTime.Now.ToUniversalTime() - dt1970).TotalSeconds;
            try {
                socket.Send(BitConverter.GetBytes(secToSend));
            } catch (SocketException e) {
                string errorMessage = "Errore nell'invio dei dati sul socket";
                Utils.logMessage("Utils.cs -- syncClock", Utils.LogCategory.Error, errorMessage);
                throw new SnifferAppSocketException(errorMessage, e);
            }
        }

        static public Tuple<double, double> findPosition(Dictionary<string, int> signalStrength) {

            int freqInMHz = 2412;

            Dictionary<string, double> estimatedDistances = new Dictionary<string, double>();

            foreach (KeyValuePair<string, int> signal in signalStrength) {
                //free space loss data
                double esponente = (27.55 - (20 * Math.Log10(freqInMHz)) + Math.Abs(signal.Value)) / 20.0;
                double estimatedDistance = Math.Pow(10.0, esponente);
                estimatedDistances.Add(signal.Key, estimatedDistance);
            }

            Dictionary<string, double> weightDevice = new Dictionary<string, double>();

            double denominatoreWeight = 0;

            foreach (double distance in estimatedDistances.Values) {
                denominatoreWeight += (1 / distance);
            }

            foreach (KeyValuePair<string, double> device in estimatedDistances) {
                double weight = (1 / device.Value) / denominatoreWeight;
                weightDevice.Add(device.Key, weight);
            }

            //calcolo le coordinate
            double x = 0;
            double y = 0;

            foreach (KeyValuePair<string, double> device in weightDevice) {
                Device dev;
                CommonData.lstConfDevices.TryGetValue(device.Key, out dev);

                x += device.Value * dev.x_position;
                y += device.Value * dev.y_position;
            }
            return new Tuple<double, double>(x, y);
        }
    }
}
