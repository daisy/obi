using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Obi
{
    /// <summary>
    /// Persistent application settings.
    /// </summary>
    /// <remarks>It also seems that making a change in the class resets the existing settings.</remarks>
    [Serializable()]
    public class KeyboardShortcuts_Settings
    {
        // follow following steps to add new content view / project view keyboard shortcut
        // 1. Create a member variable of the shortcut as done below
        // 2. Add it to the descriptions dictionnary
        // 3. Add the localizer message with the description of keyboard shortcut object as the key
        public KeyboardShortcut ContentView_SelectCompleteWaveform = new KeyboardShortcut(Keys.A, "KeyS_SelectCompleteWaveform");
        public KeyboardShortcut ContentView_PlaySelectedWaveform = new KeyboardShortcut(Keys.C, "KeyS_PlaySelectedWaveform");
        public KeyboardShortcut ContentView_TransportBarNextSection = new KeyboardShortcut(Keys.H, "KeyS_TransportBarNextSection");
        public KeyboardShortcut ContentView_TransportBarPreviousSection = new KeyboardShortcut(Keys.Shift | Keys.H, "KeyS_TransportBarPreviousSection");
        public KeyboardShortcut ContentView_TransportBarPreviousPhrase = new KeyboardShortcut(Keys.J, "KeyS_TransportBarPreviousPhrase");
        public KeyboardShortcut ContentView_TransportBarNextPhrase = new KeyboardShortcut(Keys.K, "KeyS_TransportBarNextPhrase");
        public KeyboardShortcut ContentView_TransportBarNudgeForward = new KeyboardShortcut(Keys.N, "KeyS_TransportBarNudgeForward");
        public KeyboardShortcut ContentView_TransportBarNudgeBackward = new KeyboardShortcut(Keys.Shift | Keys.N, "KeyS_TransportBarNudgeBackward");
        public KeyboardShortcut ContentView_TransportBarFineNavigationOn = new KeyboardShortcut(Keys.F2, "KeyS_TransportBarFineNavigationOn");
        public KeyboardShortcut ContentView_TransportBarFineNavigationOff = new KeyboardShortcut(Keys.Shift | Keys.F2, "KeyS_TransportBarFineNavigationOff");
        public KeyboardShortcut ContentView_TransportBarRecordSingleKey = new KeyboardShortcut(Keys.R, "KeyS_TransportBarRecordSingleKey");
        public KeyboardShortcut ContentView_TransportBarStopSingleKey = new KeyboardShortcut(Keys.T, "KeyS_TransportBarStopSingleKey");
        public KeyboardShortcut ContentView_MarkSelectionBeginTime = new KeyboardShortcut(Keys.OemOpenBrackets, "KeyS_MarkSelectionBeginTime");
        public KeyboardShortcut ContentView_MarkSelectionEndTime = new KeyboardShortcut(Keys.OemCloseBrackets, "KeyS_MarkSelectionEndTime");
        public KeyboardShortcut ContentView_ExpandAudioSelectionAtLeft = new KeyboardShortcut(Keys.Shift | Keys.Left, "KeyS_ExpandAudioSelectionAtLeft");
        public KeyboardShortcut ContentView_ContractAudioSelectionAtLeft = new KeyboardShortcut(Keys.Shift | Keys.Right, "KeyS_ContractAudioSelectionAtLeft");
        public KeyboardShortcut ContentView_TransportBarNextPage = new KeyboardShortcut(Keys.P, "KeyS_TransportBarNextPage");
        public KeyboardShortcut ContentView_TransportBarPreviousPage = new KeyboardShortcut(Keys.Shift | Keys.P, "KeyS_TransportBarPreviousPage");
        public KeyboardShortcut ContentView_TransportBarPreviewFromAudioCursor = new KeyboardShortcut(Keys.V, "KeyS_TransportBarPreviewFromAudioCursor");
        public KeyboardShortcut ContentView_TransportBarPreviewFromSelection = new KeyboardShortcut(Keys.Shift | Keys.V, "KeyS_TransportBarPreviewFromSelection");
        public KeyboardShortcut ContentView_TransportBarPreviewUptoAudioCursor = new KeyboardShortcut(Keys.X, "KeyS_TransportBarPreviewUptoAudioCursor");
        public KeyboardShortcut ContentView_TransportBarPreviewUptoSelection = new KeyboardShortcut(Keys.Shift | Keys.X, "KeyS_TransportBarPreviewUptoSelection");
        public KeyboardShortcut ContentView_TransportBarExpandPlayOptions = new KeyboardShortcut(Keys.Control | Keys.F7, "KeysS_TransportBarExpandPlayOptions");
        public KeyboardShortcut ContentView_TransportBarExpandRecordOptions = new KeyboardShortcut(Keys.Control | Keys.F8, "KeysS_TransportBarExpandRecordOptions");
        // Obi 3.8 beta
        [OptionalField]
        public KeyboardShortcut ContentView_TransportBarExpandSwitchProfile = new KeyboardShortcut(Keys.Control | Keys.F5, "KeysS_TransportBarExpandSwitchProfile");

        public KeyboardShortcut ContentView_ZoomWaveformPanel = new KeyboardShortcut(Keys.Z, "KeyS_ZoomWaveformPanel");

        //Zoom Paenl Shortcuts        
        public KeyboardShortcut ZoomPanel_Close = new KeyboardShortcut(Keys.Alt | Keys.C, "KeyS_CloseZoomPanel");
        public KeyboardShortcut ZoomPanel_Reset = new KeyboardShortcut(Keys.Alt | Keys.D0, "KeyS_ResetZoomPanel");
        public KeyboardShortcut ZoomPanel_ZoomIn = new KeyboardShortcut(Keys.Alt | Keys.Oemplus, "KeyS_ZoomInWaveform");
        public KeyboardShortcut ZoomPanel_ZoomOut = new KeyboardShortcut(Keys.Alt | Keys.OemMinus, "KeyS_ZoomOutWaveform");
        public KeyboardShortcut ZoomPanel_ZoomSelection = new KeyboardShortcut(Keys.Alt | Keys.S, "KeyS_ZoomSelectedWaveform");
        public KeyboardShortcut ZoomPanel_NextPhrase = new KeyboardShortcut(Keys.Alt | Keys.Right, "KeyS_NextPhrase");
        public KeyboardShortcut ZoomPanel_PreviousPhrase = new KeyboardShortcut(Keys.Alt | Keys.Left, "KeyS_PreviousPhrase");

        // playback shortcuts.

        public KeyboardShortcut ContentView_FastPlayStepDown = new KeyboardShortcut(Keys.S, "KeyS_FastPlayStepDown");
        public KeyboardShortcut ContentView_FastPlayStepUp = new KeyboardShortcut( Keys.F, "KeyS_FastPlayStepUp");
        public KeyboardShortcut ContentView_FastPlayRateNormilize = new KeyboardShortcut(Keys.D, "KeyS_FastPlayRateNormilize");
        public KeyboardShortcut ContentView_FastPlayNormalizeWithElapseBack = new KeyboardShortcut(Keys.E, "KeyS_FastPlayNormalizeWithElapseBack");
        public KeyboardShortcut ContentView_FastPlayWithElapseForward = new KeyboardShortcut(Keys.Y, "KeyS_FastPlayWithElapseForward");
        public KeyboardShortcut ContentView_MarkSelectionFromBeginningToTheCursor = new KeyboardShortcut(Keys.Shift | Keys.Home, "KeyS_MarkSelectionFromBeginningToTheCursor");
        public KeyboardShortcut ContentView_MarkSelectionFromCursorToTheEnd = new KeyboardShortcut(Keys.Shift | Keys.End, "KeyS_MarkSelectionFromCursorToTheEnd");


        // Strips navigation
        public KeyboardShortcut ContentView_SelectPrecedingPhrase = new KeyboardShortcut(Keys.Left, "KeyS_SelectPrecedingPhrase");
        public KeyboardShortcut ContentView_SelectFollowingPhrase = new KeyboardShortcut(Keys.Right, "KeyS_SelectFollowingPhrase");
        public KeyboardShortcut ContentView_SelectLastPhraseInStrip = new KeyboardShortcut(Keys.End, "KeyS_SelectLastPhraseInStrip");
        public KeyboardShortcut ContentView_SelectFirstPhraseInStrip = new KeyboardShortcut(Keys.Home, "KeyS_SelectFirstPhraseInStrip");
        public KeyboardShortcut ContentView_SelectNextPagePhrase = new KeyboardShortcut(Keys.Control | Keys.PageDown, "KeyS_SelectNextPagePhrase");
        public KeyboardShortcut ContentView_SelectPrecedingPagePhrase = new KeyboardShortcut(Keys.Control | Keys.PageUp, "KeyS_SelectPrecedingPagePhrase");
        public KeyboardShortcut ContentView_SelectNextSpecialRolePhrase = new KeyboardShortcut(Keys.F4, "KeyS_SelectNextSpecialRolePhrase");
        public KeyboardShortcut ContentView_SelectPrecedingSpecialRolePhrase = new KeyboardShortcut(Keys.Shift | Keys.F4, "KeyS_SelectPrecedingSpecialRolePhrase");
        public KeyboardShortcut ContentView_SelectNextEmptyPhrase = new KeyboardShortcut(Keys.Control | Keys.Alt | Keys.F4, "KeyS_SelectNextEmptyPhrase");
        public KeyboardShortcut ContentView_SelectFirstSkippableNode = new KeyboardShortcut(Keys.Alt | Keys.Home, "KeyS_SelectFirstSkippableNode");
        public KeyboardShortcut ContentView_SelectLastSkippableNode = new KeyboardShortcut(Keys.Alt | Keys.End, "KeyS_SelectLastSkippableNode");
        //public KeyboardShortcut ContentView_SelectPrecedingStrip = Keys.Control | Keys.Up;
        //public KeyboardShortcut ContentView_SelectFollowingStrip =  Keys.Control | Keys.Down;
        public KeyboardShortcut ContentView_SelectPrecedingStrip = new KeyboardShortcut(Keys.Control | Keys.Shift | Keys.Up, "KeyS_SelectPrecedingStrip");
        public KeyboardShortcut ContentView_SelectFollowingStrip = new KeyboardShortcut(Keys.Control | Keys.Shift | Keys.Down, "KeyS_SelectFollowingStrip");
        public KeyboardShortcut ContentView_SelectFirstStrip = new KeyboardShortcut(Keys.Control | Keys.Home, "KeyS_SelectFirstStrip");
        public KeyboardShortcut ContentView_SelectLastStrip = new KeyboardShortcut(Keys.Control | Keys.End, "KeyS_SelectLastStrip");

        public KeyboardShortcut ContentView_SelectUp = new KeyboardShortcut(Keys.Escape, "KeyS_SelectUp");

        // Control + arrows moves the strip cursor
        public KeyboardShortcut ContentView_SelectPrecedingStripCursor = new KeyboardShortcut(Keys.Control | Keys.Left, "KeyS_SelectPrecedingStripCursor");
        public KeyboardShortcut ContentView_SelectFollowingStripCursor = new KeyboardShortcut(Keys.Control | Keys.Right, "KeyS_SelectFollowingStripCursor");

        public KeyboardShortcut ContentView_ScrollDown_LargeIncrementWithSelection = new KeyboardShortcut(Keys.PageDown, "KeyS_ScrollDown_LargeIncrementWithSelection");
        public KeyboardShortcut ContentView_ScrollUp_LargeIncrementWithSelection = new KeyboardShortcut(Keys.PageUp, "KeyS_ScrollUp_LargeIncrementWithSelection");
        public KeyboardShortcut ContentView_ScrollDown_SmallIncrementWithSelection = new KeyboardShortcut(Keys.Down, "KeyS_ScrollDown_SmallIncrementWithSelection");
        public KeyboardShortcut ContentView_ScrollUp_SmallIncrementWithSelection = new KeyboardShortcut(Keys.Up, "KeyS_ScrollUp_SmallIncrementWithSelection");

        // Obi 3.8 beta
        [OptionalField]
        public KeyboardShortcut ContentView_SelectStartOfThePhrase = new KeyboardShortcut(Keys.Home | Keys.Alt | Keys.Control, "KeyS_SelectStartOfThePhrase");

        // Obi 3.8 beta
        [OptionalField]
        public KeyboardShortcut ContentView_SelectEndOfPhrase = new KeyboardShortcut(Keys.End | Keys.Alt | Keys.Control, "KeyS_SelectEndOfPhrase");

        //project view shortcuts
        public KeyboardShortcut ProjectView_MoveToNextViewClockwise = new KeyboardShortcut(Keys.Control | Keys.Tab, "KeyS_MoveToNextViewClockwise");
        public KeyboardShortcut ProjectView_MoveToPreviousViewAnticlockwise = new KeyboardShortcut(Keys.Control | Keys.Shift | Keys.Tab, "KeyS_MoveToPreviousViewAnticlockwise");
        public KeyboardShortcut ProjectView_ToggleBetweenContentViewAndTOCView = new KeyboardShortcut(Keys.F6, "KeyS_ToggleBetweenContentViewAndTOCView");
        public KeyboardShortcut ProjectView_PlayPauseUsingSelection = new KeyboardShortcut(Keys.Shift | Keys.Space, "KeyS_PlayPauseUsingSelection");
        public KeyboardShortcut ProjectView_PlayPauseUsingAudioCursor_Default = new KeyboardShortcut(Keys.Space, "KeyS_PlayPauseUsingAudioCursor_Default");
        public KeyboardShortcut ProjectView_ShowPropertiesOfSelectedNode = new KeyboardShortcut(Keys.Alt | Keys.Enter, "KeyS_ShowPropertiesOfSelectedNode");
        public KeyboardShortcut ProjectView_FocusOnTransportBarTimeDisplay = new KeyboardShortcut(Keys.F8, "KeyS_FocusOnTransportBarTimeDisplay");
        public KeyboardShortcut ProjectView_HardResetAllSettings = new KeyboardShortcut(Keys.Alt | Keys.Control | Keys.F10, "KeyS_HardResetAllSettings");
        [OptionalField]
        public KeyboardShortcut ProjectView_PlayOnNavigate = new KeyboardShortcut(Keys.Control | Keys.U, "KeyS_PlayOnNavigate");

        public KeyboardShortcut[] MenuKeyboardShortCutsList;

        [OptionalField]
        public string SettingsName = "";

        private static readonly string SETTINGS_FILE_NAME = "obi_KeyboardShortcuts_Settings.xml";

        [NonSerialized()]
        public Dictionary<string, KeyboardShortcut> KeyboardShortcutsDescription = new Dictionary<string, KeyboardShortcut>();

        [NonSerialized()]
        public Dictionary<string, KeyboardShortcut> MenuNameDictionary;

        [NonSerialized()]
        public static Dictionary<string, KeyboardShortcut> MenuNameDefaultShortcutDictionary = new Dictionary<string,KeyboardShortcut> ();

        [NonSerialized()]
        private Dictionary<Keys, string> UserFriendlyNameDirectory;

        /// <summary>
        /// Read the settings from the settings file; missing values are replaced with defaults.
        /// </summary>
        /// <remarks>Errors are silently ignored and default settings are returned.</remarks>
        public static KeyboardShortcuts_Settings GetKeyboardShortcuts_Settings()
        {
            KeyboardShortcuts_Settings settings = new KeyboardShortcuts_Settings();
            
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                                IsolatedStorageFileStream stream =
                    new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Open, FileAccess.Read, file);
                                SoapFormatter soap = new SoapFormatter();
                settings = (KeyboardShortcuts_Settings)soap.Deserialize(stream);
                                stream.Close();
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Keyboard shortcut setting  file : " + ex.ToString());
                if (ex is System.IO.FileNotFoundException) return GetDefaultKeyboardShortcuts_Settings();
            }

            settings.MenuNameDictionary = new Dictionary<string, KeyboardShortcut>();
            for (int i = 0; i < settings.MenuKeyboardShortCutsList.Length; i++)
            {
                settings.MenuNameDictionary.Add(settings.MenuKeyboardShortCutsList[i].Description, settings.MenuKeyboardShortCutsList[i]);
            }
            settings.KeyboardShortcutsDescription = new Dictionary<string, KeyboardShortcut>();
            
            settings.PopulateKeyboardShortcutsDictionary();
            
            
            return settings;
        }

        public static KeyboardShortcuts_Settings GetKeyboardShortcuts_SettingsFromFile(string filePath)
        {
            KeyboardShortcuts_Settings settings = new KeyboardShortcuts_Settings();
            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.OpenOrCreate);

                SoapFormatter soap = new SoapFormatter();
                settings = (KeyboardShortcuts_Settings)soap.Deserialize(stream);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Keyboard shortcut setting  file : " + ex.ToString());
                if (ex is System.IO.FileNotFoundException) return GetDefaultKeyboardShortcuts_Settings();
            }
            finally
            {
                if(stream != null)  stream.Close();
            }
            settings.MenuNameDictionary = new Dictionary<string, KeyboardShortcut>();
            for (int i = 0; i < settings.MenuKeyboardShortCutsList.Length; i++)
            {
                settings.MenuNameDictionary.Add(settings.MenuKeyboardShortCutsList[i].Description, settings.MenuKeyboardShortCutsList[i]);
            }
            settings.KeyboardShortcutsDescription = new Dictionary<string, KeyboardShortcut>();

            settings.PopulateKeyboardShortcutsDictionary();

            return settings;
        }

        public static KeyboardShortcuts_Settings GetDefaultKeyboardShortcuts_Settings()
        {
            KeyboardShortcuts_Settings settings = new KeyboardShortcuts_Settings();

            if (MenuNameDefaultShortcutDictionary != null && MenuNameDefaultShortcutDictionary.Count > 0 )
            {
                settings.MenuNameDictionary = new Dictionary<string, KeyboardShortcut>();
                foreach (string name in MenuNameDefaultShortcutDictionary.Keys) settings.MenuNameDictionary.Add(name, MenuNameDefaultShortcutDictionary[name].Copy ());
                    
            }
            else
            {
                settings.MenuNameDictionary = new Dictionary<string, KeyboardShortcut>();
            }
            
            if (settings.KeyboardShortcutsDescription != null) settings.KeyboardShortcutsDescription.Clear();
            settings.KeyboardShortcutsDescription = new Dictionary<string, KeyboardShortcut>();
            settings.UserFriendlyNameDirectory = new Dictionary<Keys, string>();
            settings.PopulateKeyboardShortcutsDictionary();
            settings.PopulateUserFriendlyNamesForKeyboardKeys();
            settings.SettingsName = "Default";

            return settings;
        }


        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        public void SaveSettings()
        {
            AddMenuShortcutsToArrayForSave();
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            IsolatedStorageFileStream stream =
                new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Create, FileAccess.Write, file);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }

        public void SaveSettingsAs(string filePath)
        {
            AddMenuShortcutsToArrayForSave();
            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.OpenOrCreate);
                SoapFormatter soap = new SoapFormatter();
                soap.Serialize(stream, this);
            }
            finally
            {
                if(stream != null)  stream.Close();
            }
        }

        private void AddMenuShortcutsToArrayForSave()
        {
            List<KeyboardShortcut> menuShortcuts = new List<KeyboardShortcut>();
            foreach (KeyboardShortcut s in MenuNameDictionary.Values)
            {
                //if ( string.IsNullOrEmpty(s.Description)) MessageBox.Show(s.Value.ToString());
                if (s.IsMenuShortcut)  menuShortcuts.Add(s);
            }
            MenuKeyboardShortCutsList = new KeyboardShortcut[menuShortcuts.Count];
            int counter = 0;
            foreach (KeyboardShortcut k in menuShortcuts)
            {
                MenuKeyboardShortCutsList[counter] = k;
                counter++;
            }
        }

        public bool IsDuplicate(Keys keyData)
        {
            if (keyData == Keys.None) return false;
            foreach (KeyboardShortcut k in KeyboardShortcutsDescription.Values)
            {
                if (k.Value == keyData)
                {
                    return true;
                }
            }
            return false;
        }

        
        public bool AddMenuShortcut(string name,string text,Keys keyData)
        {
            if (!MenuNameDictionary.ContainsKey(name))
            {
                KeyboardShortcut k = new KeyboardShortcut(keyData, name) ;
                k.IsMenuShortcut = true;
                MenuNameDictionary.Add(name, k);
                
                KeyboardShortcutsDescription.Add(text, k);
                return true;
            }
            KeyboardShortcutsDescription.Add(text, MenuNameDictionary[name]);
            return false;
        }

        public static bool AddDefaultMenuShortcut(string name, Keys keyData)
        {
            if (!MenuNameDefaultShortcutDictionary.ContainsKey(name))
            {
                KeyboardShortcut k = new KeyboardShortcut(keyData, name);
                k.IsMenuShortcut = true;
                MenuNameDefaultShortcutDictionary.Add(name, k);

                return true;
            }
            return false;
        }

        /*
        private void PopulateMenuShortcutsDictionary()
        {
            if (MenuKeyboardShortCutsList != null)
            {
                for (int i = 0; i < MenuKeyboardShortCutsList.Length; i++)
                {
                    if (string.IsNullOrEmpty(MenuKeyboardShortCutsList[i].Description)) continue;
                                        KeyboardShortcutsDescription.Add(MenuKeyboardShortCutsList[i].Description, MenuKeyboardShortCutsList[i]);
                                        MenuKeyboardShortCutsList[i].IsMenuShortcut = true;
                }
            }
        }
         */ 
        private void PopulateKeyboardShortcutsDictionary()
        {
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectCompleteWaveform.Description), ContentView_SelectCompleteWaveform);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_PlaySelectedWaveform.Description), ContentView_PlaySelectedWaveform);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarNextSection.Description), ContentView_TransportBarNextSection);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviousSection.Description), ContentView_TransportBarPreviousSection);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviousPhrase.Description), ContentView_TransportBarPreviousPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarNextPhrase.Description), ContentView_TransportBarNextPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarNudgeForward.Description), ContentView_TransportBarNudgeForward);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarNudgeBackward.Description), ContentView_TransportBarNudgeBackward);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarFineNavigationOn.Description), ContentView_TransportBarFineNavigationOn);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarFineNavigationOff.Description), ContentView_TransportBarFineNavigationOff);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_MarkSelectionBeginTime.Description), ContentView_MarkSelectionBeginTime);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_MarkSelectionEndTime.Description), ContentView_MarkSelectionEndTime);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_ExpandAudioSelectionAtLeft.Description), ContentView_ExpandAudioSelectionAtLeft);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_ContractAudioSelectionAtLeft.Description), ContentView_ContractAudioSelectionAtLeft);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarNextPage.Description), ContentView_TransportBarNextPage);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviousPage.Description), ContentView_TransportBarPreviousPage);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviewFromAudioCursor.Description), ContentView_TransportBarPreviewFromAudioCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviewFromSelection.Description), ContentView_TransportBarPreviewFromSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviewUptoAudioCursor.Description), ContentView_TransportBarPreviewUptoAudioCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviewUptoSelection.Description), ContentView_TransportBarPreviewUptoSelection);

            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarExpandPlayOptions.Description), ContentView_TransportBarExpandPlayOptions);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarExpandRecordOptions.Description), ContentView_TransportBarExpandRecordOptions);
            if (ContentView_TransportBarExpandSwitchProfile == null) ContentView_TransportBarExpandSwitchProfile = new KeyboardShortcut(Keys.Control | Keys.F5, "KeysS_TransportBarExpandSwitchProfile");
                KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarExpandSwitchProfile.Description), ContentView_TransportBarExpandSwitchProfile);

            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_ZoomWaveformPanel.Description), ContentView_ZoomWaveformPanel);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarRecordSingleKey.Description), ContentView_TransportBarRecordSingleKey);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarStopSingleKey.Description), ContentView_TransportBarStopSingleKey);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_FastPlayStepDown.Description), ContentView_FastPlayStepDown);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_FastPlayStepUp.Description), ContentView_FastPlayStepUp);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_FastPlayRateNormilize.Description), ContentView_FastPlayRateNormilize);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_FastPlayNormalizeWithElapseBack.Description), ContentView_FastPlayNormalizeWithElapseBack);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_FastPlayWithElapseForward.Description), ContentView_FastPlayWithElapseForward);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_MarkSelectionFromBeginningToTheCursor.Description), ContentView_MarkSelectionFromBeginningToTheCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_MarkSelectionFromCursorToTheEnd.Description), ContentView_MarkSelectionFromCursorToTheEnd);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectPrecedingPhrase.Description), ContentView_SelectPrecedingPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectFollowingPhrase.Description), ContentView_SelectFollowingPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectLastPhraseInStrip.Description), ContentView_SelectLastPhraseInStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectFirstPhraseInStrip.Description), ContentView_SelectFirstPhraseInStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectNextPagePhrase.Description), ContentView_SelectNextPagePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectPrecedingPagePhrase.Description), ContentView_SelectPrecedingPagePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectNextSpecialRolePhrase.Description), ContentView_SelectNextSpecialRolePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectPrecedingSpecialRolePhrase.Description), ContentView_SelectPrecedingSpecialRolePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectNextEmptyPhrase.Description), ContentView_SelectNextEmptyPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectFirstSkippableNode.Description), ContentView_SelectFirstSkippableNode);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectLastSkippableNode.Description), ContentView_SelectLastSkippableNode);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectPrecedingStrip.Description), ContentView_SelectPrecedingStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectFollowingStrip.Description), ContentView_SelectFollowingStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectFirstStrip.Description), ContentView_SelectFirstStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectLastStrip.Description), ContentView_SelectLastStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectUp.Description), ContentView_SelectUp);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectPrecedingStripCursor.Description), ContentView_SelectPrecedingStripCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectFollowingStripCursor.Description), ContentView_SelectFollowingStripCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_ScrollDown_LargeIncrementWithSelection.Description), ContentView_ScrollDown_LargeIncrementWithSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_ScrollUp_LargeIncrementWithSelection.Description), ContentView_ScrollUp_LargeIncrementWithSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_ScrollUp_SmallIncrementWithSelection.Description), ContentView_ScrollUp_SmallIncrementWithSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_ScrollDown_SmallIncrementWithSelection.Description), ContentView_ScrollDown_SmallIncrementWithSelection);
            if (ContentView_SelectStartOfThePhrase == null) ContentView_SelectStartOfThePhrase = new KeyboardShortcut(Keys.Home | Keys.Alt | Keys.Control, "KeyS_SelectStartOfThePhrase");
                KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectStartOfThePhrase.Description), ContentView_SelectStartOfThePhrase);
                if (ContentView_SelectEndOfPhrase == null) ContentView_SelectEndOfPhrase = new KeyboardShortcut(Keys.End | Keys.Alt | Keys.Control, "KeyS_SelectEndOfPhrase");
                KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectEndOfPhrase.Description), ContentView_SelectEndOfPhrase);

            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_MoveToNextViewClockwise.Description), ProjectView_MoveToNextViewClockwise);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_MoveToPreviousViewAnticlockwise.Description), ProjectView_MoveToPreviousViewAnticlockwise);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_ToggleBetweenContentViewAndTOCView.Description), ProjectView_ToggleBetweenContentViewAndTOCView);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_PlayPauseUsingSelection.Description), ProjectView_PlayPauseUsingSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_PlayPauseUsingAudioCursor_Default.Description), ProjectView_PlayPauseUsingAudioCursor_Default);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_ShowPropertiesOfSelectedNode.Description), ProjectView_ShowPropertiesOfSelectedNode);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_FocusOnTransportBarTimeDisplay.Description), ProjectView_FocusOnTransportBarTimeDisplay);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_HardResetAllSettings.Description), ProjectView_HardResetAllSettings);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_PlayOnNavigate.Description), ProjectView_PlayOnNavigate);
            KeyboardShortcutsDescription.Add(Localizer.Message(ZoomPanel_Close.Description), ZoomPanel_Close);
            KeyboardShortcutsDescription.Add(Localizer.Message(ZoomPanel_NextPhrase.Description), ZoomPanel_NextPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ZoomPanel_PreviousPhrase.Description), ZoomPanel_PreviousPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ZoomPanel_Reset.Description), ZoomPanel_Reset);
            KeyboardShortcutsDescription.Add(Localizer.Message(ZoomPanel_ZoomIn.Description), ZoomPanel_ZoomIn);
            KeyboardShortcutsDescription.Add(Localizer.Message(ZoomPanel_ZoomOut.Description), ZoomPanel_ZoomOut);
            KeyboardShortcutsDescription.Add(Localizer.Message(ZoomPanel_ZoomSelection.Description), ZoomPanel_ZoomSelection);
        }

        private void PopulateUserFriendlyNamesForKeyboardKeys()
        {
          //  UserFriendlyNameDirectory = new Dictionary<string, KeyboardShortcut>();
            if (UserFriendlyNameDirectory != null)
            {
                UserFriendlyNameDirectory.Add(Keys.D0, "0");
                UserFriendlyNameDirectory.Add(Keys.D1, "1");
                UserFriendlyNameDirectory.Add(Keys.D2, "2");
                UserFriendlyNameDirectory.Add(Keys.D3, "3");
                UserFriendlyNameDirectory.Add(Keys.D4, "4");
                UserFriendlyNameDirectory.Add(Keys.D5, "5");
                UserFriendlyNameDirectory.Add(Keys.D6, "6");
                UserFriendlyNameDirectory.Add(Keys.D7, "7");
                UserFriendlyNameDirectory.Add(Keys.D8, "8");
                UserFriendlyNameDirectory.Add(Keys.D9, "9");
                UserFriendlyNameDirectory.Add(Keys.Divide, "/");
                UserFriendlyNameDirectory.Add(Keys.Oemcomma, "Comma");
                UserFriendlyNameDirectory.Add(Keys.OemMinus, "-");
                UserFriendlyNameDirectory.Add(Keys.OemPeriod, "Period");
                UserFriendlyNameDirectory.Add(Keys.Oemplus, "+");
                UserFriendlyNameDirectory.Add(Keys.OemSemicolon, ";");
                UserFriendlyNameDirectory.Add(Keys.Control, "Ctrl");
                UserFriendlyNameDirectory.Add(Keys.Next,"Page Down");
                UserFriendlyNameDirectory.Add(Keys.PageUp, "Page Up");
                UserFriendlyNameDirectory.Add(Keys.OemOpenBrackets, "[");
                UserFriendlyNameDirectory.Add(Keys.Oem6, "]");
                
            }
        }
        public string FormatKeyboardShorcut(string str)
        {
            if( UserFriendlyNameDirectory == null || UserFriendlyNameDirectory.Count == 0 )
            {
                UserFriendlyNameDirectory = new Dictionary<Keys, string>();
                PopulateUserFriendlyNamesForKeyboardKeys();
            }
            string[] tempStore = str.Split(',');
            if (tempStore.Length == 0) return "";
            string formattedString = "";
            for (int i = tempStore.Length - 1; i >= 0; i--)
            {
                Keys tempKey = (Keys)Enum.Parse(typeof(Keys), tempStore[i]);
                if (this.UserFriendlyNameDirectory != null)
                {
                    if (this.UserFriendlyNameDirectory.ContainsKey(tempKey))
                    {
                        tempStore[i] = this.UserFriendlyNameDirectory[tempKey];
                    }
                    formattedString += tempStore[i];
                    if (i > 0) formattedString += "+";
                }
            }

            return formattedString;
        }

        public bool Compare(KeyboardShortcuts_Settings shortcuts)
        {
            if (this.KeyboardShortcutsDescription.Count > 0
                && shortcuts != null &&  shortcuts.KeyboardShortcutsDescription.Count > 0)
            {
                foreach (string desc in this.KeyboardShortcutsDescription.Keys)
                {
                    if (!shortcuts.KeyboardShortcutsDescription.ContainsKey(desc)) return false;

                    if (shortcuts.KeyboardShortcutsDescription[desc].Value != this.KeyboardShortcutsDescription[desc].Value) return false;
                }
                // if it does not returns due to any of the mismatch above then both of the instances have the same value
                return true;
            }
            return false;
        }

    }

        [Serializable()]
        public class KeyboardShortcut
        {
            public Keys Value;
            public string Description;
            public bool IsMenuShortcut;

public KeyboardShortcut(Keys keyData, string description)
            {
                Value = keyData;
                Description = description;
                IsMenuShortcut = false ;
            }

            public KeyboardShortcut Copy()
            {
                KeyboardShortcut k = new KeyboardShortcut(this.Value, this.Description);
                k.IsMenuShortcut = this.IsMenuShortcut;
                return k;
            }
    }
}
