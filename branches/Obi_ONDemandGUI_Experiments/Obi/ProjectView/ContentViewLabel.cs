using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
    {
    public partial class ContentViewLabel : UserControl
        {
        public ContentViewLabel ()
            {
            InitializeComponent ();
            }

        public string Name_SectionDisplayed
            {
            get
                {
                return m_lblSectionName.Text;
                }
            set
                {
                m_lblSectionName.Text = value;
                }
            }
        }
    }
