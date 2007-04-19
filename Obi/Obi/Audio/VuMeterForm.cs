using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Audio
{
    public partial class VuMeterForm : Form
    {
        public VuMeterForm(VuMeter VuMeterObj )
        {
            
            InitializeComponent();
            graphicalVuMeterPanel.VuMeter = VuMeterObj;
            graphicalVuMeterPanel.ResizeParent = true;
            graphicalVuMeterPanel.ScaleFactor = m_ScaleFactor ;
        }

        private double  m_ScaleFactor =2 ;

        public double MagnificationFactor
        {
            get
            {
                return m_ScaleFactor  ;
            }
            set
            {
                m_ScaleFactor  = value ;
                graphicalVuMeterPanel.ScaleFactor = m_ScaleFactor;
            }
        }

        // override property to show without focus
        private bool ShowWithoutFocusFlag = true;

        protected override bool ShowWithoutActivation
        {
            get
            {
                return ShowWithoutFocusFlag;
            }
        }

        private void VuMeterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            graphicalVuMeterPanel.UnHookEvents();
        }
        

            
        

    }
}