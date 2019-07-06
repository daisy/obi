using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AudioLib;

namespace Obi.Dialogs
{
    public partial class ObiConfiguration : Form
    {
        private static Settings m_Settings;
        private ProjectView.ProjectView m_ProjectView;
        private ObiForm m_Form;
        int m_PredefinedProfilesCount;
        
        public ObiConfiguration()
        {
            InitializeComponent();
        }

        public ObiConfiguration(ObiForm form, ProjectView.ProjectView projectView,Settings settings) : this()
        {
            m_Settings = settings;
            m_ProjectView = projectView;
            m_Form = form;
            this.Text = this.Text +" "+ Application.ProductVersion;
            if (m_Settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(m_Settings.ObiFont, this.Font.Size, FontStyle.Regular); //@fontconfig
            }
            InitializeInputDevices();
            LoadProfilesToComboboxes();
            if (m_cb_SelectProfile.Items.Count > 0)
                m_cb_SelectProfile.SelectedIndex = 0;

            if (m_cb_SelectShortcutsProfile.Items.Count > 0)
                m_cb_SelectShortcutsProfile.SelectedIndex = 0;


            m_tb_ObiConfigInstructions.Text = Localizer.Message("ConfigureObi");

        }

        private void InitializeInputDevices()
        {
            // Initialize Imput Device Combo Box
            AudioRecorder recorder = m_ProjectView.TransportBar.Recorder;
            string defaultInputName = "";
            string defaultOutputName = "";
            m_cb_InputDevice.Items.Clear();
            m_cb_OutPutDevice.Items.Clear();
            foreach (InputDevice input in recorder.InputDevices)
            {
                m_cb_InputDevice.Items.Add(input.Name);
           }
            if(m_cb_InputDevice.Items.Count > 0)
            m_cb_InputDevice.SelectedIndex = 0;

            // Initialize Ouput Device Combo Box
            AudioPlayer player = m_ProjectView.TransportBar.AudioPlayer;

            foreach (OutputDevice output in player.OutputDevices)
            {
                m_cb_OutPutDevice.Items.Add(output.Name);
            }

            if (m_cb_OutPutDevice.Items.Count > 0)
                m_cb_OutPutDevice.SelectedIndex = 0;

        }

        private void LoadProfilesToComboboxes()
        {
            // first load the default profiles
            m_PredefinedProfilesCount = 0;
            string preDefinedProfilesDirectory = GetPredefinedProfilesDirectory();
            if (!System.IO.Directory.Exists(preDefinedProfilesDirectory))
            {
                MessageBox.Show(Localizer.Message("PreferencesPredefined_ProfilesNotExist"), Localizer.Message("PreferencesPredefined_ProfilesNotExistCaption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string[] filePaths = System.IO.Directory.GetFiles(preDefinedProfilesDirectory, "*.xml");
            List<string> filePathsList = new List<string>();
            if (filePaths != null && filePaths.Length > 0)
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    filePathsList.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));
                    //   m_cb_SelectProfile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));
                }
                if (filePathsList.Contains("Basic"))
                {
                    int index = filePathsList.IndexOf("Basic");
                    m_cb_SelectProfile.Items.Add(filePathsList[index]);
                    filePathsList.RemoveAt(index);
                }
                if (filePathsList.Contains("Intermediate"))
                {
                    int index = filePathsList.IndexOf("Intermediate");
                    m_cb_SelectProfile.Items.Add(filePathsList[index]);
                    filePathsList.RemoveAt(index);
                }
                foreach (string file in filePathsList)
                {
                    m_cb_SelectProfile.Items.Add(file);
                }
                
            }

            m_PredefinedProfilesCount = m_cb_SelectProfile.Items.Count;
            // now load user defined profiles from the roming folder, the permanent settings are at same location
            string customProfilesDirectory = GetCustomProfilesDirectory(true);
            if (System.IO.Directory.Exists(customProfilesDirectory))
            {
                filePaths = System.IO.Directory.GetFiles(customProfilesDirectory, "*.xml");
                if (filePaths != null && filePaths.Length > 0)
                {
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        m_cb_SelectProfile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));
                    }
                }
            }
            // now add keyboard shortcuts profile to the respective combobox
            m_cb_SelectShortcutsProfile.Items.Add("Default Shortcuts");
            string preDefinedShortCutProfilesDirectory = GetPredefinedShortCutProfilesDirectory();
            if (System.IO.Directory.Exists(preDefinedShortCutProfilesDirectory))
            {
                filePaths = System.IO.Directory.GetFiles(preDefinedShortCutProfilesDirectory, "*.xml");
                if (filePaths != null && filePaths.Length > 0)
                {
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        m_cb_SelectShortcutsProfile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));

                    }
                }
            }
            string shortcutsProfilesDirectory = GetCustomProfilesDirectory(false);
            if (System.IO.Directory.Exists(shortcutsProfilesDirectory))
            {
                filePaths = System.IO.Directory.GetFiles(shortcutsProfilesDirectory, "*.xml");
                if (filePaths != null && filePaths.Length > 0)
                {
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        m_cb_SelectShortcutsProfile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));

                    }
                }
            }
        }


        private string GetPredefinedProfilesDirectory()
        {
            string appDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string defaultProfilesDirectory = System.IO.Path.Combine(appDirectory, "profiles");
            return defaultProfilesDirectory;
        }

        private string GetPredefinedShortCutProfilesDirectory()
        {
            string appDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string defaultProfilesDirectory = System.IO.Path.Combine(appDirectory, "ProfilesShortcuts");
            return defaultProfilesDirectory;
        }
        private string GetCustomProfilesDirectory(bool preferencesProfile)
        {
            string permanentSettingsDirectory = System.IO.Directory.GetParent(Settings_Permanent.GetSettingFilePath()).ToString();
            string filesDirectory = preferencesProfile ? "profiles" : "keyboard-shortcuts";
            string customProfilesDirectory = System.IO.Path.Combine(permanentSettingsDirectory, filesDirectory);
            return customProfilesDirectory;
        }
        private string GetFilePathOfSelectedPreferencesComboBox()
        {
            string profilePath = null;
            if (m_cb_SelectProfile.SelectedIndex >= 0 && m_cb_SelectProfile.SelectedIndex < m_PredefinedProfilesCount)
            {
                string profileFileName = m_cb_SelectProfile.Items[m_cb_SelectProfile.SelectedIndex].ToString() + ".xml";
                profilePath = System.IO.Path.Combine(GetPredefinedProfilesDirectory(), profileFileName);
            }
            else if (m_cb_SelectProfile.SelectedIndex >= m_PredefinedProfilesCount && m_cb_SelectProfile.SelectedIndex < m_cb_SelectProfile.Items.Count)
            {
                string profileFileName = m_cb_SelectProfile.Items[m_cb_SelectProfile.SelectedIndex].ToString() + ".xml";
                profilePath = System.IO.Path.Combine(GetCustomProfilesDirectory(true), profileFileName);
            }
            return profilePath;
        }

        private string GetPathOfSelectedShortcutsComboBox()
        {
            string shortcutsPath = null;
            if (m_cb_SelectShortcutsProfile.SelectedIndex == 1)
            {
                string shortcutsFileName = m_cb_SelectShortcutsProfile.Items[m_cb_SelectShortcutsProfile.SelectedIndex].ToString() + ".xml";
                shortcutsPath = System.IO.Path.Combine(GetPredefinedShortCutProfilesDirectory(), shortcutsFileName);
                return shortcutsPath;
            }
            else if (m_cb_SelectShortcutsProfile.SelectedIndex != -1)
            {
                string shortcutsFileName = m_cb_SelectShortcutsProfile.Items[m_cb_SelectShortcutsProfile.SelectedIndex].ToString() + ".xml";
                shortcutsPath = System.IO.Path.Combine(GetCustomProfilesDirectory(false), shortcutsFileName);
                return shortcutsPath;
            }
            else
                return string.Empty;
        }

        private void LoadShortcutsFromFile(string filePath)
        {
            if (filePath != null && System.IO.File.Exists(filePath))
            {
                KeyboardShortcuts_Settings shortCuts = KeyboardShortcuts_Settings.GetKeyboardShortcuts_SettingsFromFile(filePath);
                List<string> descriptions = new List<string>();
                descriptions.AddRange(m_Form.KeyboardShortcuts.KeyboardShortcutsDescription.Keys);

                foreach (string desc in descriptions)
                {
                    if (shortCuts.KeyboardShortcutsDescription.ContainsKey(desc))
                    {
                        m_Form.KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value = shortCuts.KeyboardShortcutsDescription[desc].Value;
                    }
                }
                descriptions.Clear();
                descriptions.AddRange(m_Form.KeyboardShortcuts.MenuNameDictionary.Keys);

                foreach (string desc in descriptions)
                {
                    if (shortCuts.MenuNameDictionary.ContainsKey(desc))
                    {
                        m_Form.KeyboardShortcuts.MenuNameDictionary[desc].Value = shortCuts.MenuNameDictionary[desc].Value;
                    }
                }
                m_Form.KeyboardShortcuts.SaveSettings();
                m_Form.InitializeKeyboardShortcuts(false);

                m_Form.KeyboardShortcuts.SettingsName = System.IO.Path.GetFileNameWithoutExtension(filePath);
               
            }
        }
   

        private void LoadProfile()
        {
            // Load Selected preference profile
            string profilePath = GetFilePathOfSelectedPreferencesComboBox();
            try
            {
                if (profilePath != null) LoadPreferenceProfile(profilePath);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            m_ProjectView.TransportBar.ShowSwitchProfileContextMenu();

            //Load Selected Shortcuts profile
            string shortcutsPath = GetPathOfSelectedShortcutsComboBox();
            if (m_cb_SelectShortcutsProfile.SelectedIndex == 0)
            {
                m_Form.LoadDefaultKeyboardShortcuts();
                m_Form.KeyboardShortcuts.SettingsName = System.IO.Path.GetFileNameWithoutExtension(shortcutsPath);
            }
            else if (m_cb_SelectShortcutsProfile.SelectedIndex >= 0 && m_cb_SelectShortcutsProfile.SelectedIndex < m_cb_SelectShortcutsProfile.Items.Count)
            {

                try
                {
                    if (shortcutsPath != null) LoadShortcutsFromFile(shortcutsPath);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
        }

        private void LoadPreferenceProfile(string profilePath)
        {

            if (profilePath != null && System.IO.File.Exists(profilePath))
            {
                Settings saveProfile = Settings.GetSettingsFromSavedProfile(profilePath);
                string profileName = "";
                if (m_cb_SelectProfile.SelectedIndex >= 0 && m_cb_SelectProfile.SelectedIndex < m_cb_SelectProfile.Items.Count)
                {
                    profileName = m_cb_SelectProfile.SelectedItem.ToString();
                }


                    saveProfile.CopyPropertiesToExistingSettings(m_Form.Settings, PreferenceProfiles.All, profileName);
                
               

                m_Settings = m_Form.Settings;

                m_Settings.SettingsNameForManipulation = System.IO.Path.GetFileNameWithoutExtension(profilePath);

            }
        }
        private void m_Ok_Click(object sender, EventArgs e)
        {
            // Set Input Device
            string inputDeviceSelected = m_cb_InputDevice.SelectedItem.ToString();
            m_ProjectView.TransportBar.Recorder.SetInputDevice(inputDeviceSelected);
            m_Settings.Audio_LastInputDevice = inputDeviceSelected;

            //  Set Output Device
            string outputDeviceSelected = m_cb_OutPutDevice.SelectedItem.ToString();
            m_ProjectView.TransportBar.AudioPlayer.SetOutputDevice(m_Form, (outputDeviceSelected));
            m_Settings.Audio_LastOutputDevice = outputDeviceSelected;

            // Load selected profiles
            m_Settings.SettingsName = m_cb_SelectProfile.SelectedItem.ToString();
            LoadProfile();
            m_Settings.IsObiConfigurationDone = true;
        }

      
    }
}
