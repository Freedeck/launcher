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
    public partial class AppRunner : Form
    {
        public AppRunner()
        {
            InitializeComponent();
        }

        private void AppRunner_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
        }
    }
}
