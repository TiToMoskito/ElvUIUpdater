using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace ElvUIUpdate
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            textBox1.Text = ElvUIUpdateApplication.Config.Read("Settings", "Path");
            numericUpDown1.Value = decimal.Parse(ElvUIUpdateApplication.Config.Read("Settings", "Interval"));
            autostartCheckbox.Checked = bool.Parse(ElvUIUpdateApplication.Config.Read("Settings", "Autostart"));
            startCheckonStartCheckbox.Checked = bool.Parse(ElvUIUpdateApplication.Config.Read("Settings", "AutostartCheck"));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ElvUIUpdateApplication.Config.Write("Settings", "Path", textBox1.Text);
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult result = objDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = objDialog.SelectedPath;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ElvUIUpdateApplication.Config.Write("Settings", "Interval", numericUpDown1.Value.ToString());
        }

        private void autostartCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ElvUIUpdateApplication.Config.Write("Settings", "Autostart", autostartCheckbox.Checked.ToString());

            if(autostartCheckbox.Checked)
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    key.SetValue("ElvUIUpdate", "\"" + Application.ExecutablePath + "\"");
                }
            }
            else
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    key.DeleteValue("ElvUIUpdate", false);
                }
            }
        }

        private void startCheckonStartCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ElvUIUpdateApplication.Config.Write("Settings", "AutostartCheck", startCheckonStartCheckbox.Checked.ToString());
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
