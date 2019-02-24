using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnifferProbeRequestApp
{
    class DatabaseManager
    {
        private static DatabaseManager istance = null;
        private String connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename="+ Environment.CurrentDirectory + "\\DBApp.mdf;Integrated Security=True";
       // private String connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Vincenzo\\Desktop\\universita\\programmazione di sistema\\Repo_Gitlab\\pds_project\\desktopApp\\SnifferProbeRequestApp\\SnifferProbeRequestApp\\DBApp.mdf;Integrated Security=True";
        private SqlConnection connection;
        
        private DatabaseManager() {
            //TODO: gestire eccezione connessione DB
            Console.WriteLine("Connection String: " + connectionString);
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        static public DatabaseManager getIstance()
        {
            if (istance == null)
            {
                istance = new DatabaseManager();
            }
            return istance;
        }

        //TODO: gestire eccezione insert errata
        //salva i dati ricevuti nella tabella dei dati "raw"
        public void saveReceivedData(PacketsInfo packets, IPAddress ipAddress)
        {
            if (packets.listPacketInfo.Count == 0) return;

            StringBuilder query = new StringBuilder("");
            query.Append("INSERT INTO [dbo].[Packets] (SourceAddress, SSID, signalStrength, hashCode, timestamp_packet, device) VALUES ");
            foreach (PacketInfo packet in packets.listPacketInfo)
            {
                query.Append("('" + packet.sourceAddress + "',");
                query.Append("'" + packet.SSID + "',");
                query.Append(packet.signalStrength + ",");
                query.Append("'" + packet.hashCode + "',");
                query.Append(packet.timestamp + ",");
                query.Append("'" + ipAddress.ToString() + "'),");
            }
            query.Remove(query.Length - 1, 1); //elimino l'ultima virgola

            SqlCommand command = new SqlCommand(query.ToString(), connection);
            /* TODO: decommentare
            if (command.ExecuteNonQuery() == packets.listPacketInfo.Count)
            {
                Utils.logMessage(this.ToString(), "INSERT effettuata con successo");
            }*/

            updateAssembled();
        }

        //aggrega i dati "raw", calcola la posizione, salva i dati nella tabella assembled
        //e pulisce i dati "raw"
        private void updateAssembled()
        {
            //trovo i device che sto utilizzando
            //TODO:eliminare questa parte necessaria per i test in cui setto i device
            Dictionary<String, String> devices = new Dictionary<String, String>();
            devices["d1"] = "0.0.0.0";
            devices["d2"] = "1.1.1.1";
            //fine definizione valori di test

            StringBuilder selectQuery = new StringBuilder("");
            StringBuilder fromQuery = new StringBuilder("");
            StringBuilder whereQuery = new StringBuilder("");

            selectQuery.Append("SELECT d1.SourceAddress, d1.SSID, d1.hashCode, d1.SSID, ");
            fromQuery.Append(" FROM ");
            whereQuery.Append(" WHERE");
            //per ogni device aggiungo i campi di select nella query,
            //una lettura sulla tabella Packets e la condizione di join
            foreach (KeyValuePair<String, String> device in devices) {
                String deviceName = device.Key;
                selectQuery.Append(deviceName + ".signalStrength as " + deviceName + "_signalStrength, ");
                selectQuery.Append(deviceName + ".timestamp_packet as " + deviceName + "_timestamp,");
                selectQuery.Append(deviceName + ".id as " + deviceName + "_id,");
                fromQuery.Append("[dbo].[Packets] " + deviceName + ",");
                whereQuery.Append(" " + deviceName + ".device = '" + device.Value + "' AND");
                if (deviceName != "d1") {
                    whereQuery.Append(" d1.hashCode = " + deviceName + ".hashCode AND");
                }
            }

            selectQuery.Remove(selectQuery.Length - 1, 1); //elimino l'ultima virgola
            fromQuery.Remove(fromQuery.Length - 1, 1); //elimino l'ultima virgola
            whereQuery.Remove(whereQuery.Length - 3, 3); //elimino l'ultimo AND

            String query = selectQuery.ToString() + fromQuery.ToString() + whereQuery.ToString();
            Utils.logMessage(this.ToString(), query);

            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            da.Fill(table);
            List<AssembledPacketInfo> lstAssembledInfo = new List<AssembledPacketInfo>();
            
            foreach (DataRow record in table.Rows)
            {
                String hashCode = (String) record["hashCode"];
                String sourceAddress = (String) record["SourceAddress"];
                String SSID = (String) record["SSID"];
                Dictionary<String, Int32> signalStrength = new Dictionary<string, int>();
                Int64 avgTimestamp = 0;
                foreach (KeyValuePair<String, String> device in devices) {
                    signalStrength[device.Key] = (Int32) record[device.Key + "_signalStrength"];
                    avgTimestamp += (Int64)record[device.Key + "_timestamp"];
                }
                avgTimestamp = avgTimestamp / (Int64)devices.Count;

                //calcolare la posizione
                Int32 x_pos = 0;
                Int32 y_pos = 0;

                lstAssembledInfo.Add(new AssembledPacketInfo(sourceAddress,
                    SSID, hashCode, avgTimestamp, x_pos, y_pos));
            }

            //insert nella tabella AssembledPacketInfo le informazioni dei pacchetti
            StringBuilder insertQuery = new StringBuilder();
            insertQuery.Append("INSERT INTO [dbo].[AssembledPacketInfo] (SourceAddress, SSID, hashCode, timestamp_packet, x_position, y_position) VALUES ");
            foreach (AssembledPacketInfo assembledInfo in lstAssembledInfo) {
                insertQuery.Append("('" + assembledInfo.sourceAddress + "',");
                insertQuery.Append("'" + assembledInfo.SSID + "',");
                insertQuery.Append("'" + assembledInfo.hashCode + "',");
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime date = start.AddMilliseconds(assembledInfo.timestamp).ToLocalTime();
                insertQuery.Append("'" + date + "',");
                insertQuery.Append(assembledInfo.x_position + ",");
                insertQuery.Append(assembledInfo.y_position + "),");
            }
            insertQuery.Remove(insertQuery.Length - 1, 1); //elimino l'ultima virgola
            Console.WriteLine("INSERT QUERY: " + insertQuery.ToString());

            SqlCommand command = new SqlCommand(insertQuery.ToString(), connection);
            if (command.ExecuteNonQuery() == lstAssembledInfo.Count)
            {
                Utils.logMessage(this.ToString(), "INSERT in AssembledPacketInfo effettuata con successo");
            }

            //elimino gli id dalla tabella "raw"

        }

        //calcola la posizione del device
        private void getPosition()
        {

        }


        public void closeConnection()
        {
            connection.Close();
        }

    }
}
