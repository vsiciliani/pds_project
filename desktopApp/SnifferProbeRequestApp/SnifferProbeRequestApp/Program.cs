﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

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

            ThreadGestioneWifi threadGestioneWifi = ThreadGestioneWifi.getIstance();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());

            
        }
    }
}