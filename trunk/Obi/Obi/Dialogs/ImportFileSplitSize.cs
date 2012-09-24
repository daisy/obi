using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using urakawa.media.timing;

namespace Obi.Dialogs
{
    public partial class ImportFileSplitSize : Form
    {
        private uint mMaxPhraseDurationMinutes;
        private bool mCanClose;
        private ObiPresentation mPresentation;  // presentation
        private List<string> m_filePaths;
        private Settings m_Settings;

        public ImportFileSplitSize( Settings settings,string []filesPathArray)
        {
            InitializeComponent();
            m_Settings = settings;
            m_rdbSplitPhrasesOnImport.Checked = settings.SplitPhrasesOnImport;
            m_radiobtnYes.Checked = true;
            mMaxPhraseDurationMinutes = settings.MaxPhraseDurationMinutes;
            m_filePaths = new List<string>(filesPathArray);
            mPhraseSizeTextBox.Text = mMaxPhraseDurationMinutes.ToString();
            foreach (string str in filesPathArray)
            {
                if (str != null)
                {
                    lstManualArrange.Items.Add(System.IO.Path.GetFileName(str));
                }
            }
            mPhraseSizeTextBox.ReadOnly = !settings.SplitPhrasesOnImport;
            m_txtCharToReplaceWithSpace.Text = settings.Audio_ImportCharsToReplaceWithSpaces;
            m_txtPageIdentificationString.Text = settings.Audio_ImportPageIdentificationString;
            m_numCharCountToTruncateFromStart.Value = settings.Audio_ImportCharCountToTruncateFromStart;
            mCanClose = true;
        }

        /// <summary>
        /// When set, the user wants the phrases to be split.
        /// </summary>
        public bool SplitPhrases { get { return m_rdbSplitPhrasesOnImport.Checked; } }

        public bool createSectionForEachPhrase { get { return mCreateAudioFilePerSectionCheckBox.Checked; } }
        public bool SortFileNamesAscending { get { return m_radiobtnYes.Checked; } }
        public string[] FilesPaths 
        { 
            get 
            {
                return m_filePaths.ToArray();
            
        }
       }

        //  create a porpety for section import
        /// <summary>
        /// Maximum durations of imported phrases.
        /// </summary>
        public uint MaxPhraseDurationMinutes { get { return mMaxPhraseDurationMinutes; } }

        public string PageIdentificationString { get { return mCreateAudioFilePerSectionCheckBox.Checked && !string.IsNullOrEmpty(m_txtPageIdentificationString.Text)? m_txtPageIdentificationString.Text: null; } }

        public string CharactersToBeReplacedWithSpaces { get { return mCreateAudioFilePerSectionCheckBox.Checked && !string.IsNullOrEmpty(m_txtCharToReplaceWithSpace.Text)? m_txtCharToReplaceWithSpace.Text: null; } }

        public int CharacterCountToTruncateFromStart { get { return mCreateAudioFilePerSectionCheckBox.Checked? Convert.ToInt32(m_numCharCountToTruncateFromStart.Value) : 0 ; } }

        public bool ApplyPhraseDetection { get { return m_rdbPhraseDetectionOnImportedFiles.Checked; } }

        // Check that the duration is a number.
        private void mOKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (m_rdbSplitPhrasesOnImport.Checked)
                {
                try
                    {
                    uint duration = Convert.ToUInt32 ( mPhraseSizeTextBox.Text );
                    mMaxPhraseDurationMinutes = duration;
                    if (duration <= 0) throw new Exception ();
                    m_Settings.Audio_ImportCharCountToTruncateFromStart = (uint)m_numCharCountToTruncateFromStart.Value;
                    m_Settings.Audio_ImportCharsToReplaceWithSpaces = m_txtCharToReplaceWithSpace.Text;
                    m_Settings.Audio_ImportPageIdentificationString = m_txtPageIdentificationString.Text;
                    }
                catch (System.Exception)
                    {
                    MessageBox.Show ( Localizer.Message ( "max_phrase_duration_invalid_input" ) );
                    mPhraseSizeTextBox.Text = mMaxPhraseDurationMinutes.ToString ();
                    mCanClose = false;
                    }
                }
        }

      
        // Check that we have a valid value before we close, otherwise cancel.
        private void ImportFileSplitSize_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mCanClose)
            {
                mCanClose = true;
                e.Cancel = true;
            }
        }

        // When not splitting, don't edit the text box.
        private void mSplitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mPhraseSizeTextBox.ReadOnly = !m_rdbSplitPhrasesOnImport.Checked;
        }



        private void m_btnMoveUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstManualArrange.Items.Count != 0)
                {
                    if (lstManualArrange.SelectedIndex != 0 && lstManualArrange.SelectedIndex != -1)
                    {
                        
                        object item = lstManualArrange.SelectedItem;
                        
                        int index = lstManualArrange.SelectedIndex;
                       // List<string> filePaths = new List<string>(mfilePaths);
                        //object itemInList = filesPath[index];

                        object itemInList = m_filePaths[index];
                        
                        lstManualArrange.Items.RemoveAt(index);
                        m_filePaths.RemoveAt(index);
                        

                        lstManualArrange.Items.Insert(index - 1, item);
                        m_filePaths.Insert(index - 1, itemInList.ToString());
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void m_btnMoveDown_Click(object sender, EventArgs e)
        {
            try
            {
                int index = lstManualArrange.SelectedIndex;

                if (lstManualArrange.Items.Count != 0)
                {
                    if (lstManualArrange.SelectedIndex != lstManualArrange.Items.Count - 1 && lstManualArrange.SelectedIndex != -1)
                    {
                        object item = lstManualArrange.SelectedItem;
                      
                        object itemInList = m_filePaths[index];
                        lstManualArrange.Items.RemoveAt(index);

                        m_filePaths.RemoveAt(index);

                        lstManualArrange.Items.Insert(index + 1, item);

                        m_filePaths.Insert(index + 1, itemInList.ToString());
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void m_btnAdd_Click(object sender, EventArgs e)
        {

            
            OpenFileDialog select_File = new OpenFileDialog();
            select_File.Filter = "Audio Files (*.wav)|*.wav";
            int index = m_filePaths.Count;
            select_File.RestoreDirectory = true; 
            if (select_File.ShowDialog(this) == DialogResult.OK)
            {
                string filename = System.IO.Path.GetFileName(select_File.ToString());
                lstManualArrange.Items.Add(filename);
                m_filePaths.Add(select_File.FileName.ToString());
              //  ProjectView.ProjectView.publicPaths.Insert(index + 1, select_File.FileName.ToString());

            }
            
        }




    }
}