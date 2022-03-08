using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideTaskbarOnApplications
{
    public partial class ControlPanel : Form
    {
        string[] Programs;
        public ControlPanel(string[] Programs)
        {
            this.Programs = Programs;
            InitializeComponent();
        }

        private void ControlPanel_Load(object sender, EventArgs e)
        {
            foreach (string program in Programs)
            {
                listBox1.Items.Add(program);
            }
        }
    }
}
