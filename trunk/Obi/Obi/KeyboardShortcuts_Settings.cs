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
