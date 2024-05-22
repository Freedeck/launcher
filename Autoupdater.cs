using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Freedeck_Launcher
{
    public partial class Autoupdater : Form
    {
        public Autoupdater()
        {
            InitializeComponent();
        }
        WebClient wc;
        private void Retry()
        {
            try
            {
                byte[] dl =wc.DownloadData(new Uri(textBox2.Text));
                string[] file = Encoding.Default.GetString(dl).Split('\n');
                string ucfg = File.ReadAllText(textBox1.Text + "\\src\\configs\\config.fd.js");
                string selected = file[0];
                if (ucfg.Contains("release:\"dev\""))
                {
                    selected = file[1];
                }
                string uver = File.ReadAllText(textBox1.Text + "\\package.json");
                string version = uver.Split(new string[] { "\"version\": \"" }, StringSplitOptions.None)[1].Split('"')[0];

                bool needsUpdate = false;
                needsUpdate = (version == selected);
                if(!needsUpdate)
                {
                    this.Close();
                } else
                {
                    label1.Text = "Updating! (v" + version + " -> v" + selected + ")";
                    RunUpdater();
                    this.Close();
                }
            } catch(Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void RunUpdater()
        {
            using(Process proc = new Process())
            {
                progressBar1.Value = 0;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.WorkingDirectory = textBox1.Text;
                proc.StartInfo.FileName = "git";
                proc.StartInfo.Arguments = " pull";
                progressBar1.Value = 50;
                proc.Start();
                proc.WaitForExit();
                progressBar1.Value = 100;
            }
        }

        private void Autoupdater_Load(object sender, EventArgs e)
        {
            textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Freedeck\\freedeck";
            wc = new WebClient();
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            label1.Text = "Getting " + textBox2.Text;
            Retry();
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Width = 600;
        }
    }
}
