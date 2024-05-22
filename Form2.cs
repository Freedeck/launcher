using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Freedeck_Launcher
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // debug
            if (checkBox1.Checked)
            {
                fullDisable(checkBox3);
            } else
            {
                if(!checkBox2.Checked) checkBox3.Enabled = true;
            }
            onFinishedCheck();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // server only
            if(checkBox2.Checked)
            {
                fullDisable(checkBox3);
                fullDisable(checkBox6);
            } else
            {
                if(!checkBox1.Checked) checkBox3.Enabled = true;
                checkBox6.Enabled = true;
            }
            onFinishedCheck();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            // companion only
            if(checkBox3.Checked)
            {
                fullDisable(checkBox2);
                fullDisable(checkBox1);
                fullDisable(checkBox6);
            }
            else
            {
                checkBox2.Enabled = true;
                checkBox1.Enabled = true;
                checkBox6.Enabled = true;
            }
            onFinishedCheck();
        }

        private void onFinishedCheck()
        {
            textBox1.Text = "";
            if (checkBox4.Checked) textBox1.Text += "--no-update ";
            if (checkBox1.Checked) textBox1.Text += "--debug ";
            if (checkBox2.Checked) textBox1.Text += "--server-only ";
            if (checkBox3.Checked) textBox1.Text += "--companion-only ";
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            onFinishedCheck();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox5.Checked) this.Hide();
            button1.Enabled = false;
            //richTextBox1.Text = "";
            //richTextBox2.Text = "";
            if(!File.Exists(textBox2.Text+"\\freedeck\\node_modules\\electron\\dist\\electron.exe"))
            {
                richTextBox1.Text += "Could not find Electron. Halting!";
                richTextBox2.Text += "Could not find Electron. Halting!";
                button1.Enabled = true;
                this.Show();
                return;
            }
            if(!Directory.Exists(textBox2.Text+"\\freedeck"))
            {
                if(!Directory.Exists(textBox2.Text))
                {
                    richTextBox1.Text += "Could not find " + textBox2.Text+"\n";
                    richTextBox2.Text += "Could not find " + textBox2.Text+"\n";
                }
                richTextBox1.Text += "Could not find folder 'freedeck' in " + textBox2.Text+"\n";
                richTextBox2.Text += "Could not find folder 'freedeck' in " + textBox2.Text+"\n";
                button1.Enabled = true;
                this.Show();
                return;
            }
            string msg = "Loading Freedeck from folder " + textBox2.Text + "\\freedeck with arguments " + textBox1.Text + "\n";
            if(!checkBox2.Checked && !checkBox3.Checked) {
                richTextBox1.Text = "";
                richTextBox1.Text += msg;
                startProgram(textBox1.Text, -1); 
            }
            else if (checkBox2.Checked && !checkBox3.Checked)
            {
                richTextBox1.Text = "";
                richTextBox1.Text += msg;
                startProgram(textBox1.Text, 0);
            }
            else if (!checkBox2.Checked && checkBox3.Checked)
            {
                richTextBox2.Text = "";
                richTextBox2.Text += msg;
                startProgram(textBox1.Text, 1);
            }
            button1.Enabled = true;
            this.Show();
        }
        private void startProgram(string args, int windowId)
        {
            using(Process proc = new Process())
            {
                proc.StartInfo.WindowStyle = checkBox7.Checked ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
                proc.StartInfo.RedirectStandardOutput = true;
                if (windowId == -1) proc.OutputDataReceived += Proc_OutputDataReceivedDefault;
                if (windowId == 0) proc.OutputDataReceived += Proc_OutputDataReceivedServer;
                if (windowId == 1) proc.OutputDataReceived += Proc_OutputDataReceivedCompanion;
                proc.StartInfo.Arguments = " src\\index.js " + args;
                proc.StartInfo.WorkingDirectory = textBox2.Text+"\\freedeck";
                proc.StartInfo.FileName = textBox3.Text;
                proc.StartInfo.UseShellExecute = false;

                proc.Start();
                proc.BeginOutputReadLine();
                
            }
        }

        private void Proc_OutputDataReceivedServer(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e);
            Trace.WriteLine(e.Data);
            this.BeginInvoke(new MethodInvoker(() =>
            {
                richTextBox1.AppendText(e.Data ?? string.Empty);
            }));
        }
        private void Proc_OutputDataReceivedCompanion(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e);
            Trace.WriteLine(e.Data);
            this.BeginInvoke(new MethodInvoker(() =>
            {
                richTextBox2.AppendText(e.Data ?? string.Empty);
            }));
        }

        private void Proc_OutputDataReceivedDefault(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e);
            Trace.WriteLine(e.Data);
            this.BeginInvoke(new MethodInvoker(() =>
            {
                richTextBox1.AppendText(e.Data ?? "\n");
            }));
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            textBox2.Text = home+"\\Freedeck";
            onFinishedCheck();
            textBox3.Text = textBox2.Text + "\\freedeck\\node_modules\\electron\\dist\\electron.exe";
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox6.Checked)
            {
                fullDisable(checkBox2);
                fullDisable(checkBox3);
            } else
            {
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
            }
            onFinishedCheck();
        }

        private void fullDisable(CheckBox checkbox)
        {
            checkbox.Enabled = false;
            checkbox.Checked = false;
        }
    }
}
