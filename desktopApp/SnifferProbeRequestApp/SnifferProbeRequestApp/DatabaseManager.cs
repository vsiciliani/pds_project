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
        private static DatabaseManager instance = null;
        private String connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename="+ Environment.CurrentDirectory + "\\DBApp.mdf;Integrated Security=True;MultipleActiveResultSets=True;";
        private SqlConnection connection;
        
        private DatabaseManager() {
            //TODO: gestire eccezione connessione DB
            Console.WriteLine("Connection String: " + connectionString);
            connection = new SqlConnection(connectionString);
            try {
                connection.Open();
            } catch (SqlException sqle) {
                SnifferAppException exception = new SnifferAppException("Errore di connessione durante l'apertura della stessa", sqle);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                throw exception;
            } catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore generico durante l'apertura della connessione", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                throw exception;
            }
        }

        static public DatabaseManager getInstance()
        {
            if (instance == null)
            {
                instance = new DatabaseManager();
            }
            return instance;
        }

        //TODO: gestire eccezione insert errata
        //salva i dati ricevuti nella tabella dei dati "raw"
        public void saveReceivedData(PacketsInfo packets, IPAddress ipAddress)
        {
            Console.WriteLine("Num packets: " + packets.listPacketInfo.Count);
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

            Console.WriteLine(query);

            SqlCommand command = new SqlCommand(query.ToString(), connection);
            
            if (command.ExecuteNonQuery() == packets.listPacketInfo.Count)
            {
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "INSERT effettuata con successo");
            }

            updateAssembled();
        }

        //aggrega i dati "raw", calcola la posizione, salva i dati nella tabella assembled
        //e pulisce i dati "raw"
        private void updateAssembled()
        {
            //trovo i device che sto utilizzando
            Dictionary<String, String> devices = new Dictionary<String, String>();

            int id = 1;

            foreach (String device in CommonData.lstConfDevices.Keys) {
                devices.Add("d"+id.ToString(), device);
                id++;
            }

            //StringBuilders per comporre dinamicamente la query
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
            Utils.logMessage(this.ToString(), Utils.LogCategory.Info, query);

            DataTable table = new DataTable();
            try {
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(table);
            } catch (Exception e){
                SnifferAppException exception = new SnifferAppException("Errore durante la lettura dei dati dal DB", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                throw exception;
            }

            //controllo se devo assemblare dei dati
            if (table.Rows.Count == 0) return; 

            List<AssembledPacketInfo> lstAssembledInfo = new List<AssembledPacketInfo>();

            //delete query dei processati dalla tabella "raw"
            StringBuilder deleteQuery = new StringBuilder("DELETE FROM [dbo].[Packets] WHERE id IN (");

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
                    Int32 id_packet = (Int32)record[device.Key + "_id"];
                    deleteQuery.Append(id_packet.ToString()+",");
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
            try {
                if (command.ExecuteNonQuery() == lstAssembledInfo.Count)
                {
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "INSERT in AssembledPacketInfo effettuata con successo");
                }
            } catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore durante la INSERT dei dati nel DB", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                throw exception;
            }


            //elimino gli id dalla tabella "raw"
            deleteQuery.Remove(deleteQuery.Length - 1, 1); //elimino l'ultima virgola nalla IN
            deleteQuery.Append(")"); //chiudo la parentesi della IN
            Console.WriteLine("DELETE QUERY: " + deleteQuery.ToString());
            
            SqlCommand deleteCommand = new SqlCommand(deleteQuery.ToString(), connection);
            try
            {
                if (deleteCommand.ExecuteNonQuery() == lstAssembledInfo.Count)
                {
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "DELETE da Packets effettuata con successo");
                }
            }
            catch (Exception e)
            {
                SnifferAppException exception = new SnifferAppException("Errore durante la DELETE dei dati dalla tabella Packets", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                throw exception;
            }
        }

        //conta il numero di Device Univoci presenti continuativamente nel periodo interessato (es. 5 min)
        public KeyValuePair<DateTime, Int32> countDevice()
        {

            //String selectQuery = "SELECT Current_TimeStamp  AS date_time, COUNT(DISTINCT sourceAddress) AS countDevice FROM dbo.AssembledPacketInfo WHERE timestamp_packet > DateADD(mi, -5, Current_TimeStamp)";
            String selectQuery = "SELECT Current_TimeStamp  AS date_time, COUNT(DISTINCT sourceAddress) AS countDevice FROM dbo.AssembledPacketInfo";

            DataTable resultCount = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand(selectQuery, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(resultCount);
            }
            catch (Exception e)
            {
                SnifferAppException exception = new SnifferAppException("Errore durante la lettura dei dati dal DB", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                throw exception;
            }

            DateTime time = (DateTime)resultCount.Rows[0]["date_time"];
            Int32 count = (Int32)resultCount.Rows[0]["countDevice"];
            KeyValuePair<DateTime, Int32> result = new KeyValuePair<DateTime, Int32>(time, count);

            return result;
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
