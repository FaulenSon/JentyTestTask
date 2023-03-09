using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class SetFieldSizeForm : Form
    {
        private MainForm MainForm { get; set; }

        public SetFieldSizeForm()
        {
            InitializeComponent();
        }

        public SetFieldSizeForm(MainForm mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
        }

        public void SetMainFormFieldSize()
        {
            MainForm.SetFieldSize(new Size((int)numericWidth.Value, (int)numericHeight.Value));
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            SetMainFormFieldSize();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
