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
    public partial class SettingDialog : Form
    {
        public SettingDialog()
        {
            InitializeComponent();
            IntervalUpDown.Value = Interval;
            WidthUpDown.Value = settWidth;
            HeightUpDown.Value = settHeight;
        }

        public int Interval
        {
            get { return (int)IntervalUpDown.Value; }
            set { IntervalUpDown.Value = value; }
        }
        public int settWidth
        {
            get { return (int)WidthUpDown.Value; }
            set { WidthUpDown.Value = value; }
        }
        public int settHeight
        {
            get { return (int)WidthUpDown.Value; }
            set { WidthUpDown.Value = value; }
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            Interval = (int)IntervalUpDown.Value;
            settWidth = (int)WidthUpDown.Value;
            settHeight = (int)HeightUpDown.Value;
        }
    }
}
