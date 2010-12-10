using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class RecordingToolBarForm : Form
    {
        ProjectView.TransportBar m_TransportBar;

        public RecordingToolBarForm()
        {
            InitializeComponent();
        }

        public RecordingToolBarForm(ProjectView.TransportBar transportBar):this  ()
        {
            m_TransportBar = transportBar;
        }

        private void m_recordingToolBarPlayBtn_Click(object sender, EventArgs e)
        {

        }

        private void m_recordingToolBarStopBtn_Click(object sender, EventArgs e)
        {

        }

        private void m_recordingToolBarRecordingBtn_Click(object sender, EventArgs e)
        {

        }

        private void m_recordingToolBarPrePhraseBtn_Click(object sender, EventArgs e)
        {

        }

        private void m_recordingGoToNextPhraseBtn_Click(object sender, EventArgs e)
        {

        }

        private void m_recordingToolBarNextPageBtn_Click(object sender, EventArgs e)
        {

        }

        private void m_recordingToolBarNextSectionBtn_Click(object sender, EventArgs e)
        {

        }

    }
}