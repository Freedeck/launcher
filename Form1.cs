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
using System.Collections;
using System.Runtime.InteropServices;

namespace Freedeck_Launcher
{
    public partial class Form1 : Form
    {
        private static string lVersion = "1.5.0";
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            this.KeyDown += Form1_KeyDown;
            this.Shown += Form1_Shown;
            var electronWindow = Process.GetProcesses()
                .Where(p => p.MainWindowTitle.Contains("Freedeck v6 ") || p.MainWindowTitle.Contains("FD Connect"))
                .Select(p => p.MainWindowHandle)
                .FirstOrDefault(hWnd => hWnd != IntPtr.Zero);

            if (electronWindow != IntPtr.Zero)
            {
                running = true;
                checkBox5.Checked = running;
                button1.Text = "Stop Freedeck";
                this.ShowInTaskbar = true;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            textBox1.Text = installPath;
            if (launch) run();
            reloadPaths();
        }

        bool launch = true;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.S && e.Shift)
            {
                launch = false;
            }
        }

        private static string home = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string installPath = home + "\\Freedeck";
        private void GetAndSetVersionData()
        {
            string uver = File.ReadAllText(installPath + "\\freedeck\\package.json");
            string version = uver.Split(new string[] { "\"version\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
            
            label3.Text = "Freedeck v" + version;
            label4.Text = "Launcher v" + lVersion;
        }
        private void reloadPaths()
        {
            if (Directory.Exists(textBox1.Text))
            {
                installPath = textBox1.Text;
                GetAndSetVersionData();
            } else
            {
                label3.Text = "Can\'t find a Freedeck install there.";
                label4.Text = "Launcher v" + lVersion;
            }
        }
        bool running = false;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && running)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(home + "\\Freedeck\\InstallationPath.handoff")) installPath = File.ReadAllText(home+"\\Freedeck\\InstallationPath.handoff");
            label1.BackColor = Color.Transparent;
            label2.BackColor = Color.Transparent;
            checkBox1.BackColor = Color.Transparent;
            checkBox2.BackColor = Color.Transparent;
            label3.BackColor = Color.Transparent;
            label4.BackColor = Color.Transparent;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 advanced = new Form2();
            advanced.ShowDialog();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }
        void run()
        {
            if (running)
            {
                running = false;
                button1.Enabled = false;
                checkBox5.Checked = running;

                button1.Text = "Stopping Freedeck...";
                if (!node.HasExited)
                    node.Kill();
                if (!electron.HasExited)
                    electron.Kill();
                button1.Text = "Start Freedeck";
                button1.Enabled = true;
                return;
            }
            button1.Enabled = false;
            button1.Text = "Checking for updates..";
            progressBar1.Value = 10;
            if (!checkBox2.Checked)
            {
                Autoupdater autoupdater = new Autoupdater();
                progressBar1.Value = 20;
                autoupdater.ShowDialog();
            }
            button1.Text = "Starting Server... [1/2]";
            startProgram("--server-only");
            progressBar1.Value = 30;
            button1.Text = "Starting Companion... [2/2]";
            startProgram("--companion-only", true);
            progressBar1.Value = 100;
            button1.Text = "Stop Freedeck";
            button1.Enabled = true;
            running = true;
            checkBox5.Checked = running;

            this.Hide();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            run();
            this.MinimizeBox = true;
            this.ShowInTaskbar = true;
        }
        Process electron;
        Process node;
        private void startProgram(string args, bool useElectron = false)
        {
            Process proc = new Process();

            if (useElectron)
            {
                electron = proc;
                checkBox3.Checked = true;
            }
            else
            {
                node = proc;
                checkBox4.Checked = true;
            }
            proc.StartInfo.WindowStyle = checkBox1.Checked || useElectron ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
            proc.StartInfo.Arguments = " src\\index.js " + args;
            proc.StartInfo.WorkingDirectory = installPath + "\\freedeck";
            
            if (useElectron)
                proc.StartInfo.FileName = installPath+"\\freedeck\\node_modules\\electron\\dist\\electron.exe";
            else 
                proc.StartInfo.FileName = "C:\\Program Files\\nodejs\\node.exe";

            proc.Start();
            proc.EnableRaisingEvents = true;

            if (useElectron)
                proc.Exited += new EventHandler(Proc_Exited);
            else
                proc.Exited += new EventHandler(Proc_nodeexit);    
        }

        private void Proc_nodeexit(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(installPath + "\\freedeck\\src\\configs\\config.fd.js"))
                {
                    try
                    {
                        electron.Kill();
                    } catch(Exception errr)
                    {
                        Console.WriteLine(errr.ToString());
                    }
                    this.Invoke(new MethodInvoker(delegate {
                        checkBox3.Checked = false;
                        running = false;
                        checkBox5.Checked = running;
                    }));
                    this.Invoke(new MethodInvoker(delegate
                    {
                        button1.Text = "Start Freedeck";
                        this.Show();
                        this.BringToFront();
                        this.Focus();
                    }));
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            try
            {
                node.Kill();
                this.Invoke(new MethodInvoker(delegate
                {
                    checkBox4.Checked = false;
                }));
                running = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            this.Invoke(new MethodInvoker(delegate
            {
                button1.Text = "Server stopped";
                running = false;
                this.Show();
                this.BringToFront();
                this.Focus();
            }));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            reloadPaths();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
