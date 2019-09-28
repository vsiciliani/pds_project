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
                settings = new NetworkSettings(
                    Convert.ToInt32(ConfigurationManager.AppSettings["servicePort"])
                );
                if (settings.servicePort <= 1024)
                   throw new ArgumentOutOfRangeException();
               
                threadGestioneWifi = ThreadGestioneWifi.getInstance(settings);

                threadGestioneWifi.start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain(settings));

                if (threadGestioneWifi != null)
                    threadGestioneWifi.stop();

            } catch (SnifferAppException e) {
                MessageBox.Show(e.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (Exception e) {
                if (e is FormatException || e is OverflowException || e is ArgumentOutOfRangeException) 
                    MessageBox.Show("Le configurazioni settate sono errate.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Si è verificato un errore generico nell'esecuzione del programma: " + e.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
    }
}
