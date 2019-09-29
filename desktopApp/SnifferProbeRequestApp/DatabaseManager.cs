using SnifferProbeRequestApp.valueClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace SnifferProbeRequestApp {
    /// <summary>
    /// Classe Singleton per la gestione della connessione e delle query verso il DB
    /// </summary>
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
        ///<summary>Ritorna l'istanza della classe DatabaseManager se già creata, oppure la istanzia</summary>
        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nell'apertura della connessione al DB</exception>
        ///<returns>L'istanza della classe DatabaseManager</returns>
        static public DatabaseManager getInstance() {
            if (instance == null) {
                instance = new DatabaseManager();
            }
            return instance;
        }

        /// <summary>
        /// Salva i dati ricevuti nella tabella dei dati "raw"
        /// </summary>
        /// <param name="packets">Lista dei pacchetti da salvare</param>
        /// <param name="ipAddress">Ip del dispositivo che ha catturato i pacchetti</param>
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

            int queryResult = runInsertDelete(query.ToString(), null);
            if (queryResult == packets.listPacketInfo.Count)
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "INSERT in Packets effettuata con successo");
            else
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "INSERT in Packets fallita");

            updateAssembled();
        }

        /// <summary>
        /// Aggrega i dati "raw" (Packets), calcola la posizione e salva i dati nella tabella "assembled".
        /// Elimina dalla tabella "raw" (Packets) i dati elaborati
        /// </summary>
        private void updateAssembled() {
            //trovo i device che sto utilizzando
            Dictionary<string, string> devices = new Dictionary<string, string>();

            int id = 1;

            foreach (string device in ConfDevice.lstConfDevices.Keys) {
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

            DataTable tablePackets = runSelect(queryPackets, null);

            //controllo se devo assemblare dei dati
            if (tablePackets.Rows.Count == 0) return; 

            List<AssembledPacketInfo> lstAssembledInfo = new List<AssembledPacketInfo>();

            //delete query dei dati processati dalla tabella "raw" (Packets) utilizzzando gli id
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

            int queryResult = runInsertDelete(insertQuery.ToString(), null);
            if (queryResult == lstAssembledInfo.Count)
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "INSERT in AssembledPacketInfo effettuata con successo");
            else
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "INSERT in AssembledPacketInfo fallita");

            //elimino gli id dalla tabella "raw"
            deleteQuery.Remove(deleteQuery.Length - 1, 1); //elimino l'ultima virgola nalla IN
            deleteQuery.Append(")"); //chiudo la parentesi della IN

            queryResult = runInsertDelete(deleteQuery.ToString(), null);
            if (queryResult > 0)
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "DELETE da Packets effettuata con successo");
            else
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "DELETE da Packets fallita");
        }

        ///<summary>Conta il numero di Device Univoci presenti continuativamente nel periodo interessato (es. 5 min)</summary>
        ///<exception cref = "SnifferAppSqlException">Eccezione lanciata in caso di errore nella lettura dei dati del DB</exception>
        ///<returns>Istanza delle oggetto CountDevice con il timestamp di esecuzione della query e il numero di dispositvi</returns>
        public CountDevice countDevice() {

            string selectQuery = @"SELECT Current_TimeStamp as date_time, COUNT(DISTINCT sourceAddress) as countDevice
                                    FROM (
	                                    SELECT sourceAddress,
		                                    timestamp_packet as startTimestamp,
		                                    CASE WHEN flag = 1 THEN LAG(timestamp_packet, 1,0) OVER (PARTITION BY sourceAddress ORDER BY timestamp_packet desc) ELSE null END stopTimestamp
	                                    FROM (
		                                    SELECT sourceAddress,
			                                    timestamp_packet,
			                                    prev,
			                                    succ,
			                                    CASE WHEN (DATEDIFF(MINUTE,timestamp_packet,succ) >= 2) THEN 2
					                                 WHEN (DATEDIFF(MINUTE,timestamp_packet,succ) < 0) THEN 2
                                                     WHEN (DATEDIFF(MINUTE,prev,timestamp_packet) >= 2) THEN 1
					                                 ELSE 0 END flag
		                                    FROM (
			                                    SELECT sourceAddress, 
				                                    timestamp_packet,
				                                    LAG(timestamp_packet, 1,0) OVER (PARTITION BY sourceAddress ORDER BY timestamp_packet asc) as prev,
				                                    LAG(timestamp_packet, 1,0) OVER (PARTITION BY sourceAddress ORDER BY timestamp_packet desc) as succ
			                                    FROM [dbo].[AssembledPacketInfo]
		                                    ) a
	                                    ) b
	                                    WHERE flag <> 0
                                    ) c
                                    WHERE startTimestamp < stopTimestamp
                                    AND startTimestamp < DATEADD(SECOND, -300, GETUTCDATE())
                                    AND stopTimestamp > DATEADD(SECOND, -120, GETUTCDATE())";

            DataTable resultQuery = runSelect(selectQuery, null);

            return new CountDevice((DateTime)resultQuery.Rows[0]["date_time"],
                (int)resultQuery.Rows[0]["countDevice"]);
        }

        ///<summary>Ritorna i punti dei device rilevati nell'ultimo minuto</summary>
        ///<exception cref = "SnifferAppSqlException">Eccezione lanciata in caso di errore nella lettura dei dati del DB</exception>
        ///<returns>Lista delle posizioni dei device rilevati nell'ultimo minuto</returns>
        public List<DevicePosition> devicesPosition() {

            List<DevicePosition> points = new List<DevicePosition>();

            string selectQuery = @"SELECT sourceAddress, AVG(x_position) as x_position, AVG(y_position) as y_position
                                   FROM dbo.AssembledPacketInfo
                                   WHERE timestamp_packet > DateADD(mi, -1, GETUTCDATE())
                                   GROUP BY sourceAddress";

            DataTable resultQuery = runSelect(selectQuery, null);

            foreach (DataRow record in resultQuery.Rows) {
                points.Add(
                    new DevicePosition((String)record["sourceAddress"],
                        (Double)record["x_position"],
                        (Double)record["y_position"],
                        DateTime.UtcNow) //il tempo in questo caso non è importante e quindi lo setto a Now
                    );
            }
            return points;
        }

        ///<summary>Calcola i periodi in cui sono rilevati i dispositivi piu frequenti (TOP N) a partire da un timestamp</summary>
        ///<param name="numDevice">Numero dei dispositivi da analizzare (TOP N)</param>
        ///<param name="dateLimit">Datetime da cui eseguire l'analisi</param>
        ///<returns>Lista dei periodi di connessione dei TOP N devices</returns>
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
			                                    CASE WHEN (DATEDIFF(MINUTE,timestamp_packet,succ) >= 2) THEN 2
					                                 WHEN (DATEDIFF(MINUTE,timestamp_packet,succ) < 0) THEN 2
                                                     WHEN (DATEDIFF(MINUTE,prev,timestamp_packet) >= 2) THEN 1
					                                 ELSE 0 END flag
		                                    FROM (
			                                    SELECT sourceAddress, 
				                                    timestamp_packet,
				                                    LAG(timestamp_packet, 1,0) OVER (PARTITION BY sourceAddress ORDER BY timestamp_packet asc) as prev,
				                                    LAG(timestamp_packet, 1,0) OVER (PARTITION BY sourceAddress ORDER BY timestamp_packet desc) as succ
			                                    FROM [dbo].[AssembledPacketInfo]
			                                    WHERE timestamp_packet > @datelimit
		                                    ) a
	                                    ) b
	                                    WHERE flag <> 0
                                    ) c
                                    WHERE startTimestamp < stopTimestamp
                                    )
                                    SELECT conn.sourceAddress, conn.startTimestamp, conn.stopTimestamp
                                    FROM ConnectionPeriod conn, (
                                    SELECT TOP " + numDevice+@" sourceAddress, SUM(DATEDIFF(second,startTimestamp,stopTimestamp)) as NumSecond
	                                    FROM ConnectionPeriod
	                                    GROUP BY sourceAddress
	                                    ORDER BY NumSecond desc) topDev
                                    WHERE conn.sourceAddress = topDev.sourceAddress";

            DataTable resultQuery = runSelect(selectQuery, new Dictionary<string, object>() {
                ["@datelimit"] = dateLimit
            });

            foreach (DataRow record in resultQuery.Rows) {
                devicePeriod.Add(
                    new ConnectionPeriod((string)record["sourceAddress"],
                        (DateTime)record["startTimestamp"],
                        (DateTime)record["stopTimestamp"])
                    );
            }
            return devicePeriod;
        }

        ///<summary>Ritorna i devices che sono stati rilevati dal sistema tra le date indicate</summary>
        ///<param name="dateMin">Limite temporale inferiore in cui ricercare</param>
        ///<param name="dateMax">Limite temporale superiore in cui ricercare</param>
        ///<returns>Lista dei devices rilevati</returns>
        ///<exception cref = "SnifferAppSqlException">Eccezione lanciata in caso di errore nella lettura dei dati del DB</exception>
        public List<string> connectedDeviceInPeriod(DateTime dateMin, DateTime dateMax) {
            List<string> devices = new List<string>();

            string selectQuery = @"SELECT DISTINCT sourceAddress
                                   FROM dbo.AssembledPacketInfo
                                   WHERE timestamp_packet >= @datemin 
                                   AND timestamp_packet <= @datemax";

            DataTable resultQuery = runSelect(selectQuery, new Dictionary<string, object>() {
                ["@datemin"] = dateMin,
                ["@datemax"] = dateMax
            });
            
            foreach (DataRow record in resultQuery.Rows) {
                devices.Add((string)record["sourceAddress"]);
            }

            return devices;
        }

        ///<summary>Ritorna le posizioni in cui è stato rilevato un device tra le date indicate</summary>
        ///<param name="device">Device da ricercare</param>
        ///<param name="dateMin">Limite temporale inferiore in cui ricercare</param>
        ///<param name="dateMax">Limite temporale superiore in cui ricercare</param>
        ///<returns>Lista delle posizioni in cui il device è stato rilevato</returns>
        ///<exception cref = "SnifferAppSqlException">Eccezione lanciata in caso di errore nella lettura dei dati del DB</exception>
        public List<DevicePosition> deviceMovement(string device, DateTime dateMin, DateTime dateMax) {

            List<DevicePosition> positions = new List<DevicePosition>();

            string selectQuery = @"SELECT sourceAddress, timestamp_packet, AVG(x_position) as x_position, AVG(y_position) as y_position
                                   FROM dbo.AssembledPacketInfo
                                   WHERE timestamp_packet >= @datemin 
                                   AND timestamp_packet <= @datemax 
                                   AND sourceAddress = @device 
                                   GROUP BY sourceAddress, timestamp_packet ORDER BY timestamp_packet";

            DataTable resultQuery = runSelect(selectQuery, new Dictionary<string, object>() {
                ["@datemin"] = dateMin,
                ["@datemax"] = dateMax,
                ["@device"] = device
            });

            foreach (DataRow record in resultQuery.Rows) {
                positions.Add(
                    new DevicePosition((string)record["sourceAddress"],
                                       (double)record["x_position"],
                                       (double)record["y_position"],
                                       (DateTime)record["timestamp_packet"])
                    );
            }

            return positions;
        }

        ///<summary>Esegue una select sul DB</summary>
        ///<returns>Ritorna un oggetto DataTable con i risultati della query</returns>
        ///<param name="query">Stringa contenente la query da lanciare</param>
        ///<param name="par">Dictionary con i parametri da sostituire nella query (chiave: nome parametro, valore: valore parametro)</param>
        ///<exception cref = "SnifferAppSqlException">Eccezione lanciata in caso di errore nella lettura dei dati del DB</exception>
        private DataTable runSelect(string query, Dictionary<string, object> par) {
            DataTable resultQuery = new DataTable();
            try {
                SqlCommand cmd = new SqlCommand(query, connection);

                if (par != null) {
                    foreach (KeyValuePair<string, object> parametro in par) {
                        cmd.Parameters.AddWithValue(parametro.Key, parametro.Value);
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(resultQuery);
            } catch (Exception e) {
                string message = "Errore durante la lettura dei dati dal DB";
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, message);
                throw new SnifferAppSqlException(message, e);
            }

            return resultQuery;
        }

        ///<summary>Esegue una insert/delete sul DB</summary>
        ///<returns>Ritorna un booleano per indicare se la query è stata eseguita correttamente o no</returns>
        ///<param name="query">Stringa contenente la query da lanciare</param>
        ///<param name="par">Dictionary con i parametri da sostituire nella query (chiave: nome parametro, valore: valore parametro)</param>
        ///<param name="checkCountRow">Numero di righe modificate a DB (per verificare se l'operazione è andata a buon fine</param>
        ///<exception cref = "SnifferAppSqlException">Eccezione lanciata in caso di errore nella lettura dei dati del DB</exception>
        private int runInsertDelete(string query, Dictionary<string, object> par) {
            int numRowModified = 0;
            SqlCommand cmd = new SqlCommand(query, connection);
            try {
                numRowModified = cmd.ExecuteNonQuery();              
            } catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore durante la INSERT dei dati in AssembledPacketInfo", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
            }
            return numRowModified;
        }

        ///<summary>Chiude la connessione con il DB</summary>
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
