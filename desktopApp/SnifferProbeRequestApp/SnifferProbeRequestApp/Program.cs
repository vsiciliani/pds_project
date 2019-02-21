using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SnifferProbeRequestApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main()
        {
            //controllo di avere i permessi di amministratore
            Console.WriteLine("Is Admin? " + Utils.IsAdmin());
            if (!Utils.IsAdmin())
            {
                //Utils.RestartElevated();
                //return;
            }

            // Build connection string

            /*
            string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\msys32\\home\\SicilianiVi\\esp\\pds_project\\desktopApp\\SnifferProbeRequestApp\\SnifferProbeRequestApp\\DBApp.mdf;Integrated Security=True";
            string queryString = "SELECT id, sourceAddress FROM [dbo].[Packets]";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                //command.Parameters.AddWithValue("@tPatSName", "Your-Parm-Value");
                connection.Open();
                MessageBox.Show("Connection opened");
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}, {1}",
                        reader["id"], reader["sourceAddress"]));// etc
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            */        

            //TODO: decommentare
            ThreadGestioneWifi threadGestioneWifi = ThreadGestioneWifi.getIstance();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());

            
        }
    }
}
