using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms ;

namespace Obi
{
    /// <summary>
    /// Persistent application settings.
    /// </summary>
    /// <remarks>It also seems that making a change in the class resets the existing settings.</remarks>
    [Serializable()]
    public class KeyboardShortcuts_Settings
    {
            public Keys ContentView_SelectCompleteWaveform  =Keys.A;
            public Keys ContentView_PlaySelectedWaveform = Keys.C;
            public Keys ContentView_TransportBarNextSection = Keys.H;
            public Keys ContentView_TransportBarPreviousSection = Keys.Shift | Keys.H;
            public Keys ContentView_TransportBarPreviousPhrase = Keys.J;
            public Keys ContentView_TransportBarNextPhrase = Keys.K;
            public Keys ContentView_TransportBarNudgeForward = Keys.N;
            public Keys ContentView_TransportBarNudgeBackward = Keys.Shift | Keys.N;
            public Keys ContentView_MarkSelectionBeginTime =  Keys.OemOpenBrackets ;
            public Keys ContentView_MarkSelectionEndTime =Keys.OemCloseBrackets ;
            public Keys ContentView_TransportBarNextPage = Keys.P;
            public Keys ContentView_TransportBarPreviousPage = Keys.Shift | Keys.P;
            public Keys ContentView_TransportBarPreviewFromAudioCursor = Keys.V;
            public Keys ContentView_TransportBarPreviewFromSelection = Keys.Shift | Keys.V;
            public Keys ContentView_TransportBarPreviewUptoAudioCursor =  Keys.X;
            public Keys ContentView_TransportBarPreviewUptoSelection = Keys.Shift | Keys.X;

            // playback shortcuts.

             public Keys ContentView_FastPlayStepDown = Keys.S;
            public Keys ContentView_FastPlayStepUp =  Keys.F ;
            public Keys ContentView_FastPlayRateNormilize = Keys.D;
            public Keys ContentView_FastPlayNormalizeWithElapseBack =  Keys.E;
            public Keys ContentView_MarkSelectionFromCursor = Keys.Shift | Keys.OemOpenBrackets;
            public Keys ContentView_MarkSelectionToCursor = Keys.Shift | Keys.OemCloseBrackets;


            // Strips navigation
            public Keys ContentView_SelectPrecedingPhrase =  Keys.Left;
            public Keys ContentView_SelectFollowingPhrase = Keys.Right;
            public Keys ContentView_SelectLastPhraseInStrip = Keys.End;
            public Keys ContentView_SelectFirstPhraseInStrip = Keys.Home ;
            public Keys ContentView_SelectNextPagePhrase =  Keys.Control | Keys.PageDown;
            public Keys ContentView_SelectPrecedingPagePhrase =  Keys.Control | Keys.PageUp ;
            public Keys ContentView_SelectNextSpecialRolePhrase =  Keys.F4;
            public Keys ContentView_SelectPrecedingSpecialRolePhrase = Keys.Shift | Keys.F4 ;
            public Keys ContentView_SelectNextEmptyPhrase = Keys.Control | Keys.Alt | Keys.F4 ;

            //public Keys ContentView_SelectPrecedingStrip = Keys.Control | Keys.Up;
            //public Keys ContentView_SelectFollowingStrip =  Keys.Control | Keys.Down;
            public Keys ContentView_SelectPrecedingStrip = Keys.Control | Keys.Shift | Keys.Up ;
            public Keys ContentView_SelectFollowingStrip  = Keys.Control | Keys.Shift | Keys.Down ;
            public Keys ContentView_SelectFirstStrip = Keys.Control | Keys.Home ;
            public Keys ContentView_SelectLastStrip = Keys.Control | Keys.End ;

            public Keys ContentView_SelectUp = Keys.Escape ;

            // Control + arrows moves the strip cursor
            public Keys ContentView_SelectPrecedingStripCursor = Keys.Control | Keys.Left ;
            public Keys ContentView_SelectFollowingStripCursor = Keys.Control | Keys.Right ;

            public Keys ContentView_ScrollDown_LargeIncrementWithSelection = Keys.PageDown ;
            public Keys ContentView_ScrollUp_LargeIncrementWithSelection = Keys.PageUp ;
            public Keys ContentView_ScrollDown_SmallIncrementWithSelection = Keys.Down ;
            public Keys ContentView_ScrollUp_SmallIncrementWithSelection = Keys.Up ;
        
        private static readonly string SETTINGS_FILE_NAME = "obi_KeyboardShortcuts_Settings.xml";


        /// <summary>
        /// Read the settings from the settings file; missing values are replaced with defaults.
        /// </summary>
        /// <remarks>Errors are silently ignored and default settings are returned.</remarks>
        public static KeyboardShortcuts_Settings GetKeyboardShortcuts_Settings()
        {
            KeyboardShortcuts_Settings settings = new KeyboardShortcuts_Settings ();
            
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
    }
}
