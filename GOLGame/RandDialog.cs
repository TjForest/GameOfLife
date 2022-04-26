using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLGame
{
    public partial class RandDialog : Form
    {
        public RandDialog()
        {
            InitializeComponent();
            numericUpDown1.Value = myInteger;
        }

        public int myInteger 
        {
            get { return (int)numericUpDown1.Value; }
            set { numericUpDown1.Value = value; } 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Random rng = new Random((int)DateTime.Now.Ticks);
            numericUpDown1.Value = rng.Next();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myInteger = (int)numericUpDown1.Value;
        }
    }
}