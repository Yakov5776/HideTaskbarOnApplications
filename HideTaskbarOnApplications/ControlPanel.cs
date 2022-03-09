using Newtonsoft.Json.Linq;
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
        bool Changes = false;
        bool CloseHandled = false;
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

        private void button3_Click(object sender, EventArgs e)
        {
            //TODO: implement add process dialog to take input of either textbox or a running process list & Changes = true depending on changed property of subdialog
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int SelectedIndex = listBox1.SelectedIndex;
            if (SelectedIndex == -1) return;

            listBox1.Items.RemoveAt(SelectedIndex);
            Changes = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] newPrograms = listBox1.Items.OfType<string>().ToArray();
            Program.ConfigData["Programs"] = new JArray(newPrograms);
            Program.UpdateConfig(Program.ConfigData);
            CloseHandled = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CloseHandled = true;
            this.Close();
        }

        private void ControlPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Changes && !CloseHandled)
            {
                DialogResult result = MessageBox.Show("You have some unsaved changes, would you like to save changes?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (result)
                {
                    case DialogResult.Yes:
                        button1.PerformClick();
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
        }

        public bool MoveItem(int index, int direction)
        {
            if (index == -1) return false;

            // https://stackoverflow.com/a/9684966/8200011
            // Calculate new index using move direction
            int newIndex = index + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= listBox1.Items.Count)
                return false; // Index out of range - nothing to do

            object selected = listBox1.Items[index];

            // Removing removable element
            listBox1.Items.Remove(selected);
            // Insert it in new position
            listBox1.Items.Insert(newIndex, selected);
            // Restore selection
            listBox1.SetSelected(newIndex, true);

            return true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int SelectedIndex = listBox1.SelectedIndex;
            bool Changed = MoveItem(SelectedIndex, -1);
            if (Changed) Changes = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int SelectedIndex = listBox1.SelectedIndex;
            bool Changed = MoveItem(SelectedIndex, 1);
            if (Changed) Changes = true;
        }
    }
}
