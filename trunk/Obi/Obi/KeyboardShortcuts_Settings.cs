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
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectCompleteWaveform"), ContentView_SelectCompleteWaveform);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_PlaySelectedWaveform"), ContentView_PlaySelectedWaveform);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarNextSection"), ContentView_TransportBarNextSection);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarPreviousSection"), ContentView_TransportBarPreviousSection);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarPreviousPhrase"), ContentView_TransportBarPreviousPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarNextPhrase"), ContentView_TransportBarNextPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarNudgeForward"), ContentView_TransportBarNudgeForward);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarNudgeBackward"), ContentView_TransportBarNudgeBackward);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_MarkSelectionBeginTime"), ContentView_MarkSelectionBeginTime);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_MarkSelectionEndTime"), ContentView_MarkSelectionEndTime);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarNextPage"), ContentView_TransportBarNextPage);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarPreviousPage"), ContentView_TransportBarPreviousPage);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarPreviewFromAudioCursor"), ContentView_TransportBarPreviewFromAudioCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarPreviewFromSelection"), ContentView_TransportBarPreviewFromSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarPreviewUptoAudioCursor"), ContentView_TransportBarPreviewUptoAudioCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_TransportBarPreviewUptoSelection"), ContentView_TransportBarPreviewUptoSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_FastPlayStepDown"), ContentView_FastPlayStepDown);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_FastPlayStepUp"), ContentView_FastPlayStepUp);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_FastPlayRateNormilize"), ContentView_FastPlayRateNormilize);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_FastPlayNormalizeWithElapseBack"), ContentView_FastPlayNormalizeWithElapseBack);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_MarkSelectionFromCursor"), ContentView_MarkSelectionFromCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_MarkSelectionToCursor"), ContentView_MarkSelectionToCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectPrecedingPhrase"), ContentView_SelectPrecedingPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectFollowingPhrase"), ContentView_SelectFollowingPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectLastPhraseInStrip"), ContentView_SelectLastPhraseInStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectFirstPhraseInStrip"), ContentView_SelectFirstPhraseInStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectNextPagePhrase"), ContentView_SelectNextPagePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectPrecedingPagePhrase"), ContentView_SelectPrecedingPagePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectNextSpecialRolePhrase"), ContentView_SelectNextSpecialRolePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectPrecedingSpecialRolePhrase"), ContentView_SelectPrecedingSpecialRolePhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectNextEmptyPhrase"), ContentView_SelectNextEmptyPhrase);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectPrecedingStrip"), ContentView_SelectPrecedingStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectFollowingStrip"), ContentView_SelectFollowingStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectFirstStrip"), ContentView_SelectFirstStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectLastStrip"), ContentView_SelectLastStrip);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectUp"), ContentView_SelectUp);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectPrecedingStripCursor"), ContentView_SelectPrecedingStripCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_SelectFollowingStripCursor"), ContentView_SelectFollowingStripCursor);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_ScrollDown_LargeIncrementWithSelection"), ContentView_ScrollDown_LargeIncrementWithSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_ScrollUp_LargeIncrementWithSelection"), ContentView_ScrollUp_LargeIncrementWithSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_ScrollUp_SmallIncrementWithSelection"), ContentView_ScrollUp_SmallIncrementWithSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_ScrollDown_SmallIncrementWithSelection"), ContentView_ScrollDown_SmallIncrementWithSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_MoveToNextViewClockwise"), ProjectView_MoveToNextViewClockwise);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_MoveToPreviousViewAnticlockwise"), ProjectView_MoveToPreviousViewAnticlockwise);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_ToggleBetweenContentViewAndTOCView"), ProjectView_ToggleBetweenContentViewAndTOCView);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_PlayPauseUsingSelection"), ProjectView_PlayPauseUsingSelection);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_PlayPauseUsingAudioCursor_Default"), ProjectView_PlayPauseUsingAudioCursor_Default);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_ShowPropertiesOfSelectedNode"), ProjectView_ShowPropertiesOfSelectedNode);
            KeyboardShortcutsDescription.Add(Localizer.Message("KeyS_FocusOnTransportBarTimeDisplay"), ProjectView_FocusOnTransportBarTimeDisplay);
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
