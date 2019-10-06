using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;

namespace SnifferProbeRequestApp {

    ///<summary>Classe statica che contiene funzioni di utilità</summary>
    static class Utils {

        ///<summary>Classe statica che permette di definire il livello di log</summary>
        public class LogCategory {
            private LogCategory(string value) { Value = value; }

            public string Value { get; set; }

            public static LogCategory Info { get { return new LogCategory("Info"); } }
            public static LogCategory Warning { get { return new LogCategory("Warning"); } }
            public static LogCategory Error { get { return new LogCategory("Error"); } }
        }

        ///<summary>Scrive un messaggio di log</summary>
        ///<param name="classe">Nome della classe che genera il log</param>
        ///<param name="category">Livello del messaggio di errore</param>
        ///<param name="message">Messaggio da scrivere nel log</param>
        static public void logMessage(string classe, LogCategory category, string message) {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try {
                Console.WriteLine(timestamp + " | [" + category.Value + "] | " + classe + " | " + message);
            } catch (IOException) {
                return;
            }        
        }

        ///<summary>Invia un messaggio su un socket</summary>
        ///<param name="stream">NetworkStream su cui inviare il messaggio</param>
        ///<param name="endPoint">EndPoint della connessione</param>
        ///<param name="message">Messaggio in formato stringa da inviare</param>
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

        ///<summary>Riceve un messagio da un socket in formato stringa</summary>
        ///<param name="stream">NetworkStream da cui ricevere il messaggio</param>
        ///<param name="endPoint">EndPoint della connessione</param>
        ///<returns>Il messaggio ricevuto in formato Stringa</returns>
        ///<exception cref = "SnifferAppSocketException">Eccezione lanciata in caso di errore nella ricezione dei dati sul socket</exception>
        static public string receiveMessage(NetworkStream stream, IPEndPoint endPoint) {
            string receivedMessage = string.Empty;
            int MAXBUFFER = 4096;

            while (true) {
                byte[] receivedBytes = new byte[MAXBUFFER];
                try {
                    Utils.logMessage("Utils.cs -- ReceviceMessage", Utils.LogCategory.Info, "Device: " + endPoint.Address.ToString() + " In attesa di dati");
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
                    throw new SnifferAppSocketTimeoutException(errorMessage, e);
                }
            }
            receivedMessage = receivedMessage.Remove(receivedMessage.Length - 3); //elimino gli ultimi 3 caratteri
            try {
                Utils.logMessage("Utils.cs -- ReceviceMessage", Utils.LogCategory.Info, "Sender: " + endPoint.Address.ToString() + " Ricevuto: " + receivedMessage);
            } catch (SocketException) {
                Utils.logMessage("Utils.cs -- ReceviceMessage", Utils.LogCategory.Info, "Sender: (errore nella lettura dell'IP del device) Ricevuto: " + receivedMessage);
            }
            return receivedMessage;
        }

        ///<summary>Invia un messaggio su un socket contentente il current timestamp in secondi per permette la sincronizzazione del clock</summary>
        ///<param name="socket">Socket su cui inviare il messaggio</param>
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

        ///<summary>Calcola la posizione di un dispositivo wifi rilevato rispetto ai ricevitori registrati</summary>
        ///<param name="signalStrength">Dizionario con le potenze registrate dai vari dispositivi</param>
        ///<returns>Le coordinate x e y del dispositivo rilevato</returns>
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
                ConfDevice.lstConfDevices.TryGetValue(device.Key, out dev);

                x += device.Value * dev.x_position;
                y += device.Value * dev.y_position;
            }
            return new Tuple<double, double>(x, y);
        }
    }
}
