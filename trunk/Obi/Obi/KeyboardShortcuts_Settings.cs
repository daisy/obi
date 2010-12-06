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

        public KeyboardShortcut ContentView_SelectCompleteWaveform = new KeyboardShortcut( Keys.A);
        public KeyboardShortcut ContentView_PlaySelectedWaveform = new KeyboardShortcut(Keys.C);
        public KeyboardShortcut ContentView_TransportBarNextSection = new KeyboardShortcut( Keys.H);
        public KeyboardShortcut ContentView_TransportBarPreviousSection = new KeyboardShortcut( Keys.Shift | Keys.H);
        public KeyboardShortcut ContentView_TransportBarPreviousPhrase = new KeyboardShortcut( Keys.J);
        public KeyboardShortcut ContentView_TransportBarNextPhrase = new KeyboardShortcut( Keys.K );
        public KeyboardShortcut ContentView_TransportBarNudgeForward = new KeyboardShortcut(Keys.N);
        public KeyboardShortcut ContentView_TransportBarNudgeBackward = new KeyboardShortcut( Keys.Shift | Keys.N);
        public KeyboardShortcut ContentView_MarkSelectionBeginTime = new KeyboardShortcut( Keys.OemOpenBrackets);
        public KeyboardShortcut ContentView_MarkSelectionEndTime = new KeyboardShortcut( Keys.OemCloseBrackets);
        public KeyboardShortcut ContentView_TransportBarNextPage = new KeyboardShortcut( Keys.P);
        public KeyboardShortcut ContentView_TransportBarPreviousPage = new KeyboardShortcut( Keys.Shift | Keys.P);
        public KeyboardShortcut ContentView_TransportBarPreviewFromAudioCursor = new KeyboardShortcut( Keys.V);
        public KeyboardShortcut ContentView_TransportBarPreviewFromSelection = new KeyboardShortcut( Keys.Shift | Keys.V);
        public KeyboardShortcut ContentView_TransportBarPreviewUptoAudioCursor = new KeyboardShortcut( Keys.X);
        public KeyboardShortcut ContentView_TransportBarPreviewUptoSelection = new KeyboardShortcut( Keys.Shift | Keys.X);

        // playback shortcuts.

        public KeyboardShortcut ContentView_FastPlayStepDown = new KeyboardShortcut( Keys.S);
        public KeyboardShortcut ContentView_FastPlayStepUp = new KeyboardShortcut( Keys.F);
        public KeyboardShortcut ContentView_FastPlayRateNormilize = new KeyboardShortcut( Keys.D);
        public KeyboardShortcut ContentView_FastPlayNormalizeWithElapseBack = new KeyboardShortcut( Keys.E);
        public KeyboardShortcut ContentView_MarkSelectionFromCursor = new KeyboardShortcut( Keys.Shift | Keys.OemOpenBrackets);
        public KeyboardShortcut ContentView_MarkSelectionToCursor = new KeyboardShortcut( Keys.Shift | Keys.OemCloseBrackets);


        // Strips navigation
        public KeyboardShortcut ContentView_SelectPrecedingPhrase = new KeyboardShortcut( Keys.Left);
        public KeyboardShortcut ContentView_SelectFollowingPhrase = new KeyboardShortcut( Keys.Right);
        public KeyboardShortcut ContentView_SelectLastPhraseInStrip = new KeyboardShortcut( Keys.End);
        public KeyboardShortcut ContentView_SelectFirstPhraseInStrip = new KeyboardShortcut( Keys.Home);
        public KeyboardShortcut ContentView_SelectNextPagePhrase = new KeyboardShortcut( Keys.Control | Keys.PageDown);
        public KeyboardShortcut ContentView_SelectPrecedingPagePhrase = new KeyboardShortcut( Keys.Control | Keys.PageUp);
        public KeyboardShortcut ContentView_SelectNextSpecialRolePhrase = new KeyboardShortcut( Keys.F4);
        public KeyboardShortcut ContentView_SelectPrecedingSpecialRolePhrase = new KeyboardShortcut( Keys.Shift | Keys.F4);
        public KeyboardShortcut ContentView_SelectNextEmptyPhrase = new KeyboardShortcut( Keys.Control | Keys.Alt | Keys.F4);

        //public KeyboardShortcut ContentView_SelectPrecedingStrip = Keys.Control | Keys.Up;
        //public KeyboardShortcut ContentView_SelectFollowingStrip =  Keys.Control | Keys.Down;
        public KeyboardShortcut ContentView_SelectPrecedingStrip = new KeyboardShortcut( Keys.Control | Keys.Shift | Keys.Up);
        public KeyboardShortcut ContentView_SelectFollowingStrip = new KeyboardShortcut( Keys.Control | Keys.Shift | Keys.Down);
        public KeyboardShortcut ContentView_SelectFirstStrip = new KeyboardShortcut( Keys.Control | Keys.Home);
        public KeyboardShortcut ContentView_SelectLastStrip = new KeyboardShortcut( Keys.Control | Keys.End);

        public KeyboardShortcut ContentView_SelectUp = new KeyboardShortcut( Keys.Escape);

        // Control + arrows moves the strip cursor
        public KeyboardShortcut ContentView_SelectPrecedingStripCursor = new KeyboardShortcut( Keys.Control | Keys.Left);
        public KeyboardShortcut ContentView_SelectFollowingStripCursor = new KeyboardShortcut( Keys.Control | Keys.Right);

        public KeyboardShortcut ContentView_ScrollDown_LargeIncrementWithSelection = new KeyboardShortcut( Keys.PageDown);
        public KeyboardShortcut ContentView_ScrollUp_LargeIncrementWithSelection = new KeyboardShortcut( Keys.PageUp);
        public KeyboardShortcut ContentView_ScrollDown_SmallIncrementWithSelection = new KeyboardShortcut( Keys.Down);
        public KeyboardShortcut ContentView_ScrollUp_SmallIncrementWithSelection = new KeyboardShortcut( Keys.Up);

        //project view shortcuts
        public KeyboardShortcut ProjectView_MoveToNextViewClockwise= new  KeyboardShortcut (Keys.Control | Keys.Tab);
            public KeyboardShortcut ProjectView_MoveToPreviousViewAnticlockwise = new KeyboardShortcut (Keys.Control | Keys.Shift | Keys.Tab);
            public KeyboardShortcut ProjectView_ToggleBetweenContentViewAndTOCView = new KeyboardShortcut ( Keys.F6) ;
            public KeyboardShortcut ProjectView_PlayPauseUsingSelection = new KeyboardShortcut (Keys.Shift | Keys.Space);
            public KeyboardShortcut ProjectView_PlayPauseUsingAudioCursor_Default = new KeyboardShortcut (Keys.Space);
            public KeyboardShortcut ProjectView_ShowPropertiesOfSelectedNode = new KeyboardShortcut (Keys.Alt | Keys.Enter);
        public KeyboardShortcut ProjectView_FocusOnTransportBarTimeDisplay = new KeyboardShortcut(Keys.F8);

        private static readonly string SETTINGS_FILE_NAME = "obi_KeyboardShortcuts_Settings.xml";

        [NonSerialized()]
        public Dictionary<string, KeyboardShortcut> KeyboardShortcutsDescription = new Dictionary<string, KeyboardShortcut>();

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
            settings.KeyboardShortcutsDescription = new Dictionary<string, KeyboardShortcut>();
            settings.PopulateKeyboardShortcutsDictionary();
            return settings;
        }

        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        public void SaveSettings()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            IsolatedStorageFileStream stream =
                new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Create, FileAccess.Write, file);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }

        private void PopulateKeyboardShortcutsDictionary()
        {
            KeyboardShortcutsDescription.Add("Select complete waveform of phrase", ContentView_SelectCompleteWaveform);
            KeyboardShortcutsDescription.Add("Play selected waveform", ContentView_PlaySelectedWaveform);
            KeyboardShortcutsDescription.Add("Next section in playlist", ContentView_TransportBarNextSection);
            KeyboardShortcutsDescription.Add("Previous section in playlist", ContentView_TransportBarPreviousSection);
            KeyboardShortcutsDescription.Add("Previous phrase in playlist", ContentView_TransportBarPreviousPhrase);
            KeyboardShortcutsDescription.Add("Next phrase in playlist", ContentView_TransportBarNextPhrase);
            KeyboardShortcutsDescription.Add("Nudge forward", ContentView_TransportBarNudgeForward);
            KeyboardShortcutsDescription.Add("Nudge backward", ContentView_TransportBarNudgeBackward);
            KeyboardShortcutsDescription.Add("Mark selection begin time", ContentView_MarkSelectionBeginTime);
            KeyboardShortcutsDescription.Add("Mark selection end time", ContentView_MarkSelectionEndTime);
            KeyboardShortcutsDescription.Add("Next page in playlist", ContentView_TransportBarNextPage);
            KeyboardShortcutsDescription.Add("Previous page in playlist", ContentView_TransportBarPreviousPage);
            KeyboardShortcutsDescription.Add("Preview from audio cursor", ContentView_TransportBarPreviewFromAudioCursor);
            KeyboardShortcutsDescription.Add("Preview from selection", ContentView_TransportBarPreviewFromSelection);
            KeyboardShortcutsDescription.Add("Preview upto audio cursor", ContentView_TransportBarPreviewUptoAudioCursor);
            KeyboardShortcutsDescription.Add("Preview upto selection", ContentView_TransportBarPreviewUptoSelection);
            KeyboardShortcutsDescription.Add("Fast play step down", ContentView_FastPlayStepDown);
            KeyboardShortcutsDescription.Add("Fast play step up", ContentView_FastPlayStepUp);
            KeyboardShortcutsDescription.Add("Fast play rate normilize", ContentView_FastPlayRateNormilize);
            KeyboardShortcutsDescription.Add("Fast play normalize with elapse back", ContentView_FastPlayNormalizeWithElapseBack);
            KeyboardShortcutsDescription.Add("Mark selection from cursor", ContentView_MarkSelectionFromCursor);
            KeyboardShortcutsDescription.Add("Mark selection to cursor", ContentView_MarkSelectionToCursor);
            KeyboardShortcutsDescription.Add("Select preceding phrase", ContentView_SelectPrecedingPhrase);
            KeyboardShortcutsDescription.Add("Select following phrase", ContentView_SelectFollowingPhrase);
            KeyboardShortcutsDescription.Add("Select last phrase in strip", ContentView_SelectLastPhraseInStrip);
            KeyboardShortcutsDescription.Add("Select first phrase in strip", ContentView_SelectFirstPhraseInStrip);
            KeyboardShortcutsDescription.Add("Select next page phrase", ContentView_SelectNextPagePhrase);
            KeyboardShortcutsDescription.Add("Select preceding page phrase", ContentView_SelectPrecedingPagePhrase);
            KeyboardShortcutsDescription.Add("Select next special role phrase", ContentView_SelectNextSpecialRolePhrase);
            KeyboardShortcutsDescription.Add("Select preceding special role phrase", ContentView_SelectPrecedingSpecialRolePhrase);
            KeyboardShortcutsDescription.Add("Select next empty phrase", ContentView_SelectNextEmptyPhrase);
            KeyboardShortcutsDescription.Add("Select preceding strip", ContentView_SelectPrecedingStrip);
            KeyboardShortcutsDescription.Add("Select following strip", ContentView_SelectFollowingStrip);
            KeyboardShortcutsDescription.Add("Select first strip", ContentView_SelectFirstStrip);
            KeyboardShortcutsDescription.Add("Select last strip", ContentView_SelectLastStrip);
            KeyboardShortcutsDescription.Add("Select up", ContentView_SelectUp);
            KeyboardShortcutsDescription.Add("Move strip cursor backward", ContentView_SelectPrecedingStripCursor);
            KeyboardShortcutsDescription.Add("Move strip cursor ahead and backward", ContentView_SelectFollowingStripCursor);
            KeyboardShortcutsDescription.Add("One screen down", ContentView_ScrollDown_LargeIncrementWithSelection);
            KeyboardShortcutsDescription.Add("One screen up", ContentView_ScrollUp_LargeIncrementWithSelection);
            KeyboardShortcutsDescription.Add("One line up", ContentView_ScrollUp_SmallIncrementWithSelection);
            KeyboardShortcutsDescription.Add("One line down", ContentView_ScrollDown_SmallIncrementWithSelection);
        }
        [Serializable()]
        public class KeyboardShortcut
        {
            public Keys Value;
            public KeyboardShortcut(Keys keyData)
            {
                Value = keyData;
            }

        }
    }
}
