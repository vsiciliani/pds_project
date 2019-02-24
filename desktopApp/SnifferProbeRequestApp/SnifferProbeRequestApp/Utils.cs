using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SnifferProbeRequestApp
{
    static class Utils
    {
        static public bool IsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static public void RestartElevated()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            startInfo.Verb = "runas";
            try
            {
                Process p = Process.Start(startInfo);
            }
            catch
            {

            }

            System.Windows.Forms.Application.Exit();
        }

        public class LogCategory
        {
            private LogCategory(string value) { Value = value; }

            public string Value { get; set; }

            public static LogCategory Info { get { return new LogCategory("Info"); } }
            public static LogCategory Warning { get { return new LogCategory("Warning"); } }
            public static LogCategory Error { get { return new LogCategory("Error"); } }
        }

        static public void logMessage(String classe, LogCategory category, String message)
        {
            String timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine(timestamp + " | [" + category.Value + "] | " + classe + " | " + message);
        }
    }
}
