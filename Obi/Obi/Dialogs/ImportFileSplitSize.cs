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
            m_rdbPhraseDetectionOnImportedFiles.Checked = !settings.SplitPhrasesOnImport;
            mPhraseSizeTextBox.Enabled= m_rdbSplitPhrasesOnImport.Checked;
            
            //m_radiobtnYes.Checked = true;
            mMaxPhraseDurationMinutes = settings.MaxPhraseDurationMinutes;
            m_filePaths = new List<string>(filesPathArray);
            m_filePaths.Sort();
            m_btnMoveUp.Enabled = false;
            m_btnMoveDown.Enabled = false;
            m_btnRemove.Enabled = false;
            mPhraseSizeTextBox.Text = mMaxPhraseDurationMinutes.ToString();
            foreach (string str in m_filePaths)
            {
                if (str != null)
                {
                    lstManualArrange.Items.Add(System.IO.Path.GetFileName(str));
                }
            }

            mPhraseSizeTextBox.ReadOnly = !settings.SplitPhrasesOnImport;
            m_txtCharToReplaceWithSpace.Text = settings.ImportCharsToReplaceWithSpaces;
            m_txtPageIdentificationString.Text = settings.ImportPageIdentificationString;
            m_numCharCountToTruncateFromStart.Value = settings.ImportCharCountToTruncateFromStart;
            mCanClose = true;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Audio/Importing Audio Files.htm");
            mchkCountToTruncateFromStart.Enabled =
                mchkToReplaceWithSpace.Enabled =
                mchktPageIdentificationString.Enabled = mCreateAudioFilePerSectionCheckBox.Checked;

            if (m_Settings.ImportAudioCreateSectionCheck)
            {
                m_txtCharToReplaceWithSpace.Enabled = true;
                m_numCharCountToTruncateFromStart.Enabled = true;
                m_numCharCountToTruncateFromStart.Enabled = true;
                m_txtPageIdentificationString.Enabled = true;

                mCreateAudioFilePerSectionCheckBox.Checked = true;
               
                mchkToReplaceWithSpace.Checked = true;
                mchkCountToTruncateFromStart.Checked = true;
                mchktPageIdentificationString.Checked = true;
            }
        }

        /// <summary>
        /// When set, the user wants the phrases to be split.
        /// </summary>
        public bool SplitPhrases { get { return m_rdbSplitPhrasesOnImport.Checked; } }

        public bool createSectionForEachPhrase { get { return mCreateAudioFilePerSectionCheckBox.Checked; } }
       // public bool SortFileNamesAscending { get { return m_radiobtnYes.Checked; } }
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

        public string PageIdentificationString { get { return mCreateAudioFilePerSectionCheckBox.Checked && mchktPageIdentificationString.Checked && !string.IsNullOrEmpty(m_txtPageIdentificationString.Text)? m_txtPageIdentificationString.Text: null; } }

        public string CharactersToBeReplacedWithSpaces { get { return mCreateAudioFilePerSectionCheckBox.Checked && mchkToReplaceWithSpace.Checked && !string.IsNullOrEmpty(m_txtCharToReplaceWithSpace.Text)? m_txtCharToReplaceWithSpace.Text: null; } }

        public int CharacterCountToTruncateFromStart { get { return mCreateAudioFilePerSectionCheckBox.Checked && mchkCountToTruncateFromStart.Checked? Convert.ToInt32(m_numCharCountToTruncateFromStart.Value) : 0 ; } }

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
                    m_Settings.ImportCharCountToTruncateFromStart = (uint)m_numCharCountToTruncateFromStart.Value;
                    m_Settings.ImportCharsToReplaceWithSpaces = m_txtCharToReplaceWithSpace.Text;
                    m_Settings.ImportPageIdentificationString = m_txtPageIdentificationString.Text;
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
            mPhraseSizeTextBox.Enabled= m_rdbSplitPhrasesOnImport.Checked;
        }



        private void m_btnMoveUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstManualArrange.Items.Count != 0)
                {
                    
                    if (lstManualArrange.SelectedIndex != 0 && lstManualArrange.SelectedIndex != -1)
                    {
                        int tempIndexStore = lstManualArrange.SelectedIndex;  
                        object item = lstManualArrange.SelectedItem;
                        
                        int index = lstManualArrange.SelectedIndex;
                       // List<string> filePaths = new List<string>(mfilePaths);
                        //object itemInList = filesPath[index];

                        object itemInList = m_filePaths[index];
                        
                        lstManualArrange.Items.RemoveAt(index);
                        m_filePaths.RemoveAt(index);
                        

                        lstManualArrange.Items.Insert(index - 1, item);
                        m_filePaths.Insert(index - 1, itemInList.ToString());
                        if((tempIndexStore-1)!=-1)
                        lstManualArrange.SelectedIndex = tempIndexStore-1;

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
                        int tempIndexStore = lstManualArrange.SelectedIndex;  
                        object item = lstManualArrange.SelectedItem;
                      
                        object itemInList = m_filePaths[index];
                        lstManualArrange.Items.RemoveAt(index);

                        m_filePaths.RemoveAt(index);

                        lstManualArrange.Items.Insert(index + 1, item);

                        m_filePaths.Insert(index + 1, itemInList.ToString());
                     //   if ((tempIndexStore+1) != lstManualArrange.Items.Count - 1)
                        lstManualArrange.SelectedIndex = tempIndexStore + 1;
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
            select_File.Filter = Localizer.Message("audio_file_filter");
            int index = m_filePaths.Count;
            select_File.RestoreDirectory = true;
            select_File.Multiselect = true;
            if (select_File.ShowDialog() == DialogResult.OK)
            {
                mOKButton.Enabled = true;
                string[] fileNames = select_File.FileNames;
                foreach (string fileName in fileNames)
                {
                    string nameOfFile = System.IO.Path.GetFileName(fileName);
                    if (nameOfFile != null) lstManualArrange.Items.Add(nameOfFile);
                    m_filePaths.Add(fileName);     
                }
               
                lstManualArrange.SelectedIndex = -1;
                               
            }
            
        }

        private void mCreateAudioFilePerSectionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mchkCountToTruncateFromStart.Enabled =
                mchkToReplaceWithSpace.Enabled =
                mchktPageIdentificationString.Enabled = mCreateAudioFilePerSectionCheckBox.Checked;
            if(mCreateAudioFilePerSectionCheckBox.Checked)
            {
                m_txtCharToReplaceWithSpace.Enabled = true;
                m_numCharCountToTruncateFromStart.Enabled = true;
                m_txtPageIdentificationString.Enabled = true;
                mchkToReplaceWithSpace.Checked = true;
                mchkCountToTruncateFromStart.Checked = true;
                mchktPageIdentificationString.Checked = true;
            }
            else
            {
                m_txtCharToReplaceWithSpace.Enabled = false;
                m_numCharCountToTruncateFromStart.Enabled = false;
                m_txtPageIdentificationString.Enabled = false;
                mchkToReplaceWithSpace.Checked = false;
                mchkCountToTruncateFromStart.Checked = false;
                mchktPageIdentificationString.Checked = false;
            }
        }

        private void m_rdbSplitPhrasesOnImport_CheckedChanged(object sender, EventArgs e)
        {
            if(m_rdbSplitPhrasesOnImport.Checked)
            {
                mPhraseSizeTextBox.Enabled = true;
                mPhraseSizeTextBox.ReadOnly = false;
            }
            else
            {
                mPhraseSizeTextBox.Enabled = false;
                mPhraseSizeTextBox.ReadOnly = true;
            }
        }

        private void mbtnAscendingOrder_Click(object sender, EventArgs e)
        {
            List<string> filenames = new List<string>(); // Contains file names
            Dictionary<String, String> fileNamesDictionary = new Dictionary<string, string>(); //used for storing filename as key and path as value
            List<string> tempDuplicateFileName = new List<string>(); //contains duplicate file names with path
            m_filePaths.Sort();
            foreach (string str in m_filePaths)
            {
                filenames.Add(System.IO.Path.GetFileName(str));
                if (!fileNamesDictionary.ContainsKey(System.IO.Path.GetFileName(str)))
                {
                    fileNamesDictionary.Add(System.IO.Path.GetFileName(str), str);
                }
                else
                {
                    if(!tempDuplicateFileName.Contains(fileNamesDictionary[System.IO.Path.GetFileName(str)]))
                    {
                        tempDuplicateFileName.Add(fileNamesDictionary[System.IO.Path.GetFileName(str)]);
                    }
                    tempDuplicateFileName.Add(str);
                }
            }
            filenames.Sort();
            tempDuplicateFileName.Sort();
            int tempLength = m_filePaths.Count;
            List<string> tempList = new List<string>();
            foreach (string str in filenames)
            {
                if (fileNamesDictionary.ContainsKey(str))
                {
                     tempList.Add(fileNamesDictionary[str]);
                    if (tempDuplicateFileName.Contains(fileNamesDictionary[str]))
                    {
                        int tempIndex = tempDuplicateFileName.IndexOf(fileNamesDictionary[str]);
                        tempDuplicateFileName.RemoveAt(tempIndex);
                        for (int i = tempIndex ; i < tempDuplicateFileName.Count; i++)
                        {
                            if (System.IO.Path.GetFileName(tempDuplicateFileName[i]) == str)
                            {
                                fileNamesDictionary[str] = tempDuplicateFileName[i];
                                break;
                            }
                        }
                     
                    }
                }
            }
            lstManualArrange.Items.Clear();

            if (tempList.Count != 0)
            {
                m_filePaths.Clear();
                m_filePaths = tempList;
            }
            foreach (string str in m_filePaths)
            {
                if (str != null)
                {
                    lstManualArrange.Items.Add(System.IO.Path.GetFileName(str));
                }
            }
            m_btnMoveUp.Enabled = false;
            m_btnMoveDown.Enabled = false;
            m_btnRemove.Enabled = false;
        }

        private void mbtnDesendingOrder_Click(object sender, EventArgs e)
        {

            List<string> filenames = new List<string>(); // Contains file names
            Dictionary<String, String> fileNamesDictionary = new Dictionary<string, string>(); //used for storing filename as key and path as value
            List<string> tempDuplicateFileName = new List<string>(); //contains duplicate file names with path
            m_filePaths.Sort();
            foreach (string str in m_filePaths)
            {
                filenames.Add(System.IO.Path.GetFileName(str));
                if (!fileNamesDictionary.ContainsKey(System.IO.Path.GetFileName(str)))
                {
                    fileNamesDictionary.Add(System.IO.Path.GetFileName(str), str);
                }
                else
                {
                    if (!tempDuplicateFileName.Contains(fileNamesDictionary[System.IO.Path.GetFileName(str)]))
                    {
                        tempDuplicateFileName.Add(fileNamesDictionary[System.IO.Path.GetFileName(str)]);
                    }
                    tempDuplicateFileName.Add(str);
                }
            }
            filenames.Sort();
            tempDuplicateFileName.Sort();

            List<string> tempList = new List<string>();
            foreach (string str in filenames)
            {
                if (fileNamesDictionary.ContainsKey(str))
                {
                    tempList.Add(fileNamesDictionary[str]);
                    if (tempDuplicateFileName.Contains(fileNamesDictionary[str]))
                    {
                        int tempIndex = tempDuplicateFileName.IndexOf(fileNamesDictionary[str]);
                        tempDuplicateFileName.RemoveAt(tempIndex);
                        for (int i = tempIndex; i < tempDuplicateFileName.Count; i++)
                        {
                            if (System.IO.Path.GetFileName(tempDuplicateFileName[i]) == str)
                            {
                                fileNamesDictionary[str] = tempDuplicateFileName[i];
                                break;
                            }
                        }

                    }
                }
            }
            if (tempList.Count != 0)
            {
                m_filePaths.Clear();
                m_filePaths = tempList;
            }
            int totLength = m_filePaths.Count;

            List<string> tempDescending=new List<string>();
            for (int i = totLength-1; i >= 0;i--)
            {
                tempDescending.Add(m_filePaths[i]);
            }

                m_filePaths = tempDescending;

            lstManualArrange.Items.Clear();
            foreach (string str in m_filePaths)
            {
                if (str != null)
                {
                    lstManualArrange.Items.Add(System.IO.Path.GetFileName(str));
                }
            }
            m_btnMoveUp.Enabled = false;
            m_btnMoveDown.Enabled = false;
            m_btnRemove.Enabled = false;

        }

        private void m_btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstManualArrange.Items.Count != 0)
                {
                    if (lstManualArrange.SelectedIndex >= 0 )
                    {
                        object item = lstManualArrange.SelectedItem;
                        int tempIndex = lstManualArrange.SelectedIndex;
                        lstManualArrange.Items.Remove(item);
                        for (int i = 0; i < m_filePaths.Count; i++)
                        {
                            if (System.IO.Path.GetFileName(m_filePaths[i]) == (string)item)
                            {
                                m_filePaths.RemoveAt(i);
                                break;
                            }
                        }
                        if(lstManualArrange.Items.Count!=0)
                        {
                            if(tempIndex>lstManualArrange.Items.Count-1)
                            {
                                lstManualArrange.SelectedIndex = lstManualArrange.Items.Count - 1;
                            }
                            else
                            {
                                lstManualArrange.SelectedIndex = tempIndex;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (lstManualArrange.Items.Count < 1)
            {
                mOKButton.Enabled = false;
                m_btnRemove.Enabled = false;
                m_btnMoveUp.Enabled = false;
                m_btnMoveDown.Enabled = false;
            }
        }

        private void lstManualArrange_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstManualArrange.SelectedIndex==0)
            {
                m_btnRemove.Enabled = true;
                if (lstManualArrange.Items.Count != 1)
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = true;

                }
                else
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = false;

                }
            }
            else if(lstManualArrange.SelectedIndex==lstManualArrange.Items.Count-1)
            {
                m_btnRemove.Enabled = true;
                if (lstManualArrange.Items.Count != 1)
                {
                    m_btnMoveUp.Enabled = true;
                    m_btnMoveDown.Enabled = false;

                }
                else
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = false;

                }
            }
            else if(lstManualArrange.SelectedIndex>0)
            {
                m_btnRemove.Enabled = true;
                if (lstManualArrange.Items.Count != 1)
                {
                    m_btnMoveUp.Enabled = true;
                    m_btnMoveDown.Enabled = true;
                   
                }
                else
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = false;
                   
                }
            }
            else if(lstManualArrange.SelectedIndex<0)
            {
                m_btnMoveUp.Enabled = false;
                m_btnMoveDown.Enabled = false;
                m_btnRemove.Enabled = false;
            }
        }

        private void mchkToReplaceWithSpace_CheckedChanged(object sender, EventArgs e)
        {
            if (mchkToReplaceWithSpace.Checked)
            {
                m_txtCharToReplaceWithSpace.Enabled = true;

            }
            else
            {
                m_txtCharToReplaceWithSpace.Enabled = false;
            }

        }

        private void mchkCountToTruncateFromStart_CheckedChanged(object sender, EventArgs e)
        {
            if (mchkCountToTruncateFromStart.Checked)
            {
                m_numCharCountToTruncateFromStart.Enabled = true;
            }
            else
            {
                m_numCharCountToTruncateFromStart.Enabled = false;
            }
        }

        private void mchktPageIdentificationString_CheckedChanged(object sender, EventArgs e)
        {
            if (mchktPageIdentificationString.Checked)
            {
                m_txtPageIdentificationString.Enabled = true;
            }
            else
            {
                m_txtPageIdentificationString.Enabled = false;
            }
        }

        private void ImportFileSplitSize_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mCreateAudioFilePerSectionCheckBox.Checked)
            {
                m_Settings.ImportAudioCreateSectionCheck = true;
            }
            else
            {
                m_Settings.ImportAudioCreateSectionCheck = false;
            }
        }

 
    }
}