using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace ElvUIUpdate
{
    //*****************************************************************************
    static class ElvUIUpdateApplication
    {
        public static NotifyIcon NotifyIcon { get; private set; }
        public static ElvUI ElvUI { get; private set; }
        public static Config Config { get; private set; }

        private static Main MainWindow;

        //==========================================================================

        [STAThread]
        public static void Main(string[] astrArg)
        {
            ContextMenu cm;
            MenuItem miCurr;
            int iIndex = 0;

            // Kontextmenü erzeugen
            cm = new ContextMenu();

            // Kontextmenüeinträge erzeugen
            miCurr = new MenuItem();
            miCurr.Index = iIndex++;
            miCurr.Text = "&Open";           // Eigenen Text einsetzen
            miCurr.Click += new System.EventHandler(OpenClick);
            cm.MenuItems.Add(miCurr);

            // Kontextmenüeinträge erzeugen
            miCurr = new MenuItem();
            miCurr.Index = iIndex++;
            miCurr.Text = "&Close";
            miCurr.Click += new System.EventHandler(ExitClick);
            cm.MenuItems.Add(miCurr);

            // NotifyIcon selbst erzeugen
            NotifyIcon = new NotifyIcon();
            NotifyIcon.Icon = new Icon("logo.ico"); // Eigenes Icon einsetzen
            NotifyIcon.Text = "ElvUI Update";   // Eigenen Text einsetzen
            NotifyIcon.Visible = true;
            NotifyIcon.ContextMenu = cm;
            NotifyIcon.DoubleClick += new EventHandler(OpenClick);

            Config = new Config(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
            ElvUI = new ElvUI();

            MainWindow = new Main();
            MainWindow.Show();

            if (bool.Parse(Config.Read("Settings", "AutostartCheck")))
                ElvUI.Start();

            // Ohne Appplication.Run geht es nicht
            Application.Run(MainWindow);            
        }

        //==========================================================================

        private static void ExitClick(Object sender, EventArgs e)
        {
            NotifyIcon.Dispose();
            ElvUI.Stop();
            Application.Exit();
        }

        //==========================================================================

        private static void OpenClick(Object sender, EventArgs e)
        {
            MainWindow.Show();
            MainWindow.WindowState = FormWindowState.Normal;
            MainWindow.BringToFront();
        }

        //==========================================================================

        public static void ShowBaloon(string _message, string _title)
        {
            NotifyIcon.BalloonTipText = _message;
            NotifyIcon.BalloonTipTitle = _title;
            NotifyIcon.ShowBalloonTip(1000);
        }
    }
}
