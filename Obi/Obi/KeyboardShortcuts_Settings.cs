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

        public KeyboardShortcut ContentView_SelectCompleteWaveform = new KeyboardShortcut(Keys.A, "KeyS_SelectCompleteWaveform");
        public KeyboardShortcut ContentView_PlaySelectedWaveform = new KeyboardShortcut(Keys.C, "KeyS_PlaySelectedWaveform");
        public KeyboardShortcut ContentView_TransportBarNextSection = new KeyboardShortcut(Keys.H, "KeyS_TransportBarNextSection");
        public KeyboardShortcut ContentView_TransportBarPreviousSection = new KeyboardShortcut(Keys.Shift | Keys.H, "KeyS_TransportBarPreviousSection");
        public KeyboardShortcut ContentView_TransportBarPreviousPhrase = new KeyboardShortcut(Keys.J, "KeyS_TransportBarPreviousPhrase");
        public KeyboardShortcut ContentView_TransportBarNextPhrase = new KeyboardShortcut(Keys.K, "KeyS_TransportBarNextPhrase");
        public KeyboardShortcut ContentView_TransportBarNudgeForward = new KeyboardShortcut(Keys.N, "KeyS_TransportBarNudgeForward");
        public KeyboardShortcut ContentView_TransportBarNudgeBackward = new KeyboardShortcut(Keys.Shift | Keys.N, "KeyS_TransportBarNudgeBackward");
        public KeyboardShortcut ContentView_MarkSelectionBeginTime = new KeyboardShortcut(Keys.OemOpenBrackets, "KeyS_MarkSelectionBeginTime");
        public KeyboardShortcut ContentView_MarkSelectionEndTime = new KeyboardShortcut(Keys.OemCloseBrackets, "KeyS_MarkSelectionEndTime");
        public KeyboardShortcut ContentView_TransportBarNextPage = new KeyboardShortcut(Keys.P, "KeyS_TransportBarNextPage");
        public KeyboardShortcut ContentView_TransportBarPreviousPage = new KeyboardShortcut(Keys.Shift | Keys.P, "KeyS_TransportBarPreviousPage");
        public KeyboardShortcut ContentView_TransportBarPreviewFromAudioCursor = new KeyboardShortcut(Keys.V, "KeyS_TransportBarPreviewFromAudioCursor");
        public KeyboardShortcut ContentView_TransportBarPreviewFromSelection = new KeyboardShortcut(Keys.Shift | Keys.V, "KeyS_TransportBarPreviewFromSelection");
        public KeyboardShortcut ContentView_TransportBarPreviewUptoAudioCursor = new KeyboardShortcut(Keys.X, "KeyS_TransportBarPreviewUptoAudioCursor");
        public KeyboardShortcut ContentView_TransportBarPreviewUptoSelection = new KeyboardShortcut(Keys.Shift | Keys.X, "KeyS_TransportBarPreviewUptoSelection");

        // playback shortcuts.

        public KeyboardShortcut ContentView_FastPlayStepDown = new KeyboardShortcut(Keys.S, "KeyS_FastPlayStepDown");
        public KeyboardShortcut ContentView_FastPlayStepUp = new KeyboardShortcut( Keys.F, "KeyS_FastPlayStepUp");
        public KeyboardShortcut ContentView_FastPlayRateNormilize = new KeyboardShortcut(Keys.D, "KeyS_FastPlayRateNormilize");
        public KeyboardShortcut ContentView_FastPlayNormalizeWithElapseBack = new KeyboardShortcut(Keys.E, "KeyS_FastPlayNormalizeWithElapseBack");
        public KeyboardShortcut ContentView_MarkSelectionFromCursor = new KeyboardShortcut(Keys.Shift | Keys.OemOpenBrackets, "KeyS_MarkSelectionFromCursor");
        public KeyboardShortcut ContentView_MarkSelectionToCursor = new KeyboardShortcut(Keys.Shift | Keys.OemCloseBrackets, "KeyS_MarkSelectionToCursor");


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
        public KeyboardShortcut ContentView_ScrollDown_SmallIncrementWithSelection = new KeyboardShortcut(Keys.Down, "KeyS_ScrollUp_SmallIncrementWithSelection");
        public KeyboardShortcut ContentView_ScrollUp_SmallIncrementWithSelection = new KeyboardShortcut(Keys.Up, "KeyS_ScrollDown_SmallIncrementWithSelection");

        //project view shortcuts
        public KeyboardShortcut ProjectView_MoveToNextViewClockwise = new KeyboardShortcut(Keys.Control | Keys.Tab, "KeyS_MoveToNextViewClockwise");
        public KeyboardShortcut ProjectView_MoveToPreviousViewAnticlockwise = new KeyboardShortcut(Keys.Control | Keys.Shift | Keys.Tab, "KeyS_MoveToPreviousViewAnticlockwise");
        public KeyboardShortcut ProjectView_ToggleBetweenContentViewAndTOCView = new KeyboardShortcut(Keys.F6, "KeyS_ToggleBetweenContentViewAndTOCView");
        public KeyboardShortcut ProjectView_PlayPauseUsingSelection = new KeyboardShortcut(Keys.Shift | Keys.Space, "KeyS_PlayPauseUsingSelection");
        public KeyboardShortcut ProjectView_PlayPauseUsingAudioCursor_Default = new KeyboardShortcut(Keys.Space, "KeyS_PlayPauseUsingAudioCursor_Default");
        public KeyboardShortcut ProjectView_ShowPropertiesOfSelectedNode = new KeyboardShortcut(Keys.Alt | Keys.Enter, "KeyS_ShowPropertiesOfSelectedNode");
        public KeyboardShortcut ProjectView_FocusOnTransportBarTimeDisplay = new KeyboardShortcut(Keys.F8, "KeyS_FocusOnTransportBarTimeDisplay");

        public KeyboardShortcut[] MenuKeyboardShortCutsList;

        private static readonly string SETTINGS_FILE_NAME = "obi_KeyboardShortcuts_Settings.xml";

        [NonSerialized()]
        public Dictionary<string, KeyboardShortcut> KeyboardShortcutsDescription = new Dictionary<string, KeyboardShortcut>();

        //[NonSerialized()]
        //public Dictionary<string, KeyboardShortcut> MenuKeyboardShortcutsDictionary;
        public KeyboardShortcuts_Settings()
        {
            //KeyboardShortcutsDescription.Add(this.ContentView_SelectCompleteWaveform, "Select complete waveform of phrase");
            //KeyboardShortcutsDescription.Add(ContentView_PlaySelectedWaveform, "Play selected waveform");
        }

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
            catch (Exception) { }
            
            //settings.MenuKeyboardShortcutsDictionary = new Dictionary<string, KeyboardShortcut>();
            
            settings.KeyboardShortcutsDescription = new Dictionary<string, KeyboardShortcut>();
            settings.PopulateKeyboardShortcutsDictionary();
            settings.PopulateMenuShortcutsDictionary();
            return settings;
        }

        public static KeyboardShortcuts_Settings GetDefaultKeyboardShortcuts_Settings()
        {
            KeyboardShortcuts_Settings settings = new KeyboardShortcuts_Settings();

            if (settings.KeyboardShortcutsDescription != null) settings.KeyboardShortcutsDescription.Clear();
            settings.KeyboardShortcutsDescription = new Dictionary<string, KeyboardShortcut>();
            settings.PopulateKeyboardShortcutsDictionary();
            settings.PopulateMenuShortcutsDictionary();
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

        private void AddMenuShortcutsToArrayForSave()
        {
            List<KeyboardShortcut> menuShortcuts = new List<KeyboardShortcut>();
            foreach (KeyboardShortcut s in KeyboardShortcutsDescription.Values)
            {
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

        
        public bool AddMenuShortcut(string name, Keys keyData)
        {
            if (!KeyboardShortcutsDescription.ContainsKey(name))
            {
                KeyboardShortcut k = new KeyboardShortcut(keyData) ;
                k.IsMenuShortcut = true;
                KeyboardShortcutsDescription.Add(name, k);
                return true;
            }
            return false;
        }

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
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_MarkSelectionBeginTime.Description), ContentView_MarkSelectionBeginTime);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_MarkSelectionEndTime.Description), ContentView_MarkSelectionEndTime);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarNextPage.Description), ContentView_TransportBarNextPage);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviousPage.Description), ContentView_TransportBarPreviousPage);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviewFromAudioCursor.Description), ContentView_TransportBarPreviewFromAudioCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviewFromSelection.Description), ContentView_TransportBarPreviewFromSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviewUptoAudioCursor.Description), ContentView_TransportBarPreviewUptoAudioCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_TransportBarPreviewUptoSelection.Description), ContentView_TransportBarPreviewUptoSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_FastPlayStepDown.Description), ContentView_FastPlayStepDown);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_FastPlayStepUp.Description), ContentView_FastPlayStepUp);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_FastPlayRateNormilize.Description), ContentView_FastPlayRateNormilize);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_FastPlayNormalizeWithElapseBack.Description), ContentView_FastPlayNormalizeWithElapseBack);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_MarkSelectionFromCursor.Description), ContentView_MarkSelectionFromCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_MarkSelectionToCursor.Description), ContentView_MarkSelectionToCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectPrecedingPhrase.Description), ContentView_SelectPrecedingPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectFollowingPhrase.Description), ContentView_SelectFollowingPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectLastPhraseInStrip.Description), ContentView_SelectLastPhraseInStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectFirstPhraseInStrip.Description), ContentView_SelectFirstPhraseInStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectNextPagePhrase.Description), ContentView_SelectNextPagePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectPrecedingPagePhrase.Description), ContentView_SelectPrecedingPagePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectNextSpecialRolePhrase.Description), ContentView_SelectNextSpecialRolePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectPrecedingSpecialRolePhrase.Description), ContentView_SelectPrecedingSpecialRolePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message(ContentView_SelectNextEmptyPhrase.Description), ContentView_SelectNextEmptyPhrase);
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
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_MoveToNextViewClockwise.Description), ProjectView_MoveToNextViewClockwise);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_MoveToPreviousViewAnticlockwise.Description), ProjectView_MoveToPreviousViewAnticlockwise);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_ToggleBetweenContentViewAndTOCView.Description), ProjectView_ToggleBetweenContentViewAndTOCView);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_PlayPauseUsingSelection.Description), ProjectView_PlayPauseUsingSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_PlayPauseUsingAudioCursor_Default.Description), ProjectView_PlayPauseUsingAudioCursor_Default);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_ShowPropertiesOfSelectedNode.Description), ProjectView_ShowPropertiesOfSelectedNode);
            KeyboardShortcutsDescription.Add(Localizer.Message(ProjectView_FocusOnTransportBarTimeDisplay.Description), ProjectView_FocusOnTransportBarTimeDisplay);
        }
        [Serializable()]
        public class KeyboardShortcut
        {
            public Keys Value;
            public string Description;
            public bool IsMenuShortcut;

            public KeyboardShortcut(Keys keyData)
            {
                Value = keyData;
                IsMenuShortcut = false;
            }
            public KeyboardShortcut(Keys keyData, string description)
            {
                Value = keyData;
                Description = description;
                IsMenuShortcut = false ;
            }

        }
    }
}
