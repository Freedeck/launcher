using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Freedeck_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = "https://raw.githubusercontent.com/Freedeck/Freedeck/v6/assets/logo_big.png";
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
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
            if (checkBox3.Checked) this.Hide();
            Autoupdater autoupdater = new Autoupdater();
            autoupdater.ShowDialog();
            AppRunner appRunner = new AppRunner();
            appRunner.ShowDialog();
            this.Show();
        }
    }
}
