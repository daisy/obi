using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class ContainerStripUserControl : Protobi.StripUserControl
    {
        public ContainerStripUserControl()
        {
            InitializeComponent();
            InitializeMembers();
        }

        public ContainerStripUserControl(ContainerStripController controller)
        {
            InitializeComponent();
            InitializeMembers(controller, true);
        }
    }
}

