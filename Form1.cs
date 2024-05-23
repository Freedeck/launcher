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
        private static string imgPath = installPath + "\\freedeck\\assets\\taskbar.png";
        private static Image img = Image.FromFile(imgPath);
        private void reloadPaths()
        {
            installPath = textBox1.Text;
            if(!File.Exists(installPath +"\\freedeck\\assets\\bg.jpg"))
            {
                imgPath = installPath + "\\freedeck\\assets\\taskbar.png";
            } else
            {
                imgPath = installPath + "\\freedeck\\assets\\bg.jpg";
            }
            img = Image.FromFile(imgPath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(home + "\\Freedeck\\InstallationPath.handoff")) installPath = File.ReadAllText(home+"\\Freedeck\\InstallationPath.handoff");
            textBox1.Text = installPath;
            this.BackgroundImage = img;
            reloadPaths();
            this.BackgroundImageLayout = ImageLayout.Tile;
            label1.BackColor = Color.Transparent;
            label2.BackColor = Color.Transparent;
            checkBox1.BackColor = Color.Transparent;
            checkBox2.BackColor = Color.Transparent;
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
        Waiting waiting = new Waiting();

        private void button1_Click(object sender, EventArgs e)
        {
            if(!checkBox2.Checked)
            {
                Autoupdater autoupdater = new Autoupdater();
                autoupdater.ShowDialog();
            }
            waiting.Show();
            startProgram("--server-only");
            startProgram("--companion-only", true);
            this.Hide();
        }
        Process electron;
        Process node;
        private void startProgram(string args, bool useElectron = false)
        {

            Process proc = new Process();
            if (useElectron) electron = proc;
            else node = proc;

                proc.StartInfo.WindowStyle = checkBox1.Checked || useElectron ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
                proc.StartInfo.Arguments = " src\\index.js " + args;
                proc.StartInfo.WorkingDirectory = installPath + "\\freedeck";
                if(useElectron)proc.StartInfo.FileName = installPath+"\\freedeck\\node_modules\\electron\\dist\\electron.exe";
                else proc.StartInfo.FileName = "C:\\Program Files\\nodejs\\node.exe";
                proc.Start();
                proc.EnableRaisingEvents = true;
            if (useElectron) proc.Exited += new EventHandler(Proc_Exited);
            else proc.Exited += new EventHandler(Proc_nodeexit);    
            this.Invoke(new MethodInvoker(delegate
                {
                    waiting.Close();
                }));
        }

        private void Proc_nodeexit(object sender, EventArgs e)
        {
            try
            {
                electron.Kill();
            } catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            this.Invoke(new MethodInvoker(delegate
            {
                this.Show();
            }));
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            try
            {
                node.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            reloadPaths();
        }
    }
}
