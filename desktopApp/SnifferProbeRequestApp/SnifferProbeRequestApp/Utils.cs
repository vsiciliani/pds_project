using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnifferProbeRequestApp
{
    static class Utils {

        static int MAXBUFFER = 4096;

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

        static public void logMessage(String classe, LogCategory category, String message) {
            String timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine(timestamp + " | [" + category.Value + "] | " + classe + " | " + message);
        }

        static public void sendMessage(NetworkStream stream, IPEndPoint endPoint, String message) {
            byte[] messageToSend = messageToSend = Encoding.ASCII.GetBytes(message);
            
            try {
                    stream.Write(messageToSend, 0, messageToSend.Length);
                logMessage("Utils.cs -- Send Message", LogCategory.Info,
                    "Receiver: " + endPoint.Address.ToString() + " Message: " + message);
            } catch (SocketException e) {
                SnifferAppTimeoutSocketException exception = new SnifferAppTimeoutSocketException("Superato timeout di attesa per l'invio sul socket", e);
                throw exception;
            }
        }

        static public String receiveMessage(NetworkStream stream, IPEndPoint endPoint) {
            String receivedMessage = String.Empty;
            
            while (true) {
                byte[] receivedBytes = new byte[MAXBUFFER];
                Utils.logMessage("Utils.cs -- ReceviceMessage", Utils.LogCategory.Info, 
                    "Device :" + endPoint.Address.ToString() + " In attesa di dati");
                try {
                    int numBytes = stream.Read(receivedBytes, 0, receivedBytes.Length);
                    receivedMessage += Encoding.ASCII.GetString(receivedBytes, 0, numBytes);
                    if (receivedMessage.IndexOf("//n") > -1) {
                        break;
                    }
                } catch (System.IO.IOException e) {
                    SnifferAppTimeoutSocketException exception = new SnifferAppTimeoutSocketException("Superato timeout di attesa per la ricezione sul socket", e);
                    throw exception;
                }
            }
            Utils.logMessage("Utils.cs -- ReceviceMessage", Utils.LogCategory.Info,
                "Sender: " + endPoint.Address.ToString() + " Ricevuto: " + receivedMessage.Replace("//n", ""));
            return receivedMessage;
        }


        static public void syncClock(Socket socket) {
            //invio i secondi del timestamp
            DateTime dt1970 = new DateTime(1970, 1, 1);
            long secToSend = (long)((DateTime.Now.ToUniversalTime() - dt1970).TotalSeconds);
            try {
                socket.Send(BitConverter.GetBytes(secToSend));
            } catch (SocketException e) {
                SnifferAppTimeoutSocketException exception = new SnifferAppTimeoutSocketException("Superato timeout di attesa per l'invio sul socket", e);
                throw exception;
            }
        }
    }
}
