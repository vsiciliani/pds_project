using SnifferProbeRequestApp.valueClass;
using System;
using System.Configuration;
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

            //TODO: eccezione in caso di settaggi errati
            NetworkSettings settings = new NetworkSettings(
                Convert.ToInt32(ConfigurationManager.AppSettings["servicePort"])
            );

            ThreadGestioneWifi threadGestioneWifi = null;

            try {
                threadGestioneWifi = ThreadGestioneWifi.getInstance(settings);
            } catch (SnifferAppException e) {
                MessageBox.Show(e.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            } catch (Exception) {
                MessageBox.Show("Si è verificato un errore generico nell'esecuzione del programma", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                threadGestioneWifi.stop();
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
