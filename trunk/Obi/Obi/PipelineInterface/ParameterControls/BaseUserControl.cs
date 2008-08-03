using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.PipelineInterface.ParameterControls
{
    public partial class BaseUserControl : UserControl
    {
        public BaseUserControl()
        {
            InitializeComponent();
        }

        public virtual String Value
        {
            get
            {
                return BaseTextBox.Text;
            }
            set
            {
                                BaseTextBox.Text = value;
                                            }
        }

        public virtual void UpdateScriptParameterValue()
        {

        }



    }
}
