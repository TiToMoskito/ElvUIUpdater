using System;
using System.Windows.Forms;

namespace ElvUIUpdate
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            ElvUIUpdateApplication.ElvUI.printUpdateEvent += printUpdateEvent;
            ElvUIUpdateApplication.ElvUI.isRunningEvent += isRunningEvent;
        }

        private void isRunningEvent(bool _running)
        {
            if (_running)
                startToolStripMenuItem.Text = "&Stop";
            else
                startToolStripMenuItem.Text = "&Start";
        }

        private void printUpdateEvent(string _message, string _title, bool _showBaloon)
        {          
            listBox1.Invoke((MethodInvoker)delegate {
                listBox1.Items.Add(string.Format("[{0}] {1}: {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), _title, _message));

                int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                if (_showBaloon)
                    ElvUIUpdateApplication.ShowBaloon(_message, string.Format("ElvUIUpdate {0}", _title));
            });
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ElvUIUpdateApplication.ElvUI.isRunning)
                ElvUIUpdateApplication.ElvUI.Stop();
            else
                ElvUIUpdateApplication.ElvUI.Start();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings window = new Settings();
            window.Show();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Info window = new Info();
            window.Show();
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                ElvUIUpdateApplication.NotifyIcon.Visible = true;
                ShowInTaskbar = false;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                Show();
                ElvUIUpdateApplication.NotifyIcon.Visible = false;
                ShowInTaskbar = true;
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prompt user to save his data
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }

            // Autosave and clear up ressources
            if (e.CloseReason == CloseReason.WindowsShutDown)
            { 
            }
        }
    }
}
