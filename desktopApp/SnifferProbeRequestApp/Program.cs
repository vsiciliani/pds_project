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

            ThreadGestioneWifi threadGestioneWifi = null;
            try {
                NetworkSettings settings;
                try {
                    settings = new NetworkSettings(
                        Convert.ToInt32(ConfigurationManager.AppSettings["servicePort"])
                    );
                    if (settings.servicePort <= 0)
                        throw new ArgumentOutOfRangeException();
                } catch (Exception ex) {
                    if (ex is FormatException || ex is OverflowException || ex is ArgumentOutOfRangeException) {
                        MessageBox.Show("Le configurazioni settate sono errate.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    throw ex;
                }

                try {
                    threadGestioneWifi = ThreadGestioneWifi.getInstance(settings);
                } catch (SnifferAppException e) {
                    MessageBox.Show(e.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            } catch (Exception) {
                MessageBox.Show("Si è verificato un errore generico nell'esecuzione del programma", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                threadGestioneWifi.stop();
                return;
            }

            try {
                threadGestioneWifi.stop();
            } catch (SnifferAppException e) {
                MessageBox.Show(e.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }
    }
}
