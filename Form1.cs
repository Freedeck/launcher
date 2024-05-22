using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace Freedeck_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static string home = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string installPath = home + "\\Freedeck";

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = "https://raw.githubusercontent.com/Freedeck/Freedeck/v6/assets/logo_big.png";
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            if (File.Exists(home + "\\Freedeck\\InstallationPath.handoff")) installPath = File.ReadAllText(home+"\\Freedeck\\InstallationPath.handoff");
            richTextBox1.Text = "Installation path: " + installPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 advanced = new Form2();
            this.Hide();
            advanced.ShowDialog();
            this.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!checkBox2.Checked)
            {
                Autoupdater autoupdater = new Autoupdater();
                autoupdater.ShowDialog();
            }
            richTextBox1.Text = "";
            richTextBox1.Text += installPath;
            startProgram("--server-only");
            startProgram("--companion-only", true);
            this.Close();
        }
        private void startProgram(string args, bool useElectron = false)
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.WindowStyle = checkBox1.Checked || useElectron ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
                proc.StartInfo.Arguments = " src\\index.js " + args;
                proc.StartInfo.WorkingDirectory = installPath + "\\freedeck";
                if(useElectron)proc.StartInfo.FileName = installPath+"\\freedeck\\node_modules\\electron\\dist\\electron.exe";
                else proc.StartInfo.FileName = "C:\\Program Files\\nodejs\\node.exe";
                proc.Start();
            }
        }
    }
}
