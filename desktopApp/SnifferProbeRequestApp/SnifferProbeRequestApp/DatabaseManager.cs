using SnifferProbeRequestApp.valueClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace SnifferProbeRequestApp {
    class DatabaseManager {
        private static DatabaseManager instance = null;
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename="+ Environment.CurrentDirectory + "\\DBApp.mdf;Integrated Security=True;MultipleActiveResultSets=True;";
        private SqlConnection connection;

        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nell'apertura della connessione al DB</exception>
        private DatabaseManager() {
            
            Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Connection String: " + connectionString);
            connection = new SqlConnection(connectionString);
            try {
                connection.Open();
            } catch (SqlException e) {
                string message = "Errore durante l'apertura della connessione con il database";
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, message);
                throw new SnifferAppDBConnectionException(message, e);
            }
        }

        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nell'apertura della connessione al DB</exception>
        static public DatabaseManager getInstance() {
            if (instance == null) {
                instance = new DatabaseManager();
            }
            return instance;
        }

        //TODO: gestire eccezione insert errata
        //salva i dati ricevuti nella tabella dei dati "raw"
        public void saveReceivedData(PacketsInfo packets, IPAddress ipAddress) {
            if (packets.listPacketInfo.Count == 0) return;

            StringBuilder query = new StringBuilder("");
            query.Append("INSERT INTO [dbo].[Packets] (SourceAddress, SSID, signalStrength, hashCode, timestamp_packet, device) VALUES ");
            foreach (PacketInfo packet in packets.listPacketInfo)
            {
                query.Append("('" + packet.sourceAddress + "',");
                query.Append("'" + packet.SSID + "',");
                query.Append(packet.signalStrength + ",");
                query.Append("'" + packet.hashCode + "',");
                query.Append(packet.timestamp*1000 + ","); //*1000 per passare dai secondi ai millisecondi
                query.Append("'" + ipAddress.ToString() + "'),");
            }
            query.Remove(query.Length - 1, 1); //elimino l'ultima virgola

            SqlCommand command = new SqlCommand(query.ToString(), connection);

            try
            {
                if (command.ExecuteNonQuery() == packets.listPacketInfo.Count) {
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "INSERT in Packets effettuata con successo");
                } else {
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Warning, "INSERT in Packets fallita");
                }
            } catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore durante la INSERT dei dati in Packets", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
            }
            updateAssembled();
        }

        //aggrega i dati "raw", calcola la posizione, salva i dati nella tabella assembled
        //e pulisce i dati "raw"
        private void updateAssembled() {
            //trovo i device che sto utilizzando
            Dictionary<string, string> devices = new Dictionary<string, string>();

            int id = 1;

            foreach (string device in CommonData.lstConfDevices.Keys) {
                devices.Add("d"+id.ToString(), device);
                id++;
            }

            //StringBuilders per comporre dinamicamente la query
            StringBuilder selectQuery = new StringBuilder("");
            StringBuilder fromQuery = new StringBuilder("");
            StringBuilder whereQuery = new StringBuilder("");

            //preparo lo statement della query sulla tabella Packets
            //seleziono i campi comuni (MACAddress, SSID e hashCode del pacchetto"
            selectQuery.Append("SELECT d1.SourceAddress, d1.SSID, d1.hashCode, ");
            fromQuery.Append(" FROM ");
            whereQuery.Append(" WHERE");

            //per ogni device aggiungo i campi di select nella query (potenzaSegnale, timestamp, idPacchetto)
            //di lettura sulla tabella Packets e la condizione di join
            foreach (KeyValuePair<string, string> device in devices) {
                string deviceName = device.Key;
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

            string queryPackets = selectQuery.ToString() + fromQuery.ToString() + whereQuery.ToString();
            
            DataTable tablePackets = new DataTable();
            try {
                SqlCommand cmd = new SqlCommand(queryPackets, connection);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(tablePackets);
            } catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore durante la lettura dei dati della tabella Packets", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
            }

            //controllo se devo assemblare dei dati
            if (tablePackets.Rows.Count == 0) return; 

            List<AssembledPacketInfo> lstAssembledInfo = new List<AssembledPacketInfo>();

            //delete query dei processati dalla tabella "raw" Packets utilizzzando gli id
            StringBuilder deleteQuery = new StringBuilder("DELETE FROM [dbo].[Packets] WHERE id IN (");

            foreach (DataRow record in tablePackets.Rows) {
                //leggo dalla tabella i campi comuni
                string hashCode = (string) record["hashCode"];
                string sourceAddress = (string) record["SourceAddress"];
                string SSID = (string) record["SSID"];
                //creo un dizionario per salvarmi le potenze di segnali dei vari dispositivi
                Dictionary<string, int> signalStrength = new Dictionary<string, int>();
                long avgTimestamp = 0;

                //ciclo sui device configurati
                foreach (KeyValuePair<string, string> device in devices) {
                    //salvo nel dizionario la potenza del segnale
                    signalStrength[device.Value] = (int) record[device.Key + "_signalStrength"];
                    avgTimestamp += (long)record[device.Key + "_timestamp"];

                    //leggo l'id del pacchetto per inserirlo nella query di delete dalla tabella Packets
                    int id_packet = (int)record[device.Key + "_id"];
                    deleteQuery.Append(id_packet.ToString()+",");
                }
                //calcolo il timestamp medio di ricezione del pacchetto dai vari devices
                avgTimestamp /= devices.Count;

                //calcolare la posizione
                Tuple<double, double> position = Utils.findPosition(signalStrength);
                
                lstAssembledInfo.Add(new AssembledPacketInfo(sourceAddress,
                    SSID, hashCode, avgTimestamp, position.Item1, position.Item2));
            }

            //insert nella tabella AssembledPacketInfo le informazioni dei pacchetti
            StringBuilder insertQuery = new StringBuilder();
            insertQuery.Append("INSERT INTO [dbo].[AssembledPacketInfo] (SourceAddress, SSID, hashCode, timestamp_packet, x_position, y_position) VALUES ");

            foreach (AssembledPacketInfo assembledInfo in lstAssembledInfo) {
                insertQuery.Append("('" + assembledInfo.sourceAddress + "',");
                insertQuery.Append("'" + assembledInfo.SSID + "',");
                insertQuery.Append("'" + assembledInfo.hashCode + "',");
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime date = start.AddMilliseconds(assembledInfo.timestamp);
                insertQuery.Append("'" + date.ToString("MM/dd/yyyy HH:mm:ss") + "',");
                insertQuery.Append(assembledInfo.x_position.ToString().Replace(",",".") + ",");
                insertQuery.Append(assembledInfo.y_position.ToString().Replace(",", ".") + "),");
            }
            insertQuery.Remove(insertQuery.Length - 1, 1); //elimino l'ultima virgola

            Utils.logMessage(this.ToString(), Utils.LogCategory.Info, insertQuery.ToString());

            SqlCommand command = new SqlCommand(insertQuery.ToString(), connection);
            try {
                if (command.ExecuteNonQuery() == lstAssembledInfo.Count) {
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "INSERT in AssembledPacketInfo effettuata con successo");
                } else {
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Warning, "INSERT in AssembledPacketInfo fallita");
                }
            } catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore durante la INSERT dei dati in AssembledPacketInfo", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
            }

            //elimino gli id dalla tabella "raw"
            deleteQuery.Remove(deleteQuery.Length - 1, 1); //elimino l'ultima virgola nalla IN
            deleteQuery.Append(")"); //chiudo la parentesi della IN
            
            SqlCommand deleteCommand = new SqlCommand(deleteQuery.ToString(), connection);
            try {
                if (deleteCommand.ExecuteNonQuery() == lstAssembledInfo.Count) {
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "DELETE da Packets effettuata con successo");
                }
            }
            catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore durante la DELETE dei dati dalla tabella Packets", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
            }
        }

        //conta il numero di Device Univoci presenti continuativamente nel periodo interessato (es. 5 min)
        ///<exception cref = "SnifferAppSqlException">Eccezione lanciata in caso di errore nella lettura dei dati del DB</exception>
        public CountDevice countDevice() {

            string selectQuery = @"SELECT Current_TimeStamp as date_time, count(*) as countDevice
                                   FROM (
                                     SELECT sourceAddress, MIN(timestamp_packet) as min_timestamp, MAX(timestamp_packet) as max_timestamp
                                     FROM dbo.AssembledPacketInfo
                                     WHERE timestamp_packet > DateADD(mi, -5, GETUTCDATE())
                                     GROUP BY sourceAddress
                                   ) sub_query
                                   WHERE (DATEDIFF(SECOND , min_timestamp , max_timestamp) > 180)";

            DataTable resultCount = new DataTable();
            try {
                SqlCommand cmd = new SqlCommand(selectQuery, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(resultCount);
            }
            catch (Exception e) {
                string message = "Errore durante la lettura dei dati dal DB";
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, message);
                throw new SnifferAppSqlException(message, e);
            }

            return new CountDevice((DateTime)resultCount.Rows[0]["date_time"],
                (int)resultCount.Rows[0]["countDevice"]);
        }

        //ritorna i punti dei device rilevati nell'ultimo minuto
        ///<exception cref = "SnifferAppSqlException">Eccezione lanciata in caso di errore nella lettura dei dati del DB</exception>
        public List<DevicePosition> devicesPosition() {

            List<DevicePosition> points = new List<DevicePosition>();

            string selectQuery = @"SELECT sourceAddress, AVG(x_position) as x_position, AVG(y_position) as y_position
                                   FROM dbo.AssembledPacketInfo
                                   WHERE timestamp_packet > DateADD(mi, -1, GETUTCDATE())
                                   GROUP BY sourceAddress";

            DataTable resultQuery = new DataTable();
            try {
                SqlCommand cmd = new SqlCommand(selectQuery, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(resultQuery);
            } catch (Exception e) {
                string message = "Errore durante la lettura dei dati dal DB";
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, message);
                throw new SnifferAppSqlException(message, e);
            }

            foreach (DataRow record in resultQuery.Rows) {

                points.Add(
                    new DevicePosition((String)record["sourceAddress"],
                        (Double)record["x_position"],
                        (Double)record["y_position"])
                    );
            }
            return points;
        }

        //ritorna i periodi in cui sono rilevati i dispositivi piu frequenti
        ///<exception cref = "SnifferAppSqlException">Eccezione lanciata in caso di errore nella lettura dei dati del DB</exception>
        public List<ConnectionPeriod> longTermStatistic(string numDevice, string dateLimit) {

            List<ConnectionPeriod> devicePeriod = new List<ConnectionPeriod>();

            string selectQuery = @"WITH ConnectionPeriod (sourceAddress, startTimestamp, stopTimestamp)  
                                    AS  
                                    (  
	                                    SELECT sourceAddress,
	                                    startTimestamp,
	                                    stopTimestamp
                                    FROM (
	                                    SELECT sourceAddress,
		                                    timestamp_packet as startTimestamp,
		                                    CASE WHEN flag = 1 THEN LAG(timestamp_packet, 1,0) OVER (PARTITION BY sourceAddress ORDER BY timestamp_packet desc) ELSE null END stopTimestamp
	                                    FROM (
		                                    SELECT sourceAddress,
			                                    timestamp_packet,
			                                    prev,
			                                    succ,
			                                    CASE WHEN (DATEDIFF(MINUTE,prev,timestamp_packet) > 5) THEN 1
					                                    WHEN (DATEDIFF(MINUTE,timestamp_packet,succ) > 5) THEN 2
					                                    WHEN (DATEDIFF(MINUTE,timestamp_packet,succ) < 0) THEN 2
					                                    ELSE 0 END flag
		                                    FROM (
			                                    SELECT sourceAddress, 
				                                    timestamp_packet,
				                                    LAG(timestamp_packet, 1,0) OVER (PARTITION BY sourceAddress ORDER BY timestamp_packet asc) as prev,
				                                    LAG(timestamp_packet, 1,0) OVER (PARTITION BY sourceAddress ORDER BY timestamp_packet desc) as succ
			                                    FROM [dbo].[AssembledPacketInfo]
			                                    WHERE timestamp_packet > '" + dateLimit + @"'
		                                    ) a
	                                    ) b
	                                    WHERE flag <> 0
                                    ) c
                                    WHERE startTimestamp < stopTimestamp
                                    )
                                    SELECT conn.sourceAddress, conn.startTimestamp, conn.stopTimestamp
                                    FROM ConnectionPeriod conn, (
                                    SELECT TOP " + numDevice + @" sourceAddress, SUM(DATEDIFF(second,startTimestamp,stopTimestamp)) as NumSecond
	                                    FROM ConnectionPeriod
	                                    GROUP BY sourceAddress
	                                    ORDER BY NumSecond desc) topDev
                                    WHERE conn.sourceAddress = topDev.sourceAddress";

            DataTable resultQuery = new DataTable();
            try {
                SqlCommand cmd = new SqlCommand(selectQuery, connection);

                SqlDataAdapter da = new SqlDataAdapter(cmd);             
                da.Fill(resultQuery);
            } catch (Exception e) {
                string message = "Errore durante la lettura dei dati dal DB";
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, message);
                throw new SnifferAppSqlException(message, e);
            }

            foreach (DataRow record in resultQuery.Rows) {
                devicePeriod.Add(
                    new ConnectionPeriod((string)record["sourceAddress"],
                        (DateTime)record["startTimestamp"],
                        (DateTime)record["stopTimestamp"])
                    );
            }
            return devicePeriod;
        }

        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nella chiusura della connessione al DB</exception>
        public void closeConnection() {
            try {
                connection.Close();
            } catch (Exception) {
                string message = "Errore durante la chiusura della connessione al DB";
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, message);
                throw new SnifferAppDBConnectionException(message);
            }   
        }
    }
}
