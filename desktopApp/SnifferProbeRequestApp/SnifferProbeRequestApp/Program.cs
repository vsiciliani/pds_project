using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SnifferProbeRequestApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main() {
            //controllo di avere i permessi di amministratore
            Utils.logMessage("MAIN", Utils.LogCategory.Info, "Is Admin? " + Utils.IsAdmin());
            //Console.WriteLine("Is Admin? " + Utils.IsAdmin());
            if (!Utils.IsAdmin()) {
                //Utils.RestartElevated();
                //return;
            }

            ThreadGestioneWifi threadGestioneWifi;

            try {
                threadGestioneWifi = ThreadGestioneWifi.getInstance();
            } catch (SnifferAppException e) {
                MessageBox.Show(e.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            } catch (Exception) {
                MessageBox.Show("Si è verificato un errore generico nell'esecuzione del programma", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
            try {
                threadGestioneWifi.stop();
            } catch (SnifferAppException e) {
                MessageBox.Show(e.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }
    }
}
