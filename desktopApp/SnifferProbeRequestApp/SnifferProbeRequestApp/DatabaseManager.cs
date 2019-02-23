using System;
using System.Collections.Generic;
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
        private String connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\msys32\\home\\SicilianiVi\\esp\\pds_project\\desktopApp\\SnifferProbeRequestApp\\SnifferProbeRequestApp\\DBApp.mdf;Integrated Security=True";
        private SqlConnection connection;
        
        private DatabaseManager() {
            //TODO: gestire eccezione connessione DB
            connection = new SqlConnection(connectionString);
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
            connection.Open();
            MessageBox.Show("Connection opened");
            if (command.ExecuteNonQuery() == packets.listPacketInfo.Count)
            {
                Utils.logMessage(this.ToString(), "INSERT effettuata con successo");
            }
        }

        //aggrega i dati "raw", calcola la posizione, salva i dati nella tabella assembled
        //e pulisce i dati "raw"
        private void updateAssembled()
        {

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
