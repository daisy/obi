using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class TimeElapsed : Form
    {
        public TimeElapsed(Settings settings, string totalTimeElapsed)
        {
            InitializeComponent();
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
            m_txtBoxTotalTimeElapsed.Text = totalTimeElapsed;
        }

        private void m_btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
