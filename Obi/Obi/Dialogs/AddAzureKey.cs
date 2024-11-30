using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AddAzureKey : Form
    {
        public AddAzureKey()
        {
            InitializeComponent();
        }

        private void m_AddBtn_Click(object sender, EventArgs e)
        {
            if(m_KeyTB.Text.Length == 0 || m_ResgionTB.Text.Length == 0) 
            {
                MessageBox.Show("Please add key and region of your Azure subcription plan");
                return;
            }            
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine(m_KeyTB.Text);
            csvContent.AppendLine(m_ResgionTB.Text);
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            string csvPath = directory + "\\key.csv ";
            File.AppendAllText(csvPath, csvContent.ToString());
            this.Close();
        }
    }
}
