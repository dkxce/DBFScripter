using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DBFScripter
{
    public partial class SelectScript : Form
    {
        public string currentFile = "";

        public SelectScript()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile = DBFScripter.GetCurrentDir() + @"\Scripts\" + listBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = DBFScripter.GetCurrentDir();
            ofd.Title = "Выберите скрипт...";
            ofd.DefaultExt = "*.csm";
            ofd.Filter = "Файлы скриптов C# (*.csm;*.cs)|*.csm;*.cs";
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            currentFile = ofd.FileName;
            ofd.Dispose();
            DialogResult = DialogResult.OK;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.Text != "")
                DialogResult = DialogResult.OK;
        }

        private void SelectScript_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
                       
        }
    }
}