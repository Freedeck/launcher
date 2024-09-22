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
        int state = 0;
        private void Retry()
        {
            try
            {
                if(!Directory.Exists(textBox1.Text + "\\src\\configs"))
                {
                    // this is first time boot
                    this.Close();
                    return;
                }
                byte[] dl =wc.DownloadData(new Uri(textBox2.Text));
                string[] file = Encoding.Default.GetString(dl).Split('\n');
                string ucfg = File.ReadAllText(textBox1.Text + "\\src\\configs\\config.fd.js");
                string selected = file[0];
                state = 0;
                if (ucfg.Contains("release:\"dev\""))
                {
                    selected = file[1];
                    state = 1;
                }
                string uver = File.ReadAllText(textBox1.Text + "\\package.json");
                string version = uver.Split(new string[] { "\"version\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                bool needsUpdate = false;
                needsUpdate = (version != selected);
                if(!needsUpdate)
                {
                    MessageBox.Show("No update");
                    this.Close();
                } else
                {
                    label1.Text = "Updating! (v" + version + " -> v" + selected + ")";
                    if(selected.Contains("ob7") && version.Contains("ob6"))
                    {
                        Console.WriteLine("Migrating files to OB7 structure");
                        var dir = textBox1.Text;

                        if(!Directory.Exists(dir +"\\user-data"))
                        {
                            Directory.CreateDirectory(dir + "\\user-data");
                        }

                        if(Directory.Exists(dir +"\\src\\public\\sounds"))
                        {
                            Directory.Move(dir + "\\src\\public\\sounds", dir + "\\user-data\\sounds");
                        }

                        if(Directory.Exists(dir +"\\src\\public\\us-icons"))
                        {
                            Directory.Move(dir + "\\src\\public\\us-icons", dir + "\\user-data\\icons");
                        }
                        Console.WriteLine("Migration complete");
                    }
                    RunUpdater();
                    this.Close();
                }
            } catch(Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        private void runProcess(String file, String args)
        {
            using (Process proc = new Process())
            {
                progressBar1.Value = 0;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.WorkingDirectory = textBox1.Text;
                proc.StartInfo.FileName = file;
                proc.StartInfo.Arguments = " " + args;
                proc.Start();
                proc.WaitForExit();
            }
        }
        private void RunUpdater()
        {
            String wantedBranch = "v6";
            if (state == 1) wantedBranch = "v6-dev";
            runProcess("C:\\Program Files\\Git\\bin\\git.exe", "checkout -f " + wantedBranch);
            runProcess("C:\\Program Files\\Git\\bin\\git.exe", "pull");
            runProcess("C:\\Program Files\\nodejs\\npm", "i");
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
